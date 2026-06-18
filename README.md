# RecipeCQRS

A Recipe Manager API built with ASP.NET Core, demonstrating a CQRS (Command Query Responsibility Segregation) architecture using MediatR. Each use case (create a recipe, list recipes, delete a recipe, etc.) is its own self-contained command/query + handler + validator, instead of one large "RecipeService" class.

![CI](https://github.com/YOUR_USERNAME/RecipeCQRS/actions/workflows/ci.yml/badge.svg)

## What it does

The API lets a registered user:

- Register and log in (JWT-based authentication)
- Create, list, view, update, and delete their own recipes
- Attach ingredients, step-by-step instructions, and tags to a recipe
- Filter their recipe list by search text or tag
- List all tags they have used

Recipes are private to the user who created them — every query and command is scoped by the caller's user id, taken from the JWT, never from the request body.

## Architecture

The solution is split into four projects, following Clean Architecture boundaries:

```
RecipeCQRS.API             ASP.NET Core host: controllers, JWT auth, Swagger, middleware
RecipeCQRS.Application      Use cases: MediatR commands/queries, handlers, validators, DTOs, entities
RecipeCQRS.Infrastructure    EF Core DbContext, migrations, Identity stores, JWT token service
RecipeCQRS.Tests             xUnit tests for handlers and validators (EF Core InMemory provider)
```

**Request flow (CQRS via MediatR):**

```
Controller → IMediator.Send(Command/Query)
           → LoggingBehavior      (logs request/response)
           → ValidationBehavior   (runs FluentValidation, throws ValidationException on failure)
           → Handler              (talks to AppDbContext, returns a DTO or void)
```

Pipeline behaviors are registered in `Program.cs` in a fixed order: logging wraps validation, so every request is logged even if it fails validation.

Each feature lives under `RecipeCQRS.Application/Features/<Aggregate>/<Commands|Queries>/<UseCase>/`, e.g.:

```
Features/Recipes/Commands/CreateRecipe/
  CreateRecipeCommand.cs          the input (implements IRequest<T>)
  CreateRecipeCommandHandler.cs   the logic
  CreateRecipeCommandValidator.cs FluentValidation rules
```

### Data model

| Entity | Notes |
|---|---|
| `AppUser` | extends ASP.NET Core Identity's `IdentityUser`; owns many `Recipe`s |
| `Recipe` | belongs to one user; has many `Ingredient`s, `Step`s, and `Tag`s (many-to-many) |
| `Ingredient` | name, quantity, unit, sort order |
| `Step` | instruction text, step number |
| `Tag` | free-text label, shared across recipes/users |

### Cross-cutting concerns

- **`ExceptionMiddleware`** — converts thrown exceptions into JSON error responses: `ValidationException` → 400, `KeyNotFoundException` → 404, `UnauthorizedAccessException` → 403, anything else → 500.
- **JWT auth** — issued by `TokenService` on login, validated on every `[Authorize]` endpoint. The user id, email, and username are embedded as claims.

## Tech Stack

| Package | Purpose |
|---|---|
| ASP.NET Core 8 | Web API framework |
| MediatR 12 | In-process mediator for CQRS commands/queries |
| FluentValidation | Declarative validation, run as a pipeline behavior |
| EF Core 8 + Npgsql | ORM targeting PostgreSQL |
| ASP.NET Core Identity | User accounts and password hashing |
| JWT Bearer Authentication | Stateless auth for the API |
| Swagger / Swashbuckle | Interactive API docs (Development environment only) |
| xUnit + EF Core InMemory | Unit tests for handlers/validators |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A PostgreSQL database (local instance, Docker container, or a hosted one like Neon.tech)

## Local Setup

```bash
git clone https://github.com/YOUR_USERNAME/RecipeCQRS.git
cd RecipeCQRS
```

### 1. Configure secrets

`RecipeCQRS.API/appsettings.json` is gitignored — create it yourself (or use `dotnet user-secrets`, recommended for anything beyond local experimentation):

```bash
cd RecipeCQRS.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Username=youruser;Password=yourpass;Database=RecipeCQRS"
dotnet user-secrets set "JwtSettings:Secret" "a-random-string-at-least-32-characters-long"
dotnet user-secrets set "JwtSettings:Issuer" "RecipeCQRS"
dotnet user-secrets set "JwtSettings:Audience" "RecipeCQRS"
dotnet user-secrets set "JwtSettings:ExpiryDays" "7"
```

Or, if you'd rather keep it in `appsettings.json` directly for local dev:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Username=youruser;Password=yourpass;Database=RecipeCQRS"
  },
  "JwtSettings": {
    "Secret": "a-random-string-at-least-32-characters-long",
    "Issuer": "RecipeCQRS",
    "Audience": "RecipeCQRS",
    "ExpiryDays": 7
  }
}
```

### 2. Apply database migrations

From `RecipeCQRS.API`:

```bash
dotnet ef migrations add Initial --project ../RecipeCQRS.Infrastructure --startup-project .
dotnet ef database update --project ../RecipeCQRS.Infrastructure --startup-project .
```

(`Initial` already exists in this repo — only run `migrations add` again if you've changed an entity and need a new migration, with a new name.)

### 3. Run the API

```bash
dotnet run
```

- API: `http://localhost:5000`
- Swagger UI (Development environment only): `http://localhost:5000/swagger`

## Using the API

All endpoints except `register`/`login` require an `Authorization: Bearer <token>` header.

### Auth

| Method | Route | Body | Description |
|---|---|---|---|
| POST | `/api/auth/register` | `{ username, email, password }` | Create an account |
| POST | `/api/auth/login` | `{ email, password }` | Returns `{ token }` (JWT) |

### Recipes

| Method | Route | Description |
|---|---|---|
| GET | `/api/recipes?search=&tags=` | List the caller's recipes, optional search/tag filter |
| GET | `/api/recipes/{id}` | Get full recipe detail (ingredients, steps, tags) |
| POST | `/api/recipes` | Create a recipe |
| PUT | `/api/recipes/{id}` | Update a recipe (must be the owner) |
| DELETE | `/api/recipes/{id}` | Delete a recipe (must be the owner) |

Example `POST /api/recipes` body:

```json
{
  "title": "Pancakes",
  "description": "Fluffy weekend pancakes",
  "imageUrl": null,
  "prepTime": 10,
  "cookTime": 15,
  "servings": 4,
  "ingredients": [
    { "name": "Flour", "quantity": 2, "unit": "cups" },
    { "name": "Egg", "quantity": 2, "unit": null }
  ],
  "steps": [
    "Mix dry ingredients",
    "Whisk in eggs and milk",
    "Cook on a griddle until golden"
  ],
  "tags": ["breakfast", "easy"]
}
```

### Tags

| Method | Route | Description |
|---|---|---|
| GET | `/api/tags` | List distinct tags used by the caller's recipes |

## Running Tests

```bash
dotnet test --verbosity normal
```

Tests cover command/query handlers (with EF Core's InMemory provider) and FluentValidation rules — no real database required.

## Project Layout

```
RecipeCQRS.sln
RecipeCQRS.API/              Controllers, Program.cs, middleware, appsettings
RecipeCQRS.Application/      Commands, queries, handlers, validators, entities, DTOs
RecipeCQRS.Infrastructure/   AppDbContext, EF Core migrations, TokenService
RecipeCQRS.Tests/            xUnit test suite
```
