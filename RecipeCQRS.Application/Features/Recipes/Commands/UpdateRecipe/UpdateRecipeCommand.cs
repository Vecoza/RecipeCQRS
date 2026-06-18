using MediatR;
using RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;

namespace RecipeCQRS.Application.Features.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand : IRequest
{
    public Guid    Id          { get; init; }
    public string  UserId      { get; init; } = string.Empty;
    public string  Title       { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ImageUrl    { get; init; }
    public int     PrepTime    { get; init; }
    public int     CookTime    { get; init; }
    public int     Servings    { get; init; }

    public List<IngredientDto> Ingredients { get; init; } = [];
    public List<string>        Steps       { get; init; } = [];
    public List<string>        Tags        { get; init; } = [];
}
