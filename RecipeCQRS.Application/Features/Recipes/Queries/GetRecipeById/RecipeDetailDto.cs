namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipeById;

public class RecipeDetailDto
{
    public Guid                    Id          { get; set; }
    public string                  Title       { get; set; } = string.Empty;
    public string?                 Description { get; set; }
    public string?                 ImageUrl    { get; set; }
    public int                     PrepTime    { get; set; }
    public int                     CookTime    { get; set; }
    public int                     Servings    { get; set; }
    public DateTime                CreatedAt   { get; set; }
    public DateTime?               UpdatedAt   { get; set; }
    public List<string>            Tags        { get; set; } = [];
    public List<IngredientDetailDto> Ingredients { get; set; } = [];
    public List<StepDetailDto>     Steps       { get; set; } = [];
}

public class IngredientDetailDto
{
    public string  Name     { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit     { get; set; }
    public int     SortOrder { get; set; }
}

public class StepDetailDto
{
    public int    StepNumber  { get; set; }
    public string Instruction { get; set; } = string.Empty;
}
