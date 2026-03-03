# Financial Runway Simulator — Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Build a single POST endpoint that accepts a bank CSV, salary, savings, and life events, calls OpenAI GPT-5 to analyze everything, and returns structured financial runway data plus narrative advice.

**Architecture:** ApiService receives the request, sends everything to OpenAI in a single call with structured JSON output, maps the result, saves to PostgreSQL, and returns it. BFF proxies the call via Refit. OpenAI API key lives in ApiService appsettings.

**Tech Stack:** .NET 10, FastEndpoints 8.0.1, OpenAI REST API (HttpClient), EF Core 10 + PostgreSQL, Refit 10.0.1

---

### Task 1: Add OpenAI config to ApiService appsettings

**Files:**
- Modify: `Hackathon.AppHost/appsettings.Development.json`

**Step 1: Add OpenAI section**

Add the `OpenAI` section to `Hackathon.AppHost/appsettings.Development.json`. The file should become:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Aspire.Hosting.Dcp": "Warning"
    }
  },
  "Auth": {
    "ApiKey": "sampleApiKey12332131"
  },
  "ConnectionStrings": {
    "hackathonDb": "USER ID = pgSproutAdmin; Password=pXkeGkmGBK2t?sTByqKZM!zVJdG459; Server=psql-spr-test-server.postgres.database.azure.com; Port=5432;Database = hackathon_prometheus; Pooling = true;"
  },
  "OpenAI": {
    "ApiKey": "<copy from Hackathon.Bff/appsettings.Development.json>",
    "Model": "gpt-4o"
  }
}
```

Note: Copy the actual OpenAI API key value from the BFF's appsettings. Use model `gpt-4o` (the value in BFF says `gpt-5` but use what's actually available).

**Step 2: Commit**

```bash
git add Hackathon.AppHost/appsettings.Development.json
git commit -m "feat(api): add OpenAI config to AppHost appsettings"
```

---

### Task 2: Create Models (DTOs + DB entity)

**Files:**
- Create: `Hackathon.ApiService/Features/FinancialRunway/Models.cs`

**Step 1: Create the models file**

```csharp
using System.Text.Json.Serialization;

namespace Hackathon.ApiService.Features.FinancialRunway;

// === Enums ===

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LifeEventType
{
    BuyHouse,
    BuyCar,
    HaveBaby,
    LoseJob,
    GetRaise,
    Custom
}

// === Request DTOs ===

public class LifeEventInput
{
    public LifeEventType Type { get; set; }
    public string? Description { get; set; }
    public int MonthFromNow { get; set; }
}

// === Response DTOs ===

