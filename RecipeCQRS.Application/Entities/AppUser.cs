using Microsoft.AspNetCore.Identity;

namespace RecipeCQRS.Application.Entities;

public class AppUser : IdentityUser
{
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
