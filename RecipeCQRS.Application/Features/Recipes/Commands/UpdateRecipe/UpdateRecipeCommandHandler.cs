using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;
using RecipeCQRS.Application.Entities;
using RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;

namespace RecipeCQRS.Application.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand>
{
    private readonly IAppDbContext _db;

    public UpdateRecipeCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (recipe is null)
            throw new KeyNotFoundException($"Recipe {request.Id} not found.");

        if (recipe.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to update this recipe.");

        recipe.Title       = request.Title;
        recipe.Description = request.Description;
        recipe.ImageUrl    = request.ImageUrl;
        recipe.PrepTime    = request.PrepTime;
        recipe.CookTime    = request.CookTime;
        recipe.Servings    = request.Servings;
        recipe.UpdatedAt   = DateTime.UtcNow;

        recipe.Ingredients.Clear();
        recipe.Steps.Clear();
        recipe.Tags.Clear();

        foreach (var tagName in request.Tags)
        {
            var tag = await _db.Tags
                          .FirstOrDefaultAsync(t => t.Name == tagName, cancellationToken)
                      ?? new Tag { Name = tagName };
            recipe.Tags.Add(tag);
        }

        foreach (var (item, idx) in request.Ingredients.Select((i, idx) => (i, idx)))
        {
            recipe.Ingredients.Add(new Ingredient
            {
                Id        = Guid.NewGuid(),
                Name      = item.Name,
                Quantity  = item.Quantity,
                Unit      = item.Unit,
                SortOrder = idx
            });
        }

        foreach (var (instruction, idx) in request.Steps.Select((s, idx) => (s, idx)))
        {
            recipe.Steps.Add(new Step
            {
                Id          = Guid.NewGuid(),
                Instruction = instruction,
                StepNumber  = idx + 1
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
