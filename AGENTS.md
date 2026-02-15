# AGENTS.md - OnTime Project Guidelines

## Project Structure

This is a full-stack appointment scheduling application with:

- **OnTime.API**: ASP.NET Core 9.0 backend API
- **OnTime.Client**: React 19 + TypeScript + Vite frontend

## Build Commands

### Backend (OnTime.API)

```bash
cd OnTime.API
# Build
dotnet build

# Run
dotnet run

# Watch mode (auto-restart on changes)
dotnet watch

# EF Core migrations
dotnet ef migrations add <Name>
dotnet ef database update
```

### Frontend (OnTime.Client)

```bash
cd OnTime.Client
# Install dependencies
npm install

# Development server
npm run dev

# Build
npm run build

# Preview production build
npm run preview
```

### Docker (PostgreSQL)

```bash
# Start PostgreSQL container
docker compose up -d

# Stop container
docker compose down
```

## Lint Commands

### Frontend

```bash
cd OnTime.Client
npm run lint          # Run ESLint
```

### Backend

No explicit linting configured. Follow C# conventions in `.editorconfig`.

## Test Commands

**No test framework is currently configured.**

When adding tests:

- Backend: Use `dotnet test` (xUnit/NUnit)
- Frontend: Consider Vitest or Jest

## Code Style Guidelines

### C# (Backend)

#### Naming Conventions (enforced by .editorconfig)

- **PascalCase**: Classes, methods, properties, public fields
- **camelCase**: Local variables, parameters
- **PascalCase**: Interfaces with `I` prefix (e.g., `IAppointmentService`)
- **PascalCase**: File-scoped namespaces

#### Imports

- Group using statements: System, Third-party, Project
- Use file-scoped namespaces: `namespace OnTime.API.Models.Domain;`
- Implicit usings enabled (see `.csproj`)

#### Types

- Use `record` for DTOs/requests/responses
- Use `required` modifier for non-nullable properties
- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Use `var` when type is obvious from right-hand side

#### Classes

- Use primary constructor syntax for dependency injection (preferred)

#### Error Handling

- Return `ActionResult<T>` from controllers
- Use `Results.Problem()` for server errors
- Use `Unauthorized()` for auth failures
- Log warnings with structured logging: `logger.LogWarning("Message: {Param}", value)`

### TypeScript/React (Frontend)

#### Naming Conventions

- **PascalCase**: Components, types, interfaces
- **camelCase**: Functions, variables, hooks
- **PascalCase**: File names for components

#### Imports

- Use ES modules (`"type": "module"`)
- Group: React, third-party, local imports
- Use `.tsx` extension for React components
- Import types explicitly when needed

#### Types

- Enable strict mode in tsconfig
- Use explicit return types for functions
- Prefer `interface` over `type` for object shapes
- Use `const` assertions for literal values

#### React

- Use functional components with hooks
- Prefer `useState` and `useEffect` from React 19
- Use `StrictMode` in development

#### Formatting

- 2-space indentation
- Single quotes for strings
- Semicolons required
- Trailing commas in multi-line objects/arrays

## Project Conventions

### Backend Architecture

#### Controllers

- Handle HTTP requests, inherit from `BaseApiController`
- Use primary constructor for dependency injection
- Return `ActionResult<T>` or `IResult` from actions
- Apply `[Authorize]` for protected endpoints, `[AllowAnonymous]` for public

#### Services

- Business logic in `/Services/{Feature}/` folders
- Implement interfaces (e.g., `ISessionService`)
- Register as scoped in `Program.cs`: `builder.Services.AddScoped<IAppointmentService, AppointmentService>();`
- Inject `ILogger<T>` and `AppDbContext` via constructor
- Return nullable types or sentinel values for failures

#### Models/Domain

- Entity classes inherit from `BaseEntity`:
  ```csharp
  public abstract class BaseEntity
  {
      public DateTime CreatedAt { get; set; }
      public DateTime UpdatedAt { get; set; }
  }
  ```
- `User` extends `IdentityUser` with additional properties
- Use `required` modifier for non-nullable properties

#### Models/Requests

- Record types with validation attributes (`[Required]`, `[Range]`, `[EmailAddress]`, `[Phone]`)

#### Models/Responses

- Record types for API output
- Flat structures preferred

#### Database

- EF Core with PostgreSQL via `AppDbContext`
- `AppDbContext` extends `IdentityDbContext<User>`
- Automatic timestamp management via overridden `SaveChanges` methods
- Entity configurations in separate classes implementing `IEntityTypeConfiguration<T>`
- Configurations auto-discovered: `builder.ApplyConfigurationsFromAssembly()`
- Use `AsNoTracking()` for read-only queries
- PostgreSQL identity columns via `UseIdentityByDefaultColumns()`
- Snake_case for join table names (e.g., `AppointmentSession`)

#### Extensions

- Extension methods for model mapping (e.g., `SessionExtensions.ToResponse()`)
- Update helpers for applying request changes to domain objects

### Frontend Architecture

#### Technology Stack

- **React 19** with TypeScript 5.8
- **Vite 7** for build tooling
- **Mantine 8** for UI components (core, dates, hooks, modals, notifications)
- **TanStack Router** for routing
- **TanStack Query** for data fetching
- **OpenAPI Fetch** for type-safe API clients
- **Dayjs** for date manipulation
- **Embla Carousel** for carousels

#### Project Structure

- Components in `src/` directory
- Assets in `src/assets/`
- Entry point: `main.tsx` with `StrictMode`
- Root component: `App.tsx` with `MantineProvider`

### Authentication & Authorization

- ASP.NET Core Identity with JWT
- Policy-based authorization: `RequireServiceProvider` role
- Identity configured in `Program.cs` with JWT Bearer tokens

### API Documentation

- **Scalar** used for API documentation in development
- OpenAPI/Swagger available at `/scalar/v1`

## Key Technologies

- **Backend**: ASP.NET Core 9.0, EF Core 9, PostgreSQL 17, Identity, Scalar (OpenAPI)
- **Frontend**: React 19, TypeScript 5.8, Vite 7, ESLint, Mantine UI
- **Authentication**: ASP.NET Core Identity with JWT Bearer tokens
- **Database**: PostgreSQL (Docker container via compose.yaml)

## Important Notes

- API runs on HTTP port: 3000
- Frontend runs on Vite dev server port: 3001
- PostgreSQL container exposes port 5432
- `AppDbContext` manages `CreatedAt` and `UpdatedAt` timestamps automatically
- All entity configurations are auto-discovered from assembly
- Frontend uses OpenAPI-generated clients for type-safe API communication
- C# naming conventions are enforced via `.editorconfig` (private fields use `_camelCase`)
