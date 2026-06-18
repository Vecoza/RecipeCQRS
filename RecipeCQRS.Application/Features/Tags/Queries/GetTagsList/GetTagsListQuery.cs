using MediatR;

namespace RecipeCQRS.Application.Features.Tags.Queries.GetTagsList;

public record GetTagsListQuery : IRequest<List<string>>
{
    public string UserId { get; init; } = string.Empty;
}
