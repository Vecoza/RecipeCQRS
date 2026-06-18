using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeCQRS.Application.Features.Tags.Queries.GetTagsList;

namespace RecipeCQRS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetTagsListQuery
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
        });

        return Ok(result);
    }
}
