using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Entities;
using RecipeCQRS.Application.Features.Recipes.Queries.GetRecipesList;
using RecipeCQRS.Infrastructure.Data;

namespace RecipeCQRS.Tests.Recipes.Queries;

public class GetRecipesListQueryHandlerTests
{
    private readonly AppDbContext _db;
    private readonly GetRecipesListQueryHandler _handler;

    public GetRecipesListQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db      = new AppDbContext(options);
        _handler = new GetRecipesListQueryHandler(_db);
    }

    private async Task SeedRecipes()
    {
        var italian = new Tag { Name = "Italian" };
        var asian   = new Tag { Name = "Asian" };

        _db.Recipes.AddRange(
            new Recipe
            {
                Id        = Guid.NewGuid(),
                UserId    = "user-1",
                Title     = "Pasta Carbonara",
                Servings  = 4,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Tags      = [italian]
            },
            new Recipe
            {
                Id        = Guid.NewGuid(),
                UserId    = "user-1",
                Title     = "Chicken Stir Fry",
                Servings  = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Tags      = [asian]
            },
            new Recipe
            {
                Id        = Guid.NewGuid(),
                UserId    = "user-2",
                Title     = "Pizza Margherita",
                Servings  = 2,
                CreatedAt = DateTime.UtcNow,
                Tags      = [italian]
            }
        );

        await _db.SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_ReturnsOnlyCurrentUsersRecipes()
    {
        await SeedRecipes();

        var result = await _handler.Handle(new GetRecipesListQuery { UserId = "user-1" }, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, r => Assert.DoesNotContain("Pizza", r.Title));
    }

    [Fact]
    public async Task Handle_SearchFilter_ReturnMatchingRecipes()
    {
        await SeedRecipes();

        var result = await _handler.Handle(
            new GetRecipesListQuery { UserId = "user-1", Search = "pasta" }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Pasta Carbonara", result[0].Title);
    }

    [Fact]
    public async Task Handle_TagFilter_ReturnsMatchingRecipes()
    {
        await SeedRecipes();

        var result = await _handler.Handle(
            new GetRecipesListQuery { UserId = "user-1", Tags = "Asian" }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Chicken Stir Fry", result[0].Title);
    }

    [Fact]
    public async Task Handle_NoRecipes_ReturnsEmptyList()
    {
        var result = await _handler.Handle(
            new GetRecipesListQuery { UserId = "user-with-no-recipes" }, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ReturnsRecipesOrderedByCreatedAtDescending()
    {
        await SeedRecipes();

        var result = await _handler.Handle(new GetRecipesListQuery { UserId = "user-1" }, CancellationToken.None);

        Assert.True(result[0].CreatedAt > result[1].CreatedAt);
    }
}
