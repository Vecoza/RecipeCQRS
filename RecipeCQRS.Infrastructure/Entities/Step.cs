namespace RecipeCQRS.Infrastructure.Entities;

public class Step
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public int StepNumber { get; set; }

    public Recipe Recipe { get; set; } = null!;
}
