# CQRS Demo API

A Recipe Manager API rebuilt with CQRS + MediatR to demonstrate real enterprise .NET architecture.

![CI](https://github.com/YOUR_USERNAME/RecipeCQRS/actions/workflows/ci.yml/badge.svg)

## Tech Stack

| Package | Purpose |
|---|---|
| ASP.NET Core 8 | Web API framework |
| MediatR 12 | In-process message broker for CQRS |
| FluentValidation | Declarative validation in dedicated classes |
| EF Core 8 + Npgsql | ORM with PostgreSQL |
| Serilog | Structured logging |
| xUnit | Unit testing |

## Local Setup

```bash
git clone https://github.com/YOUR_USERNAME/RecipeCQRS.git
cd RecipeCQRS

# Add your Neon.tech connection string to User Secrets
cd RecipeCQRS.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "YOUR_NEON_CONNECTION_STRING"
dotnet user-secrets set "JwtSettings:Secret" "YOUR_JWT_SECRET_MIN_32_CHARS"

# Run migrations
dotnet ef migrations add Initial --project ../RecipeCQRS.Infrastructure --startup-project .
dotnet ef database update --project ../RecipeCQRS.Infrastructure --startup-project .

# Start the API
dotnet run
# Swagger UI: https://localhost:5001/swagger
```

## Running Tests

```bash
dotnet test --verbosity normal
```

## Architecture

> README to be expanded on Day 7 with full architecture diagram and CQRS explanation.
