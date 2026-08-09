# Agent & Developer Guide (AGENTS.md)

This document serves as the central onboarding guide and technical reference for developers and AI agents working on the **OnTime** project.

---

## 🚀 Quick Start & Onboarding Flow

### Prerequisites
- **.NET 10 SDK**
- **Node.js** (v20+) and **npm**
- **Docker** & **Docker Compose**

### Step-by-Step Local Setup

1. **Spin up Infrastructure Services**:
   From the repository root, start PostgreSQL, Mailpit (SMTP), and Nginx CDN:
   ```bash
   docker compose -f infra/compose.yaml up -d
   ```

2. **Run the Backend API**:
   Build and start the ASP.NET Core web API:
   ```bash
   dotnet build OnTime.slnx
   dotnet run --project backend/OnTime.Api
   ```
   > [!NOTE]
   > On startup, `Program.cs` automatically executes EF Core migrations for both `AppIdentityDbContext` and `AppDbContext`, and seeds default Identity roles (`Client`, `Professional`).

3. **Run the Frontend Application**:
   Navigate to `frontend/`, install dependencies, generate the API client, and start Vite:
   ```bash
   cd frontend
   npm install
   npm run generate-client   # Requires backend API running on http://localhost:3000
   npm run dev
   ```

### 🌐 Service Port Reference
| Service | Access URL / Port | Description |
| :--- | :--- | :--- |
| **Backend API** | `http://localhost:3000` / `https://localhost:3001` | ASP.NET Core Web API & Swagger UI (`/swagger`) |
| **Hangfire Dashboard** | `http://localhost:3000/hangfire` | Background job processing dashboard (Dev mode) |
| **PostgreSQL** | `localhost:5432` | Database (`user: postgres`, `password: password`, `db: ontime`) |
| **Mailpit UI** | `http://localhost:8025` | Local SMTP email testing web dashboard |
| **Mailpit SMTP** | `localhost:1025` | Local SMTP server port |
| **Nginx CDN** | `http://localhost:8080` | Local CDN serving processed images from `backend/images/optimized` |
| **Frontend Dev** | `http://localhost:5173` | React / Vite SPA frontend dev server |

---

## 📁 Directory Structure
```text
ontime/
├── backend/                       # Backend Solution Folder (.NET 10)
│   ├── OnTime.slnx                # XML-based dotnet solution file
│   ├── HttpRequests/              # Local HTTP endpoint requests (.http files)
│   ├── Directory.Build.props      # Central build configurations (TreatWarningsAsErrors=true)
│   ├── Directory.Packages.props   # Centrally managed package versions
│   ├── OnTime.Domain/             # Pure Domain Layer (Entities, Common, Enums)
│   ├── OnTime.Application/        # Application Core (CQRS, MediatR, interfaces)
│   ├── OnTime.Infrastructure/     # Infrastructure (EF Core AppDbContext, files, Hangfire jobs)
│   ├── OnTime.Identity/           # ASP.NET Core Identity (AppIdentityDbContext, auth handlers, roles)
│   └── OnTime.Api/                # Web API (Controllers, Swagger, DI, Program)
├── frontend/                      # Frontend Application Folder (React, Vite, TypeScript)
│   ├── src/
│   │   ├── assets/                # Static assets, variable fonts
│   │   ├── components/            # UI components (Shadcn UI under components/ui)
│   │   ├── domain/                # Domain models, constants (localStoreKeys.ts), auth helpers
│   │   ├── lib/                   # Utility helpers (utils.ts)
│   │   ├── routes/                # TanStack Router file-based pages
│   │   ├── generated/             # Auto-generated API client types (apiClient.ts)
│   │   ├── App.tsx                # Main App Router component
│   │   ├── router.ts              # TanStack router setup
│   │   └── main.tsx               # App mount point
│   ├── vite.config.ts             # Vite build configuration
│   └── tsconfig.json              # TypeScript compilation setup
├── docs/                          # Project specifications and architecture documentation
│   └── IDEA.md                    # Core project idea & architectural specifications
├── infra/                         # DevOps and Local Infrastructure
│   ├── compose.yaml               # Docker Compose (Postgres, Mailpit, CDN)
│   └── nginx.conf                 # CDN reverse proxy configuration
└── AGENTS.md                      # This guide
```

---

