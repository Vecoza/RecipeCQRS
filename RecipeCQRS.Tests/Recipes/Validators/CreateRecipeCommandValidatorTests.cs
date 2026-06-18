using FluentValidation.TestHelper;
using RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;

namespace RecipeCQRS.Tests.Recipes.Validators;

public class CreateRecipeCommandValidatorTests
{
    private readonly CreateRecipeCommandValidator _validator = new();

    private static CreateRecipeCommand ValidCommand() => new()
    {
        UserId      = "user-1",
        Title       = "Pasta Carbonara",
        Servings    = 4,
        PrepTime    = 10,
        CookTime    = 20,
        Ingredients = [new IngredientDto { Name = "Pasta", Quantity = 200, Unit = "g" }],
        Steps       = ["Boil pasta"]
    };

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyTitle_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { Title = "" });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_TitleExceeds200Chars_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { Title = new string('a', 201) });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validate_ZeroServings_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { Servings = 0 });
        result.ShouldHaveValidationErrorFor(x => x.Servings);
    }

    [Fact]
    public void Validate_NegativePrepTime_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { PrepTime = -1 });
        result.ShouldHaveValidationErrorFor(x => x.PrepTime);
    }

    [Fact]
    public void Validate_EmptyIngredientsList_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { Ingredients = [] });
        result.ShouldHaveValidationErrorFor(x => x.Ingredients);
    }

    [Fact]
    public void Validate_IngredientWithEmptyName_FailsValidation()
    {
        var cmd = ValidCommand() with
        {
            Ingredients = [new IngredientDto { Name = "", Quantity = 100 }]
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_IngredientWithZeroQuantity_FailsValidation()
    {
        var cmd = ValidCommand() with
        {
            Ingredients = [new IngredientDto { Name = "Pasta", Quantity = 0 }]
        };

        var result = _validator.TestValidate(cmd);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Validate_EmptyStepsList_FailsValidation()
    {
        var result = _validator.TestValidate(ValidCommand() with { Steps = [] });
        result.ShouldHaveValidationErrorFor(x => x.Steps);
    }
}
