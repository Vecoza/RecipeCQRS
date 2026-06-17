using FluentValidation;

namespace RecipeCQRS.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Servings must be at least 1");

        RuleFor(x => x.PrepTime)
            .GreaterThanOrEqualTo(0).WithMessage("Prep time cannot be negative");

        RuleFor(x => x.CookTime)
            .GreaterThanOrEqualTo(0).WithMessage("Cook time cannot be negative");

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("At least one ingredient is required");

        RuleForEach(x => x.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(i => i.Name)
                .NotEmpty().WithMessage("Ingredient name is required");

            ingredient.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");
        });

        RuleFor(x => x.Steps)
            .NotEmpty().WithMessage("At least one step is required");
    }
}
