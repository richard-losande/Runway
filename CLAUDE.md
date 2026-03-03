# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

This is a .NET 10 / Aspire 9.5 solution. The orchestrator is `Hackathon.AppHost`.

```bash
# Run the full stack (starts all services via Aspire)
dotnet run --project Hackathon.AppHost

# Build entire solution
dotnet build Hackathon.slnx

# Run a single project
dotnet run --project Hackathon.ApiService
dotnet run --project Hackathon.Bff
```

There are no tests yet. The solution file is `Hackathon.slnx` (XML-based slnx format).

## Architecture

```
Hackathon.AppHost          -- Aspire orchestrator; wires up all services
Hackathon.ApiService       -- Backend API (FastEndpoints + EF Core + PostgreSQL)
Hackathon.Bff              -- Backend-for-Frontend gateway (JWT/Keycloak auth)
Hackathon.Web              -- Blazor Server frontend (not in AppHost yet)
Hackathon.ServiceDefaults  -- Shared Aspire defaults (OpenTelemetry, health checks, service discovery, resilience)
```

**Service graph (defined in `AppHost/AppHost.cs`):**
- `apiservice` exposes `/health`
- `bff` references `apiservice` and waits for it to be healthy

**Note:** `Hackathon.Web` has a csproj but is not referenced by the AppHost and is not wired into the Aspire orchestration. It contains a Blazor Server app with sample weather pages.

## Key Patterns

### API Endpoints (ApiService)
Uses **FastEndpoints** (v8). Endpoints live under `Features/{FeatureName}/Endpoint.cs` following a vertical-slice layout. Each file contains `Request`, `Response`, and `Endpoint` classes together.

Example pattern — `Features/SayHello/Endpoint.cs`:
- `Request` / `Response` DTOs as inner or sibling classes
- `Endpoint<TRequest, TResponse>` with `Configure()` for route and `HandleAsync()` for logic
- Inject `IMaindbContext` for database access

Planned folder structure includes `Databases/Models/`, `Integrations/`, and `Shared/` (currently empty).

### Authentication
- **ApiService**: Custom API key scheme via `x-api-key` header. Key stored in config at `Auth:ApiKey`. Health checks, swagger, and `/` are public paths.
- **BFF**: JWT Bearer via Keycloak. Multi-tenant — realm is extracted from the JWT issuer. Config key: `KeyCloak:BaseUrl`. OIDC configuration is cached per-issuer via `KeycloakOidcManagerCache`.

### Database
PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`. Connection string key: `hackathonDb`. The `MainDbContext` is registered with an `IMaindbContext` interface for DI.

### Swagger
Available in development via FastEndpoints.Swagger on both ApiService and BFF. ApiService documents the `x-api-key` auth scheme in OpenAPI.

### Service Defaults
All services should call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`. This adds OpenTelemetry, health checks (`/health`, `/alive`), service discovery, and HTTP resilience.
