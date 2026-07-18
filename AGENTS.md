# Agent & Developer Guide (AGENTS.md)

## 📁 Directory Structure
```text
ontime/
├── backend/                       # Backend Solution Folder (.NET 10)
│   ├── OnTime.slnx                # XML-based dotnet solution file
│   ├── HttpRequests/              # Local HTTP endpoint requests (.http files)
│   ├── Directory.Build.props      # Central build configurations
│   ├── Directory.Packages.props   # Centrally managed package versions
│   ├── OnTime.Domain/             # Pure Domain Layer (Entities, Common, Enums)
│   ├── OnTime.Application/        # Application Core (CQRS, MediatR, interfaces)
│   ├── OnTime.Infrastructure/     # Infrastructure (EF Core AppDbContext, files, bus implementation)
│   ├── OnTime.Bus/                # Channel-based lightweight event broker wrapper
│   └── OnTime.Api/                # Web API (Controllers, Swagger, DI, Program)
├── frontend/                      # Frontend Application Folder (React, Vite, TypeScript)
│   ├── src/
│   │   ├── assets/                # Static assets, variable fonts
│   │   ├── components/            # UI components (Shadcn UI under components/ui)
│   │   ├── lib/                   # Utility helpers (utils.ts)
│   │   ├── routes/                # TanStack Router file-based pages
│   │   ├── generated/             # Auto-generated API client types (apiClient.ts)
│   │   ├── App.tsx                # Main App Router component
│   │   ├── router.ts              # TanStack router setup
│   │   └── main.tsx               # App mount point
│   ├── vite.config.ts             # Vite build configuration
│   └── tsconfig.json              # TypeScript compilation setup
├── infra/                         # DevOps and Local Infrastructure
│   ├── compose.yaml               # Docker Compose (Postgres, Keycloak, Mailpit, CDN)
│   └── nginx.conf                 # CDN reverse proxy configuration
└── AGENTS.md                      # This file
```

---

## 🖥️ Backend Guidelines & Commands

### Code Style
- **Pattern**: Clean Architecture, CQRS with MediatR, and Result pattern.
- **Controllers**: All API controllers must inherit from `BaseApiController`.
- **Handlers**: Must inherit from `BaseHandler<TRequest, TResponse>` and implement `HandleSafe`.
- **Instance Scope**: Always use `this.` explicitly for instance variables.
- **Packages**: Managed centrally in `backend/Directory.Packages.props`.
- **Options Pattern**: Prefer the Options pattern via `appsettings.json` to host configuration. End the classes with the `Settings` suffix instead of `-Options` (e.g., `AuthenticationSettings`, not `AuthenticationOptions`).
- **Constants**: Avoid hardcoded strings; always prefer placing them in static classes organized by concern.
- **Guard Clauses**: Write clean code with guard clauses, keeping it short and concise (Frontend and backend).
- **HTTP Request Files**: When changing the API project, create or update a `.http` file inside `backend/HttpRequests/` (1 per controller) with sample requests. Use variables for host and authentication token.
- **CDN / Media URLs**: Never store dynamic absolute URLs (such as CDN image paths) in the database. Construct them dynamically from configuration values via extensions or helper services.
- **Build & Quality**: Fix any errors and warnings reported when running build or format commands.

### CLI Commands (from root)
- **Build**: `dotnet build OnTime.slnx`
- **Format**: `dotnet format OnTime.slnx`
- **Add Migration**: `dotnet ef migrations add <Name> --project backend/OnTime.Infrastructure --startup-project backend/OnTime.Api --context AppDbContext`
- **Apply Migrations**: `dotnet ef database update --project backend/OnTime.Infrastructure --startup-project backend/OnTime.Api --context AppDbContext`

---

## 🎨 Frontend Guidelines & Commands

### Code Style
- **Stack**: React 19, TypeScript, Tailwind CSS v4, TanStack Router.
- **API Calls**: Uses `openapi-fetch` and `openapi-react-query` based on generated client.
- **Constants**: Avoid hardcoded magic strings; place them in dedicated files/classes (e.g. `src/domain/constants/localStoreKeys.ts`).
- **Async/Await**: Prefer `async/await` over `.then()` chains to improve readability.
- **Code Nesting**: Keep code structure flat; try to avoid nesting callback functions or logic deeply. Use guard clauses for early exits to prevent deep nesting.
- **Component File Length**: Prefer short components. Keep component sizes small and focused. Extract nested/long sections of code into separate local components in the same file. A separate file with the component inside is only needed when the component will be re-used by another file.

  **❌ Bad (Long Component with inline sections):**
  ```tsx
  function Dashboard() {
    return (
      <div>
        <header>...</header>
        <main>
          {/* 100 lines of complex chart and tables */}
          <div className="grid">...</div>
        </main>
        <footer>...</footer>
      </div>
    )
  }
  ```

  **✔️ Good (Short Component extracting local components):**
  ```tsx
  function Dashboard() {
    return (
      <div>
        <DashboardHeader />
        <DashboardContent />
        <DashboardFooter />
      </div>
    )
  }

  function DashboardHeader() { ... }
  function DashboardContent() { ... }
  function DashboardFooter() { ... }
  ```

- **Nested Ternaries / TSX Conditions**: Avoid nested ternary conditions inside TSX return statements (e.g. `{isLoading ? (...) : isAuthenticated ? (...) : (...)}`). Instead, extract the conditions to a local sub-component using early returns (`if (isLoading) { return ... }`).
- **Build & Quality**: Fix any errors and warnings reported when running build or linting commands.

### CLI Commands (from `frontend/`)
- **Generate API Client**: `npm run generate-client`
- **Lint**: `npm run lint` (runs `oxlint`)
