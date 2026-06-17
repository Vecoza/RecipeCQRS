using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;
using RecipeCQRS.Application.Entities;

namespace RecipeCQRS.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe>     Recipes     => Set<Recipe>();
    public DbSet<Tag>        Tags        => Set<Tag>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Step>       Steps       => Set<Step>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Recipe>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.HasOne(r => r.User)
                  .WithMany(u => u.Recipes)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Many-to-many: Recipe <-> Tag
            entity.HasMany(r => r.Tags)
                  .WithMany(t => t.Recipes)
                  .UsingEntity(j => j.ToTable("RecipeTag"));

            // Cascade delete ingredients and steps when recipe is deleted
            entity.HasMany(r => r.Ingredients)
                  .WithOne(i => i.Recipe)
                  .HasForeignKey(i => i.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Steps)
                  .WithOne(s => s.Recipe)
                  .HasForeignKey(s => s.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.Name).IsUnique();
        });
    }
}
