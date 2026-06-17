using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;
using RecipeCQRS.Application.Features.Recipes.Commands.DeleteRecipe;

namespace RecipeCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecipesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecipesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public IActionResult GetAll() => Ok(new List<object>());

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public IActionResult GetById(Guid id) => Ok();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecipeCommand cmd)
    {
        var id = await _mediator.Send(
            cmd with { UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)! });

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteRecipeCommand
        {
            Id     = id,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
        });

        return NoContent();
    }
}
