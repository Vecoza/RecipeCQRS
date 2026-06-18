using MediatR;

namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipesList;

public record GetRecipesListQuery : IRequest<List<RecipeSummaryDto>>
{
    public string  UserId { get; init; } = string.Empty;
    public string? Search { get; init; }
    public string? Tags   { get; init; }
}
