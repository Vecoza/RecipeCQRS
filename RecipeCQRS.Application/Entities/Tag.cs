namespace RecipeCQRS.Application.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
