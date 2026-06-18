using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;

namespace RecipeCQRS.Application.Features.Tags.Queries.GetTagsList;

public class GetTagsListQueryHandler : IRequestHandler<GetTagsListQuery, List<string>>
{
    private readonly IAppDbContext _db;

    public GetTagsListQueryHandler(IAppDbContext db) => _db = db;

    public async Task<List<string>> Handle(GetTagsListQuery request, CancellationToken cancellationToken)
    {
        return await _db.Recipes
            .Where(r => r.UserId == request.UserId)
            .SelectMany(r => r.Tags)
            .Select(t => t.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}