## 🖥️ Backend Guidelines & Commands

### Architecture & Patterns
- **Architecture**: Clean Architecture with strict layer isolation.
- **CQRS & Background Jobs**: CQRS implemented via MediatR; async background job processing via Hangfire.
- **Controllers**: All API controllers must inherit from `BaseApiController`, always return ErrorResponse when the return is not a an HTTP 2XX code and there is an error message to be sent.
- **Handlers**: Must inherit from `BaseHandler<TRequest, TResponse>` and implement `HandleSafe` returning `Result<TResponse>`.
- **Instance Scope**: Always use `this.` explicitly for instance variables.
- **Strict Build Quality**: `TreatWarningsAsErrors` is set to `true` in `Directory.Build.props`. All code must compile cleanly without warnings.
- **Packages**: Managed centrally in `backend/Directory.Packages.props`.
- **Options Pattern**: Prefer the Options pattern via `appsettings.json`. Configuration classes must end with `Settings` (e.g., `AuthenticationSettings`, `ImageSettings`, `EmailSettings`).
- **Guard Clauses**: Write clean code with guard clauses for early exits to prevent deep branching.
- **HTTP Request Files**: When creating or updating API endpoints, maintain `.http` files inside `backend/HttpRequests/` (1 file per controller) with sample requests using `@host` variables.
- **CDN / Media URLs**: Never store dynamic absolute URLs (such as CDN image paths) in the database. Construct them dynamically from configuration values via extensions or helper services.

### Database & EF Core Migrations
The solution utilizes **two distinct EF Core DbContexts**:
1. `AppDbContext` in `OnTime.Infrastructure` (Core domain data)
2. `AppIdentityDbContext` in `OnTime.Identity` (Authentication & user data)

#### Migration CLI Commands (run from root)

- **Add Migration for Core Domain (`AppDbContext`)**:
  ```bash
  dotnet ef migrations add <MigrationName> --project backend/OnTime.Infrastructure --startup-project backend/OnTime.Api --context AppDbContext
  ```

- **Apply Migrations for Core Domain (`AppDbContext`)**:
  ```bash
  dotnet ef database update --project backend/OnTime.Infrastructure --startup-project backend/OnTime.Api --context AppDbContext
  ```

- **Add Migration for Identity (`AppIdentityDbContext`)**:
  ```bash
  dotnet ef migrations add <MigrationName> --project backend/OnTime.Identity --startup-project backend/OnTime.Api --context AppIdentityDbContext
  ```

- **Apply Migrations for Identity (`AppIdentityDbContext`)**:
  ```bash
  dotnet ef database update --project backend/OnTime.Identity --startup-project backend/OnTime.Api --context AppIdentityDbContext
  ```

### CLI Commands (from root)
- **Build Solution**: `dotnet build OnTime.slnx`
- **Format Code**: `dotnet format OnTime.slnx`

---

## 🎨 Frontend Guidelines & Commands

### Tech Stack & Architecture
- **Core**: React 19, TypeScript, Tailwind CSS v4, TanStack Router (file-based routing), TanStack React Query.
- **API Client**: Generated using `openapi-typescript`, consumed via `openapi-fetch` and `openapi-react-query`.

### Code Style & Component Structure
- **Typed API Client Generation**:
  Run the client generator script whenever backend OpenAPI endpoints change:
  ```bash
  npm run generate-client
  ```
  *(Generates `src/generated/apiClient.ts` by fetching OpenAPI specs from `http://localhost:3000/swagger/v1/swagger.json`)*

- **Constants**: Avoid hardcoded magic strings; place them in dedicated files (e.g., `src/domain/constants/localStoreKeys.ts`).
- **Async/Await**: Prefer `async/await` over `.then()` chains for readability.
- **Code Nesting & Guard Clauses**: Keep code structure flat. Use guard clauses for early exits to prevent deep nesting.
- **Component File Length & Extraction**: Keep components small and focused. Extract nested/long sections of JSX into separate local components in the same file. Create a new file only when a component is shared across multiple files.

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

### CLI Commands (from `frontend/`)
- **Dev Server**: `npm run dev`
- **Generate API Client**: `npm run generate-client`
- **Lint**: `npm run lint` (runs `oxlint`)
- **Type-Check & Build**: `npm run build` (runs `tsc -b && vite build`)
