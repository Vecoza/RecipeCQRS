using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;
using RecipeCQRS.Infrastructure.Data;

namespace RecipeCQRS.Tests.Recipes.Commands;

public class CreateRecipeCommandHandlerTests
{
    private readonly AppDbContext _db;
    private readonly CreateRecipeCommandHandler _handler;

    public CreateRecipeCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db      = new AppDbContext(options);
        _handler = new CreateRecipeCommandHandler(_db);
    }

    private static CreateRecipeCommand ValidCommand(string userId = "user-1") => new()
    {
        UserId      = userId,
        Title       = "Pasta Carbonara",
        Servings    = 4,
        PrepTime    = 10,
        CookTime    = 20,
        Ingredients = [new IngredientDto { Name = "Pasta", Quantity = 200, Unit = "g" }],
        Steps       = ["Boil pasta", "Make sauce", "Combine"]
    };

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNonEmptyGuid()
    {
        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesRecipeToDatabase()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.Equal(1, await _db.Recipes.CountAsync());
    }

    [Fact]
    public async Task Handle_ValidCommand_AssignsCorrectUserId()
    {
        await _handler.Handle(ValidCommand("user-abc"), CancellationToken.None);

        var saved = await _db.Recipes.FirstAsync();
        Assert.Equal("user-abc", saved.UserId);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesIngredientsWithCorrectSortOrder()
    {
        var cmd = ValidCommand() with
        {
            Ingredients =
            [
                new IngredientDto { Name = "Pasta",  Quantity = 200, Unit = "g" },
                new IngredientDto { Name = "Eggs",   Quantity = 3 },
                new IngredientDto { Name = "Cheese", Quantity = 50, Unit = "g" }
            ]
        };

        await _handler.Handle(cmd, CancellationToken.None);

        var ingredients = _db.Ingredients.OrderBy(i => i.SortOrder).ToList();
        Assert.Equal(3, ingredients.Count);
        Assert.Equal(0, ingredients[0].SortOrder);
        Assert.Equal(1, ingredients[1].SortOrder);
        Assert.Equal(2, ingredients[2].SortOrder);
    }

    [Fact]
    public async Task Handle_NewTag_CreatesTagInDatabase()
    {
        var cmd = ValidCommand() with { Tags = ["Italian"] };

        await _handler.Handle(cmd, CancellationToken.None);

        Assert.Equal(1, await _db.Tags.CountAsync());
        Assert.Equal("Italian", (await _db.Tags.FirstAsync()).Name);
    }

    [Fact]
    public async Task Handle_ExistingTag_ReusesTagWithoutDuplicate()
    {
        var cmd = ValidCommand() with { Tags = ["Italian"] };

        await _handler.Handle(cmd, CancellationToken.None);
        await _handler.Handle(cmd with { Title = "Pizza" }, CancellationToken.None);

        Assert.Equal(1, await _db.Tags.CountAsync());
    }
}
