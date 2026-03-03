# BFF-to-ApiService Refit Integration — Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a FastEndpoints endpoint in the BFF that proxies calls to the ApiService using Refit, with API key authentication via a DelegatingHandler.

**Architecture:** The BFF receives JWT-authenticated requests from clients, then calls the ApiService using Refit with an `x-api-key` header injected by a DelegatingHandler. Aspire service discovery resolves the ApiService URL. The BFF owns its own DTOs and Refit interface — nothing is shared from ApiService.

**Tech Stack:** .NET 10, FastEndpoints 8.0.1, Refit 10.0.1, Aspire 9.5 service discovery

---

### Task 1: Add NuGet packages to BFF

**Files:**
- Modify: `Hackathon.Bff/Hackathon.Bff.csproj`

**Step 1: Add the four required packages**

Run these from the repo root:

```bash
dotnet add Hackathon.Bff package FastEndpoints --version 8.0.1
dotnet add Hackathon.Bff package FastEndpoints.Swagger --version 8.0.1
dotnet add Hackathon.Bff package Refit --version 10.0.1
dotnet add Hackathon.Bff package Refit.HttpClientFactory --version 10.0.1
```

**Step 2: Verify restore succeeds**

```bash
dotnet restore Hackathon.Bff
```

Expected: `Restore completed` with no errors.

**Step 3: Commit**

```bash
git add Hackathon.Bff/Hackathon.Bff.csproj
git commit -m "feat(bff): add FastEndpoints and Refit packages"
```

---

### Task 2: Add API key config to BFF appsettings

**Files:**
- Modify: `Hackathon.Bff/appsettings.Development.json`

**Step 1: Add the Auth:ApiKey section**

Edit `Hackathon.Bff/appsettings.Development.json` to add the `Auth` section. The final file should be:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Auth": {
    "ApiKey": "sampleApiKey12332131"
  }
}
```

This matches the key in `Hackathon.AppHost/appsettings.Development.json` that the ApiService validates against.

**Step 2: Commit**

```bash
git add Hackathon.Bff/appsettings.Development.json
git commit -m "feat(bff): add Auth:ApiKey config for ApiService calls"
```

---

### Task 3: Create ApiKeyDelegatingHandler

**Files:**
- Create: `Hackathon.Bff/Integrations/ApiService/ApiKeyDelegatingHandler.cs`

**Step 1: Create the handler**

```csharp
namespace Hackathon.Bff.Integrations.ApiService;

public class ApiKeyDelegatingHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public ApiKeyDelegatingHandler(IConfiguration configuration)
    {
        _apiKey = configuration["Auth:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: Auth:ApiKey");
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.Bff
```

Expected: `Build succeeded` with 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.Bff/Integrations/ApiService/ApiKeyDelegatingHandler.cs
git commit -m "feat(bff): add ApiKeyDelegatingHandler for ApiService auth"
```

---

### Task 4: Create Refit interface IApiServiceClient

**Files:**
- Create: `Hackathon.Bff/Integrations/ApiService/IApiServiceClient.cs`

**Step 1: Create the Refit interface with response DTO**

```csharp
using Refit;

namespace Hackathon.Bff.Integrations.ApiService;

public class SayHelloResponse
{
    public required string Message { get; set; }
}

[Headers("Accept: application/json")]
public interface IApiServiceClient
{
    [Get("/api/v1/say-hello")]
    Task<SayHelloResponse> SayHelloAsync([Query] string name, CancellationToken cancellationToken = default);
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.Bff
```

Expected: `Build succeeded` with 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.Bff/Integrations/ApiService/IApiServiceClient.cs
git commit -m "feat(bff): add IApiServiceClient Refit interface"
```

---

### Task 5: Create BFF SayHello endpoint

**Files:**
- Create: `Hackathon.Bff/Features/SayHello/Endpoint.cs`

**Step 1: Create the endpoint**

```csharp
using FastEndpoints;
using Hackathon.Bff.Integrations.ApiService;

namespace Hackathon.Bff.Features.SayHello;

public class Request
{
    public required string Name { get; set; }
}

public class Response
{
    public required string Message { get; set; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IApiServiceClient _apiServiceClient;

    public Endpoint(IApiServiceClient apiServiceClient)
    {
        _apiServiceClient = apiServiceClient;
    }

    public override void Configure() => Get("/api/v1/say-hello");

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var result = await _apiServiceClient.SayHelloAsync(request.Name, ct);
        await SendOkAsync(new Response { Message = result.Message }, ct);
    }
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.Bff
```

Expected: `Build succeeded` with 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.Bff/Features/SayHello/Endpoint.cs
git commit -m "feat(bff): add SayHello endpoint that proxies to ApiService"
```

---

### Task 6: Wire everything up in Program.cs

**Files:**
- Modify: `Hackathon.Bff/Program.cs`

**Step 1: Add required usings and service registrations**

Add these usings at the top of `Program.cs`:

```csharp
using FastEndpoints;
using FastEndpoints.Swagger;
using Hackathon.Bff.Integrations.ApiService;
using Refit;
```

**Step 2: Add ServiceDefaults, FastEndpoints, Refit, and DelegatingHandler registrations**

After the existing `var builder = WebApplication.CreateBuilder(args);` line, add `builder.AddServiceDefaults();` right after `builder.Services.AddProblemDetails();`.

Before `var app = builder.Build();`, add these registrations:

```csharp
builder.Services.AddFastEndpoints();
builder.Services.AddTransient<ApiKeyDelegatingHandler>();
builder.Services
    .AddRefitClient<IApiServiceClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https+http://apiservice"))
    .AddHttpMessageHandler<ApiKeyDelegatingHandler>();
```

**Step 3: Add middleware and endpoint mapping**

After `app.UseAuthorization();`, add:

```csharp
app.UseFastEndpoints();
```

Before `app.Run();`, add:

```csharp
app.MapDefaultEndpoints();
```

The final `Program.cs` middleware pipeline order should be:
1. `app.UseHttpsRedirection();`
2. `app.UseSwaggerGen();` (dev only)
3. `app.UseCors();`
4. `app.UseAuthentication();`
5. `app.UseAuthorization();`
6. `app.UseFastEndpoints();`
7. `app.MapDefaultEndpoints();`
8. `app.Run();`

**Step 4: Verify the whole solution compiles**

```bash
dotnet build Hackathon.slnx
```

Expected: `Build succeeded` with 0 errors across all projects.

**Step 5: Commit**

```bash
git add Hackathon.Bff/Program.cs
git commit -m "feat(bff): wire up FastEndpoints, Refit client, and ServiceDefaults in Program.cs"
```

---

### Task 7: Smoke test with Aspire

**Step 1: Run the AppHost**

```bash
dotnet run --project Hackathon.AppHost
```

Expected: Aspire dashboard opens. Both `apiservice` and `bff` show as healthy.

**Step 2: Test the BFF endpoint**

Using the Aspire dashboard, find the BFF's URL. Then test:

```bash
curl -H "Authorization: Bearer <valid-jwt>" "http://localhost:<bff-port>/api/v1/say-hello?name=World"
```

Expected response:

```json
{"message": "Hello, World!"}
```

Note: If Keycloak is not available for JWT testing, you can temporarily test by commenting out the auth fallback policy in the BFF's `Program.cs` or using Swagger UI.

**Step 3: Verify Swagger**

Navigate to the BFF's Swagger UI (e.g., `http://localhost:<bff-port>/swagger`) and confirm the `GET /api/v1/say-hello` endpoint is listed.
