using Microsoft.AspNetCore.Identity;

namespace RecipeCQRS.Infrastructure.Entities;

public class AppUser : IdentityUser
{
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
