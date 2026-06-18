using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;

namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipesList;

public class GetRecipesListQueryHandler : IRequestHandler<GetRecipesListQuery, List<RecipeSummaryDto>>
{
    private readonly IAppDbContext _db;

    public GetRecipesListQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<RecipeSummaryDto>> Handle(GetRecipesListQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Recipes
            .Include(r => r.Tags)
            .Where(r => r.UserId == request.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(r => r.Title.ToLower().Contains(request.Search.ToLower()));

        if (!string.IsNullOrWhiteSpace(request.Tags))
        {
            var tagList = request.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(r => r.Tags.Any(t => tagList.Contains(t.Name)));
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RecipeSummaryDto
            {
                Id          = r.Id,
                Title       = r.Title,
                Description = r.Description,
                PrepTime    = r.PrepTime,
                CookTime    = r.CookTime,
                Servings    = r.Servings,
                CreatedAt   = r.CreatedAt,
                Tags        = r.Tags.Select(t => t.Name).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
