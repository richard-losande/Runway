# BFF-to-ApiService Refit Integration Design

## Overview

Add a FastEndpoints endpoint in the BFF layer that proxies calls to the ApiService using Refit, authenticating with an API key via a DelegatingHandler.

**Auth flow:** Client (JWT/Keycloak) -> BFF Endpoint -> Refit (x-api-key header) -> ApiService

## Decisions

- **BFF framework:** FastEndpoints (consistent with ApiService)
- **API key delivery:** DelegatingHandler in the HttpClient pipeline
- **API key source:** BFF's own `appsettings.json` under `Auth:ApiKey`
- **Route:** Mirror ApiService routes (e.g., `GET /api/v1/say-hello`)
- **Service discovery:** Aspire service discovery (`https+http://apiservice`), already wired in AppHost

## File Structure

```
Hackathon.Bff/
  Integrations/
    ApiService/
      IApiServiceClient.cs        -- Refit interface for ApiService
      ApiKeyDelegatingHandler.cs   -- Attaches x-api-key header to all outgoing requests
  Features/
    SayHello/
      Endpoint.cs                  -- BFF endpoint (Request, Response, Endpoint)
  Program.cs                       -- Updated with FastEndpoints, Refit, ServiceDefaults
  appsettings.Development.json     -- Add Auth:ApiKey
```

## Components

### IApiServiceClient (Refit Interface)

Defines the ApiService contract from the BFF's perspective. DTOs are owned by the BFF, not shared.

```csharp
[Get("/api/v1/say-hello")]
Task<SayHelloResponse> SayHelloAsync([Query] string name);
```

### ApiKeyDelegatingHandler

Reads `Auth:ApiKey` from IConfiguration. Adds `x-api-key` header to every outgoing `HttpRequestMessage`.

### BFF SayHello Endpoint

FastEndpoint at `GET /api/v1/say-hello`. Injects `IApiServiceClient`, calls through to ApiService, returns result. Protected by the BFF's existing JWT fallback auth policy.

### Program.cs Changes

- Add `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
- Register `ApiKeyDelegatingHandler` as transient
- Register Refit client: `AddRefitClient<IApiServiceClient>()` with base address `https+http://apiservice` and `.AddHttpMessageHandler<ApiKeyDelegatingHandler>()`
- Add `app.UseFastEndpoints()` and `app.UseSwaggerGen()`

### NuGet Packages to Add (Hackathon.Bff.csproj)

- `FastEndpoints`
- `FastEndpoints.Swagger`
- `Refit`
- `Refit.HttpClientFactory`
