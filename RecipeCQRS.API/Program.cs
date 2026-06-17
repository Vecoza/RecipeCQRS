using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using FluentValidation;
using RecipeCQRS.API.Middleware;
using RecipeCQRS.Application.Common.Behaviors;
using RecipeCQRS.Application.Common.Interfaces;
using RecipeCQRS.Application.Entities;
using RecipeCQRS.Infrastructure.Data;
using RecipeCQRS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IAppDbContext>(
    p => p.GetRequiredService<AppDbContext>());

// ── 2. Identity ────────────────────────────────────────────────────────────────
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase       = false;
    opt.Password.RequiredLength         = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager();

// ── 3. JWT Authentication ──────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["JwtSettings:Secret"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer           = true,
            ValidIssuer              = builder.Configuration["JwtSettings:Issuer"],
            ValidateAudience         = true,
            ValidAudience            = builder.Configuration["JwtSettings:Audience"],
            ValidateLifetime         = true
        };
    });

// ── 4. MediatR ─────────────────────────────────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoggingBehavior<,>).Assembly));

// ── 5. Pipeline Behaviors — ORDER MATTERS (logging outer, validation inner) ────
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ── 6. FluentValidation ────────────────────────────────────────────────────────
builder.Services.AddValidatorsFromAssembly(typeof(ValidationBehavior<,>).Assembly);

// ── 7. Services ────────────────────────────────────────────────────────────────
builder.Services.AddScoped<TokenService>();

// ── 8. Controllers & Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RecipeCQRS API", Version = "v1" });

    // Allow JWT in Swagger UI
    c.AddSecurityDefinition("Bearer", new()
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Enter your JWT token here"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware pipeline — ExceptionMiddleware MUST be first ───────────────────
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

// Expose for integration tests
public partial class Program { }
