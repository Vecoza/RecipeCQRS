using Microsoft.EntityFrameworkCore;
using RecipeCQRS.Application.Entities;

namespace RecipeCQRS.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Recipe>     Recipes     { get; }
    DbSet<Tag>        Tags        { get; }
    DbSet<Ingredient> Ingredients { get; }
    DbSet<Step>       Steps       { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
