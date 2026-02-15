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

## Lint Commands

### Frontend
```bash
cd OnTime.Client
npm run lint          # Run ESLint
```

### Backend
No explicit linting configured. Follow C# conventions below.

## Test Commands

**No test framework is currently configured.**

When adding tests:
- Backend: Use `dotnet test` (xUnit/NUnit)
- Frontend: Consider Vitest or Jest

## Code Style Guidelines

### C# (Backend)

#### Naming Conventions
- **PascalCase**: Classes, methods, properties, public fields
- **camelCase**: Local variables, private fields
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
- Use primary constructor syntax for dependency injection
- Inherit from `BaseApiController` for API controllers
- Use `[Authorize]` attribute on controllers requiring authentication

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
- **Controllers**: Handle HTTP requests, inherit from `BaseApiController`
- **Services**: Business logic, implement interfaces, registered as scoped
- **Models/Domain**: Entity classes with inheritance from `BaseEntity`
- **Models/Requests**: Record types for input validation
- **Models/Responses**: Record types for API output
- **Database**: EF Core with PostgreSQL, use `AppDbContext`
- **Extensions**: Extension methods for model mapping

### Frontend Architecture
- Standard React 19 + Vite setup
- Components in `src/` directory
- Assets in `src/assets/`

### Database
- Use EF Core migrations: `dotnet ef migrations add <Name>`
- PostgreSQL provider configured
- Use `AsNoTracking()` for read-only queries

## Key Technologies

- **Backend**: ASP.NET Core 9.0, EF Core, PostgreSQL, Identity, Scalar (OpenAPI)
- **Frontend**: React 19, TypeScript 5.8, Vite, ESLint
- **Authentication**: ASP.NET Core Identity with JWT

## Important Notes

- API uses `Scalar` for API documentation in development
- Controllers use policy-based authorization (`RequireServiceProvider`)
- Frontend runs on Vite dev server (default port 5173)
- Backend API runs on HTTPS (default port varies)
