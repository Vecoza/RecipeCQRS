using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Common.Interfaces;

namespace RecipeCQRS.Application.Features.Recipes.Commands.DeleteRecipe;

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand>
{
    private readonly IAppDbContext _db;

    public DeleteRecipeCommandHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await _db.Recipes
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (recipe is null)
            throw new KeyNotFoundException($"Recipe {request.Id} not found.");

        if (recipe.UserId != request.UserId)
            throw new UnauthorizedAccessException("You do not have permission to delete this recipe.");

        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
