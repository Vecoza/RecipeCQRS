namespace RecipeCQRS.Application.Features.Recipes.Queries.GetRecipesList;

public class RecipeSummaryDto
{
    public Guid         Id          { get; set; }
    public string       Title       { get; set; } = string.Empty;
    public string?      Description { get; set; }
    public int          PrepTime    { get; set; }
    public int          CookTime    { get; set; }
    public int          Servings    { get; set; }
    public DateTime     CreatedAt   { get; set; }
    public List<string> Tags        { get; set; } = [];
}
