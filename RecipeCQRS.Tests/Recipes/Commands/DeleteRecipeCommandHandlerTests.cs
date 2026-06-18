using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Entities;
using RecipeCQRS.Application.Features.Recipes.Commands.DeleteRecipe;
using RecipeCQRS.Infrastructure.Data;

namespace RecipeCQRS.Tests.Recipes.Commands;

public class DeleteRecipeCommandHandlerTests
{
    private readonly AppDbContext _db;
    private readonly DeleteRecipeCommandHandler _handler;

    public DeleteRecipeCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db      = new AppDbContext(options);
        _handler = new DeleteRecipeCommandHandler(_db);
    }

    private async Task<Recipe> SeedRecipe(string userId = "user-1")
    {
        var recipe = new Recipe
        {
            Id        = Guid.NewGuid(),
            UserId    = userId,
            Title     = "Test Recipe",
            Servings  = 2,
            CreatedAt = DateTime.UtcNow
        };

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        return recipe;
    }

    [Fact]
    public async Task Handle_OwnerDeletes_RemovesRecipeFromDatabase()
    {
        var recipe = await SeedRecipe("user-1");

        await _handler.Handle(new DeleteRecipeCommand { Id = recipe.Id, UserId = "user-1" }, CancellationToken.None);

        Assert.Equal(0, await _db.Recipes.CountAsync());
    }

    [Fact]
    public async Task Handle_NonOwnerDeletes_ThrowsUnauthorizedAccessException()
    {
        var recipe = await SeedRecipe("user-1");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new DeleteRecipeCommand { Id = recipe.Id, UserId = "user-999" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistentRecipe_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeleteRecipeCommand { Id = Guid.NewGuid(), UserId = "user-1" }, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonOwnerDeletes_RecipeRemainsInDatabase()
    {
        var recipe = await SeedRecipe("user-1");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(new DeleteRecipeCommand { Id = recipe.Id, UserId = "user-999" }, CancellationToken.None));

        Assert.Equal(1, await _db.Recipes.CountAsync());
    }
}
