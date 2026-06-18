using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;

namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipeById;

public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDetailDto?>
{
    private readonly IAppDbContext _db;

    public GetRecipeByIdQueryHandler(IAppDbContext db) => _db = db;

    public async Task<RecipeDetailDto?> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
    {
        return await _db.Recipes
            .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
            .Include(r => r.Steps.OrderBy(s => s.StepNumber))
            .Include(r => r.Tags)
            .Where(r => r.Id == request.Id && r.UserId == request.UserId)
            .Select(r => new RecipeDetailDto
            {
                Id          = r.Id,
                Title       = r.Title,
                Description = r.Description,
                ImageUrl    = r.ImageUrl,
                PrepTime    = r.PrepTime,
                CookTime    = r.CookTime,
                Servings    = r.Servings,
                CreatedAt   = r.CreatedAt,
                UpdatedAt   = r.UpdatedAt,
                Tags        = r.Tags.Select(t => t.Name).ToList(),
                Ingredients = r.Ingredients
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new IngredientDetailDto
                    {
                        Name      = i.Name,
                        Quantity  = i.Quantity,
                        Unit      = i.Unit,
                        SortOrder = i.SortOrder
                    }).ToList(),
                Steps = r.Steps
                    .OrderBy(s => s.StepNumber)
                    .Select(s => new StepDetailDto
                    {
                        StepNumber  = s.StepNumber,
                        Instruction = s.Instruction
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