public class AnalyzeResponse
{
    public int RunwayMonths { get; set; }
    public decimal MonthlyBurnRate { get; set; }
    public List<CategorizedExpense> CategorizedExpenses { get; set; } = [];
    public List<MonthlyProjection> MonthlyProjections { get; set; } = [];
    public List<LifeEventImpact> LifeEventImpacts { get; set; } = [];
    public int AdjustedRunwayMonths { get; set; }
    public string Narrative { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}

public class CategorizedExpense
{
    public string Category { get; set; } = string.Empty;
    public decimal MonthlyAverage { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlyProjection
{
    public int Month { get; set; }
    public decimal Balance { get; set; }
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
}

public class LifeEventImpact
{
    public string Event { get; set; } = string.Empty;
    public int ImpactOnRunway { get; set; }
    public decimal NewMonthlyExpense { get; set; }
}

// === DB Entity ===

public class FinancialAnalysis
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public decimal MonthlySalary { get; set; }
    public decimal TotalSavings { get; set; }
    public int RunwayMonths { get; set; }
    public int AdjustedRunwayMonths { get; set; }
    public decimal MonthlyBurnRate { get; set; }
    public string ResponseJson { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.ApiService
```

Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/FinancialRunway/Models.cs
git commit -m "feat(api): add Financial Runway DTOs and DB entity"
```

---

### Task 3: Add FinancialAnalysis to DbContext + create migration

**Files:**
- Modify: `Hackathon.ApiService/Databases/DbContexts/MainDbContext.cs`

**Step 1: Add DbSet to MainDbContext**

```csharp
using Hackathon.ApiService.Features.FinancialRunway;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.ApiService.Databases.DbContexts
{
    public class MainDbContext : DbContext, IMaindbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        public DbSet<FinancialAnalysis> FinancialAnalyses => Set<FinancialAnalysis>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FinancialAnalysis>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ResponseJson).HasColumnType("jsonb");
            });
        }
    }
}
```

**Step 2: Install EF Core tools if not installed**

```bash
dotnet tool install --global dotnet-ef
```

(Skip if already installed.)

**Step 3: Create migration**

```bash
dotnet ef migrations add AddFinancialAnalysis --project Hackathon.ApiService --startup-project Hackathon.ApiService
```

Note: This may fail if the DB connection string is unreachable. That's OK for a hackathon — the migration files are generated and can be applied later. If it fails on connection, try:

```bash
dotnet ef migrations add AddFinancialAnalysis --project Hackathon.ApiService --startup-project Hackathon.ApiService -- --connection "Host=localhost;Database=hackathon_dev;Username=postgres;Password=postgres"
```

**Step 4: Verify it compiles**

```bash
dotnet build Hackathon.ApiService
```

**Step 5: Commit**

```bash
git add Hackathon.ApiService/Databases/DbContexts/MainDbContext.cs Hackathon.ApiService/Migrations/
git commit -m "feat(api): add FinancialAnalysis entity to DbContext with migration"
```

---

### Task 4: Create OpenAiService

**Files:**
- Create: `Hackathon.ApiService/Features/FinancialRunway/OpenAiService.cs`

**Step 1: Create the service**

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hackathon.ApiService.Features.FinancialRunway;

public interface IOpenAiService
{
    Task<AnalyzeResponse> AnalyzeFinancialDataAsync(
        string csvContent,
        decimal monthlySalary,
        decimal totalSavings,
        List<LifeEventInput> lifeEvents,
        CancellationToken ct);
}

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<AnalyzeResponse> AnalyzeFinancialDataAsync(
        string csvContent,
        decimal monthlySalary,
        decimal totalSavings,
        List<LifeEventInput> lifeEvents,
        CancellationToken ct)
    {
        var lifeEventsText = lifeEvents.Count > 0
            ? string.Join("\n", lifeEvents.Select(e =>
                $"- {e.Type}{(string.IsNullOrEmpty(e.Description) ? "" : $": {e.Description}")} in {e.MonthFromNow} months"))
            : "None";

        var systemMessage = @"You are a financial analyst. Given a bank statement CSV, monthly salary, savings, and planned life events, produce a structured JSON analysis of the user's financial runway.

Return ONLY valid JSON matching this exact schema:
{
  ""runwayMonths"": <int, months until savings depleted assuming current burn rate minus salary>,
  ""monthlyBurnRate"": <decimal, average monthly spending from CSV>,
  ""categorizedExpenses"": [{ ""category"": ""<string>"", ""monthlyAverage"": <decimal>, ""percentage"": <decimal 0-100> }],
  ""monthlyProjections"": [{ ""month"": <int 1-12>, ""balance"": <decimal>, ""income"": <decimal>, ""expenses"": <decimal> }],
  ""lifeEventImpacts"": [{ ""event"": ""<string>"", ""impactOnRunway"": <int, months added or removed>, ""newMonthlyExpense"": <decimal> }],
  ""adjustedRunwayMonths"": <int, runway after all life events>,
  ""narrative"": ""<string, 2-3 paragraph analysis with actionable advice>""
}";

        var userMessage = $@"## Financial Data

**Monthly Salary:** {monthlySalary:C}
**Total Savings:** {totalSavings:C}

## Bank Statement CSV
```
{csvContent}
```

## Planned Life Events
{lifeEventsText}

Analyze this data and return the JSON analysis.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            response_format = new { type = "json_object" },
            temperature = 0.3
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(messageContent))
            throw new InvalidOperationException("OpenAI returned empty content");

        var result = JsonSerializer.Deserialize<AnalyzeResponse>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize OpenAI response");

        result.AnalyzedAt = DateTime.UtcNow;
        return result;
    }
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.ApiService
```

Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/FinancialRunway/OpenAiService.cs
git commit -m "feat(api): add OpenAiService for financial runway analysis"
```

---

### Task 5: Create ApiService AnalyzeEndpoint

**Files:**
- Create: `Hackathon.ApiService/Features/FinancialRunway/AnalyzeEndpoint.cs`

**Step 1: Create the endpoint**

```csharp
using System.Text.Json;
using FastEndpoints;
using Hackathon.ApiService.Databases.DbContexts;

namespace Hackathon.ApiService.Features.FinancialRunway;

public class AnalyzeRequest
{
    public IFormFile CsvFile { get; set; } = null!;
    public decimal MonthlySalary { get; set; }
    public decimal TotalSavings { get; set; }
    public string? LifeEventsJson { get; set; }
}

public class AnalyzeEndpoint : Endpoint<AnalyzeRequest, AnalyzeResponse>
{
    private readonly IOpenAiService _openAiService;
    private readonly MainDbContext _dbContext;

    public AnalyzeEndpoint(IOpenAiService openAiService, MainDbContext dbContext)
    {
        _openAiService = openAiService;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/v1/financial-runway/analyze");
        AllowFileUploads();
    }

    public override async Task HandleAsync(AnalyzeRequest req, CancellationToken ct)
    {
        // Read CSV content
        using var reader = new StreamReader(req.CsvFile.OpenReadStream());
        var csvContent = await reader.ReadToEndAsync(ct);

        // Parse life events
        var lifeEvents = new List<LifeEventInput>();
        if (!string.IsNullOrWhiteSpace(req.LifeEventsJson))
        {
            lifeEvents = JsonSerializer.Deserialize<List<LifeEventInput>>(req.LifeEventsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }

        // Call OpenAI
        var result = await _openAiService.AnalyzeFinancialDataAsync(
            csvContent, req.MonthlySalary, req.TotalSavings, lifeEvents, ct);

        // Save to DB
        var entity = new FinancialAnalysis
        {
            Id = Guid.NewGuid(),
            MonthlySalary = req.MonthlySalary,
            TotalSavings = req.TotalSavings,
            RunwayMonths = result.RunwayMonths,
            AdjustedRunwayMonths = result.AdjustedRunwayMonths,
            MonthlyBurnRate = result.MonthlyBurnRate,
            ResponseJson = JsonSerializer.Serialize(result),
            AnalyzedAt = result.AnalyzedAt
        };

        _dbContext.FinancialAnalyses.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}
```

**Step 2: Verify it compiles**

```bash
dotnet build Hackathon.ApiService
```

Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/FinancialRunway/AnalyzeEndpoint.cs
git commit -m "feat(api): add FinancialRunway AnalyzeEndpoint"
```

---

### Task 6: Register OpenAiService in ApiService Program.cs

**Files:**
- Modify: `Hackathon.ApiService/Program.cs`

**Step 1: Add the service registration**

Add this line after the `AddScoped<IMaindbContext>` line (around line 19) in `Hackathon.ApiService/Program.cs`:

```csharp
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();
```

Also add the using at the top:

```csharp
using Hackathon.ApiService.Features.FinancialRunway;
```

**Step 2: Verify the full solution compiles**

```bash
dotnet build Hackathon.slnx
```

Expected: Build succeeded, 0 errors.

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Program.cs
git commit -m "feat(api): register OpenAiService in DI"
```

---

### Task 7: Add BFF Refit method + proxy endpoint

**Files:**
- Modify: `Hackathon.Bff/Integrations/ApiService/IApiServiceClient.cs`
- Create: `Hackathon.Bff/Features/FinancialRunway/AnalyzeEndpoint.cs`

**Step 1: Add Refit method to IApiServiceClient**

Add this method and response DTO to `Hackathon.Bff/Integrations/ApiService/IApiServiceClient.cs`:

```csharp
using Refit;

namespace Hackathon.Bff.Integrations.ApiService;

public class SayHelloResponse
{
    public required string Message { get; set; }
}

public class FinancialRunwayResponse
{
    public int RunwayMonths { get; set; }
    public decimal MonthlyBurnRate { get; set; }
    public List<object> CategorizedExpenses { get; set; } = [];
    public List<object> MonthlyProjections { get; set; } = [];
    public List<object> LifeEventImpacts { get; set; } = [];
    public int AdjustedRunwayMonths { get; set; }
    public string Narrative { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}

[Headers("Accept: application/json")]
public interface IApiServiceClient
{
    [Get("/api/v1/say-hello")]
    Task<SayHelloResponse> SayHelloAsync([Query] string name, CancellationToken cancellationToken = default);

    [Multipart]
    [Post("/api/v1/financial-runway/analyze")]
    Task<FinancialRunwayResponse> AnalyzeFinancialRunwayAsync(
        [AliasAs("csvFile")] StreamPart csvFile,
        [AliasAs("monthlySalary")] decimal monthlySalary,
        [AliasAs("totalSavings")] decimal totalSavings,
        [AliasAs("lifeEventsJson")] string? lifeEventsJson,
        CancellationToken cancellationToken = default);
}
```

**Step 2: Create BFF proxy endpoint**

Create `Hackathon.Bff/Features/FinancialRunway/AnalyzeEndpoint.cs`:

```csharp
using FastEndpoints;
using Hackathon.Bff.Integrations.ApiService;
using Refit;

namespace Hackathon.Bff.Features.FinancialRunway;

public class AnalyzeRequest
{
    public IFormFile CsvFile { get; set; } = null!;
    public decimal MonthlySalary { get; set; }
    public decimal TotalSavings { get; set; }
    public string? LifeEventsJson { get; set; }
}

public class AnalyzeEndpoint : Endpoint<AnalyzeRequest, FinancialRunwayResponse>
{
    private readonly IApiServiceClient _apiServiceClient;

    public AnalyzeEndpoint(IApiServiceClient apiServiceClient)
    {
        _apiServiceClient = apiServiceClient;
    }

    public override void Configure()
    {
        Post("/api/v1/financial-runway/analyze");
        AllowFileUploads();
    }

    public override async Task HandleAsync(AnalyzeRequest req, CancellationToken ct)
    {
        var stream = req.CsvFile.OpenReadStream();
        var streamPart = new StreamPart(stream, req.CsvFile.FileName, req.CsvFile.ContentType);

        var result = await _apiServiceClient.AnalyzeFinancialRunwayAsync(
            streamPart, req.MonthlySalary, req.TotalSavings, req.LifeEventsJson, ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}
```

**Step 3: Verify the full solution compiles**

```bash
dotnet build Hackathon.slnx
```

Expected: Build succeeded, 0 errors.

**Step 4: Commit**

```bash
git add Hackathon.Bff/Integrations/ApiService/IApiServiceClient.cs Hackathon.Bff/Features/FinancialRunway/AnalyzeEndpoint.cs
git commit -m "feat(bff): add FinancialRunway proxy endpoint with Refit multipart"
```

---

### Task 8: Smoke test end-to-end

**Step 1: Create a sample CSV file for testing**

Create a file `test-bank-statement.csv` in the repo root (for testing only):

```csv
Date,Description,Amount,Type
2026-01-05,Monthly Salary,5000.00,Credit
2026-01-07,Rent Payment,-1500.00,Debit
2026-01-10,Grocery Store,-250.00,Debit
2026-01-12,Electric Bill,-120.00,Debit
2026-01-15,Netflix,-15.99,Debit
2026-01-18,Gas Station,-60.00,Debit
2026-01-20,Restaurant,-85.00,Debit
2026-01-25,Internet Bill,-79.99,Debit
2026-02-05,Monthly Salary,5000.00,Credit
2026-02-07,Rent Payment,-1500.00,Debit
2026-02-09,Grocery Store,-310.00,Debit
2026-02-11,Water Bill,-45.00,Debit
2026-02-14,Gym Membership,-50.00,Debit
2026-02-18,Gas Station,-55.00,Debit
2026-02-22,Clothing Store,-200.00,Debit
2026-02-28,Phone Bill,-85.00,Debit
```

**Step 2: Run the AppHost**

```bash
dotnet run --project Hackathon.AppHost
```

**Step 3: Test the ApiService endpoint directly**

```bash
curl -X POST "http://localhost:5407/api/v1/financial-runway/analyze" \
  -H "x-api-key: sampleApiKey12332131" \
  -F "csvFile=@test-bank-statement.csv" \
  -F "monthlySalary=5000" \
  -F "totalSavings=25000" \
  -F 'lifeEventsJson=[{"type":"BuyCar","description":"Used Toyota Corolla","monthFromNow":3},{"type":"Custom","description":"Start a side business requiring $2000 upfront","monthFromNow":6}]'
```

Expected: JSON response with `runwayMonths`, `categorizedExpenses`, `monthlyProjections`, `lifeEventImpacts`, `narrative`, etc.

**Step 4: Verify Swagger shows the new endpoint**

Navigate to `http://localhost:5407/swagger` and confirm the `POST /api/v1/financial-runway/analyze` endpoint is listed with file upload support.
