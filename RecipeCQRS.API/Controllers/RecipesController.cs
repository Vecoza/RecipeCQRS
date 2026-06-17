using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecipeCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipesController : ControllerBase
{
    // Stub — will be fully implemented on Day 3
    [HttpGet]
    public IActionResult GetAll() => Ok(new List<object>());
}
