using MediatR;

namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipeById;

public record GetRecipeByIdQuery : IRequest<RecipeDetailDto?>
{
    public Guid   Id     { get; init; }
    public string UserId { get; init; } = string.Empty;
}
