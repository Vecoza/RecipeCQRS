using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeCQRS.Application.Entities;
using RecipeCQRS.Infrastructure.Services;

namespace RecipeCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser>   _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly TokenService           _tokenService;

    public AuthController(
        UserManager<AppUser>   userManager,
        SignInManager<AppUser> signInManager,
        TokenService           tokenService)
    {
        _userManager   = userManager;
        _signInManager = signInManager;
        _tokenService  = tokenService;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new AppUser
        {
            UserName = request.Username,
            Email    = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return CreatedAtAction(nameof(Register), new { id = user.Id });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized(new { error = "Invalid email or password" });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return Unauthorized(new { error = "Invalid email or password" });

        var token = _tokenService.CreateToken(user);
        return Ok(new { token });
    }
}

public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);
