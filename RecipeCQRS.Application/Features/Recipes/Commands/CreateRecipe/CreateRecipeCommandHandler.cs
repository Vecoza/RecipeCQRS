using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;
using RecipeCQRS.Application.Entities;

namespace RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Guid>
{
    private readonly IAppDbContext _db;

    public CreateRecipeCommandHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();
        foreach (var tagName in request.Tags)
        {
            var tag = await _db.Tags
                          .FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken)
                      ?? new Tag { Name = tagName };
            tags.Add(tag);
        }

        var recipe = new Recipe
        {
            Id          = Guid.NewGuid(),
            UserId      = request.UserId,
            Title       = request.Title,
            Description = request.Description,
            ImageUrl    = request.ImageUrl,
            PrepTime    = request.PrepTime,
            CookTime    = request.CookTime,
            Servings    = request.Servings,
            CreatedAt   = DateTime.UtcNow,
            Tags        = tags,
            Ingredients = request.Ingredients.Select((i, idx) => new Ingredient
            {
                Id        = Guid.NewGuid(),
                Name      = i.Name,
                Quantity  = i.Quantity,
                Unit      = i.Unit,
                SortOrder = idx
            }).ToList(),
            Steps = request.Steps.Select((s, idx) => new Step
            {
                Id          = Guid.NewGuid(),
                Instruction = s,
                StepNumber  = idx + 1
            }).ToList()
        };

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}
