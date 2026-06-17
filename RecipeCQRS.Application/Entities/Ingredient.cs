namespace RecipeCQRS.Application.Entities;

public class Ingredient
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }
    public int SortOrder { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
