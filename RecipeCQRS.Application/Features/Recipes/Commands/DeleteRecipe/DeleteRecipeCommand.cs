using MediatR;

namespace RecipeCQRS.Application.Features.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand : IRequest
{
    public Guid   Id     { get; init; }
    public string UserId { get; init; } = string.Empty;
}
