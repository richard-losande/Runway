# Runway Superpowers Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Rebuild the financial runway feature into a 3-superpower agentic architecture with a Vue 3 frontend, matching the Superpowers PRD exactly.

**Architecture:** Backend refactored into SP1 (Transaction Intelligence, OpenAI), SP2 (Survival Simulator, deterministic), SP3 (Behavioral Intelligence, OpenAI + deterministic), and an Orchestrator with data-driven routing. Frontend is a new Vue 3 + Vite + Sprout Design System app with 5 sequential screens. Frontend calls ApiService directly (no BFF, no auth).

**Tech Stack:** .NET 10 / FastEndpoints 8 / OpenAI REST API / Vue 3 / TypeScript / Vite / Pinia / design-system-next / Axios

---

## Task 1: Create Demo CSV Data

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/DemoData/AlexTransactions.csv`

**Step 1: Create the demo CSV**

Create Alex's 4-month transaction history (August–November 2024). Grab Food must grow 38% across 4 months. Shopee must grow 22% month over month. Total monthly burn must average ₱52,400 with breakdown: Fixed ₱28,000, Variable ₱14,200, Discretionary ₱10,200.

```csv
date,description,amount,type
2024-08-01,Landlord Rent Payment,18000.00,debit
2024-08-03,Meralco Electric Bill,3100.00,debit
2024-08-03,Manila Water,780.00,debit
2024-08-05,Netflix Subscription,649.00,debit
2024-08-05,Spotify Premium,169.00,debit
2024-08-07,Sprout Salary Loan,5000.00,debit
2024-08-10,SM Supermarket,5100.00,debit
2024-08-12,Mercury Drug,1750.00,debit
2024-08-14,Grab Transport,3300.00,debit
2024-08-15,Metered Taxi,800.00,debit
2024-08-08,GRAB*FOOD 8101,4800.00,debit
2024-08-11,GRAB*FOOD 8102,450.00,debit
2024-08-18,GRAB*FOOD 8103,380.00,debit
2024-08-22,GRAB*FOOD 8104,520.00,debit
2024-08-10,Shopee Purchase,1800.00,debit
2024-08-20,Lazada Order,200.00,debit
2024-08-16,Coffee Bean & Tea Leaf,450.00,debit
2024-08-25,Starbucks,550.00,debit
2024-09-01,Landlord Rent Payment,18000.00,debit
2024-09-03,Meralco Electric Bill,3200.00,debit
2024-09-03,Manila Water,800.00,debit
2024-09-05,Netflix Subscription,649.00,debit
2024-09-05,Spotify Premium,169.00,debit
2024-09-07,Sprout Salary Loan,5000.00,debit
2024-09-10,SM Supermarket,5200.00,debit
2024-09-12,Mercury Drug,1800.00,debit
2024-09-14,Grab Transport,3400.00,debit
2024-09-15,Metered Taxi,800.00,debit
2024-09-08,GRAB*FOOD 9101,5200.00,debit
2024-09-13,GRAB*FOOD 9102,600.00,debit
2024-09-19,GRAB*FOOD 9103,420.00,debit
2024-09-24,GRAB*FOOD 9104,580.00,debit
2024-09-10,Shopee Purchase,2100.00,debit
2024-09-22,Lazada Order,340.00,debit
2024-09-16,Coffee Bean & Tea Leaf,500.00,debit
2024-09-25,Starbucks,500.00,debit
2024-10-01,Landlord Rent Payment,18000.00,debit
2024-10-03,Meralco Electric Bill,3250.00,debit
2024-10-03,Manila Water,810.00,debit
2024-10-05,Netflix Subscription,649.00,debit
2024-10-05,Spotify Premium,169.00,debit
2024-10-07,Sprout Salary Loan,5000.00,debit
2024-10-10,SM Supermarket,5250.00,debit
2024-10-12,Mercury Drug,1850.00,debit
2024-10-14,Grab Transport,3500.00,debit
2024-10-15,Metered Taxi,800.00,debit
2024-10-08,GRAB*FOOD 1001,6100.00,debit
2024-10-13,GRAB*FOOD 1002,700.00,debit
2024-10-19,GRAB*FOOD 1003,480.00,debit
2024-10-24,GRAB*FOOD 1004,620.00,debit
2024-10-10,Shopee Purchase,2500.00,debit
2024-10-22,Lazada Order,440.00,debit
2024-10-16,Coffee Bean & Tea Leaf,520.00,debit
2024-10-25,Starbucks,480.00,debit
2024-11-01,Landlord Rent Payment,18000.00,debit
2024-11-03,Meralco Electric Bill,3300.00,debit
2024-11-03,Manila Water,820.00,debit
2024-11-05,Netflix Subscription,649.00,debit
2024-11-05,Spotify Premium,169.00,debit
2024-11-07,Sprout Salary Loan,5000.00,debit
2024-11-10,SM Supermarket,5300.00,debit
2024-11-12,Mercury Drug,1850.00,debit
2024-11-14,Grab Transport,3500.00,debit
2024-11-15,Metered Taxi,800.00,debit
2024-11-08,GRAB*FOOD 1101,7200.00,debit
2024-11-13,GRAB*FOOD 1102,800.00,debit
2024-11-19,GRAB*FOOD 1103,550.00,debit
2024-11-24,GRAB*FOOD 1104,650.00,debit
2024-11-10,Shopee Purchase,3000.00,debit
2024-11-22,Lazada Order,500.00,debit
2024-11-16,Coffee Bean & Tea Leaf,500.00,debit
2024-11-25,Starbucks,500.00,debit
```

**Step 2: Verify the CSV is set as embedded resource**

In the csproj file, ensure the CSV is included as an embedded resource so the API can load it at runtime:

Add to `Hackathon.ApiService/Hackathon.ApiService.csproj`:
```xml
<ItemGroup>
  <EmbeddedResource Include="Features\Runway\DemoData\AlexTransactions.csv" />
</ItemGroup>
```

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/DemoData/AlexTransactions.csv Hackathon.ApiService/Hackathon.ApiService.csproj
git commit -m "feat: add Alex demo CSV transaction data for Runway"
```

---

## Task 2: Create SP2 Models (Deterministic — No Dependencies)

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Models/Sp2Models.cs`

**Step 1: Write the SP2 DTOs**

These match the PRD Section 9 schemas exactly.

```csharp
using System.Text.Json.Serialization;

namespace Hackathon.ApiService.Features.Runway.Models;

// === SP2 Input ===

public class Sp2Input
{
    public decimal MonthlyBurn { get; set; }
    public BurnBreakdown BurnBreakdown { get; set; } = new();
    public decimal MonthlyIncome { get; set; }
    public decimal LiquidSavings { get; set; }
    public string PriorityScenario { get; set; } = "cut_lifestyle";
    public List<string> ActiveScenarios { get; set; } = [];
}

// === SP2 Output ===

public class Sp2Output
{
    public BaselineResult Baseline { get; set; } = new();
    public List<ScenarioResult> Scenarios { get; set; } = [];
    public StackedResult StackedResult { get; set; } = new();
}

public class BaselineResult
{
    public int SurvivalDays { get; set; }
    public string HumanLabel { get; set; } = string.Empty;
    public decimal MonthlyBurn { get; set; }
    public decimal MonthlySurplus { get; set; }
    public double BurnRate { get; set; }
    public string StabilityZone { get; set; } = string.Empty;
}

public class ScenarioResult
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int SurvivalDays { get; set; }
    public int DeltaDays { get; set; }
    public bool IsPriority { get; set; }
}

public class StackedResult
{
    public List<string> ActiveScenarios { get; set; } = [];
    public int SurvivalDays { get; set; }
    public int DeltaDays { get; set; }
}

// === Shared ===

public class BurnBreakdown
{
    public decimal Fixed { get; set; }
    public decimal Variable { get; set; }
    public decimal Discretionary { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StabilityZone
{
    Critical,
    Fragile,
    Stable,
    Strong
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Models/Sp2Models.cs
git commit -m "feat: add SP2 Survival Simulator DTOs"
```

---

## Task 3: Implement SP2 — Survival & Scenario Simulator

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Services/ISurvivalSimulator.cs`
- Create: `Hackathon.ApiService/Features/Runway/Services/SurvivalSimulator.cs`

**Step 1: Write the SP2 interface**

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface ISurvivalSimulator
{
    Sp2Output Calculate(Sp2Input input);
}
```

**Step 2: Write the SP2 implementation**

All logic is deterministic. No OpenAI calls. Formulas match PRD Section 9 exactly.

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class SurvivalSimulator : ISurvivalSimulator
{
    private static readonly Dictionary<string, (string Label, string DisplayName)> ScenarioMeta = new()
    {
        ["cut_lifestyle"] = ("Cut dining & subscriptions", "\u2702\ufe0f Cut dining & subscriptions"),
        ["side_hustle"] = ("Add \u20b110k side hustle", "\U0001f4bc Start a \u20b110k side hustle"),
        ["salary_increase"] = ("Get a 10% salary raise", "\U0001f4c8 Get a 10% salary raise"),
        ["major_upgrade"] = ("Upgrade your lifestyle", "\U0001f697 Upgrade your lifestyle"),
    };

    public Sp2Output Calculate(Sp2Input input)
    {
        var baselineDays = ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn);
        var surplus = input.MonthlyIncome - input.MonthlyBurn;
        var burnRate = input.MonthlyIncome > 0
            ? (double)(input.MonthlyBurn / input.MonthlyIncome)
            : 1.0;
        var zone = ClassifyZone(baselineDays);

        var baseline = new BaselineResult
        {
            SurvivalDays = baselineDays,
            HumanLabel = FormatHumanLabel(baselineDays),
            MonthlyBurn = input.MonthlyBurn,
            MonthlySurplus = surplus,
            BurnRate = Math.Round(burnRate, 3),
            StabilityZone = zone,
        };

        // Compute individual scenario results
        var scenarios = new List<ScenarioResult>
        {
            ComputeScenario("cut_lifestyle", input, baselineDays),
            ComputeScenario("side_hustle", input, baselineDays),
            ComputeScenario("salary_increase", input, baselineDays),
            ComputeScenario("major_upgrade", input, baselineDays),
        };

        // Priority scenario goes first
        var priorityIndex = scenarios.FindIndex(s => s.Id == input.PriorityScenario);
        if (priorityIndex > 0)
        {
            var priority = scenarios[priorityIndex];
            scenarios.RemoveAt(priorityIndex);
            scenarios.Insert(0, priority);
        }
        if (scenarios.Count > 0)
        {
            var first = scenarios.First(s => s.Id == input.PriorityScenario);
            first.IsPriority = true;
        }

        // Compute stacked result
        var stacked = ComputeStacked(input, baselineDays);

        return new Sp2Output
        {
            Baseline = baseline,
            Scenarios = scenarios,
            StackedResult = stacked,
        };
    }

    private ScenarioResult ComputeScenario(string scenarioId, Sp2Input input, int baselineDays)
    {
        var scenarioDays = ApplyScenario(scenarioId, input);
        var meta = ScenarioMeta[scenarioId];
        return new ScenarioResult
        {
            Id = scenarioId,
            Label = meta.Label,
            SurvivalDays = scenarioDays,
            DeltaDays = scenarioDays - baselineDays,
            IsPriority = false,
        };
    }

    private int ApplyScenario(string scenarioId, Sp2Input input)
    {
        return scenarioId switch
        {
            "cut_lifestyle" => ComputeSurvivalDays(
                input.LiquidSavings,
                input.MonthlyBurn - (input.BurnBreakdown.Discretionary * 0.70m)),
            "side_hustle" => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn) + 6,
            "salary_increase" => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn) + 15,
            "major_upgrade" => ComputeSurvivalDays(
                input.LiquidSavings,
                input.MonthlyBurn + 15000m),
            _ => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn),
        };
    }

    private StackedResult ComputeStacked(Sp2Input input, int baselineDays)
    {
        if (input.ActiveScenarios.Count == 0)
        {
            return new StackedResult
            {
                ActiveScenarios = [],
                SurvivalDays = baselineDays,
                DeltaDays = 0,
            };
        }

        // Apply scenarios cumulatively
        var currentBurn = input.MonthlyBurn;
        var bonusDays = 0;

        foreach (var scenario in input.ActiveScenarios.Take(3))
        {
            switch (scenario)
            {
                case "cut_lifestyle":
                    currentBurn -= input.BurnBreakdown.Discretionary * 0.70m;
                    break;
                case "side_hustle":
                    bonusDays += 6;
                    break;
                case "salary_increase":
                    bonusDays += 15;
                    break;
                case "major_upgrade":
                    currentBurn += 15000m;
                    break;
            }
        }

        var stackedDays = Math.Max(0, ComputeSurvivalDays(input.LiquidSavings, currentBurn) + bonusDays);

        return new StackedResult
        {
            ActiveScenarios = input.ActiveScenarios.Take(3).ToList(),
            SurvivalDays = stackedDays,
            DeltaDays = stackedDays - baselineDays,
        };
    }

    private static int ComputeSurvivalDays(decimal liquidSavings, decimal monthlyBurn)
    {
        if (monthlyBurn <= 0) return 9999;
        var days = (int)Math.Floor((double)(liquidSavings / monthlyBurn) * 30);
        return Math.Max(0, days);
    }

    private static string ClassifyZone(int days) => days switch
    {
        < 30 => "critical",
        < 60 => "fragile",
        < 120 => "stable",
        _ => "strong",
    };

    private static string FormatHumanLabel(int days)
    {
        var months = days / 30;
        var remainingDays = days % 30;
        if (months == 0) return $"That's about {remainingDays} days";
        if (remainingDays == 0) return $"That's about {months} month{(months > 1 ? "s" : "")}";
        return $"That's about {months} month{(months > 1 ? "s" : "")} and {remainingDays} day{(remainingDays > 1 ? "s" : "")}";
    }
}
```

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Services/ISurvivalSimulator.cs Hackathon.ApiService/Features/Runway/Services/SurvivalSimulator.cs
git commit -m "feat: implement SP2 Survival & Scenario Simulator"
```

---

## Task 4: Create SP1 Models

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Models/Sp1Models.cs`

**Step 1: Write the SP1 DTOs**

Match PRD Section 8 schemas exactly.

```csharp
namespace Hackathon.ApiService.Features.Runway.Models;

// === SP1 Input ===

public class Sp1Input
{
    public string CsvContent { get; set; } = string.Empty;
    public decimal MonthlyIncome { get; set; }
    public int MonthsCovered { get; set; } = 4;
}

// === SP1 Output ===

public class Sp1Output
{
    public decimal MonthlyBurn { get; set; }
    public BurnBreakdown BurnBreakdown { get; set; } = new();
    public double ElasticityScore { get; set; }
    public double IncomeToBurnRatio { get; set; }
    public List<DangerSignal> DangerSignals { get; set; } = [];
    public string TopDangerCategory { get; set; } = string.Empty;
}

public class DangerSignal
{
    public string Category { get; set; } = string.Empty;
    public double MonthlyGrowthRate { get; set; }
    public decimal MonthlyAmount { get; set; }
    public string Insight { get; set; } = string.Empty;
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Models/Sp1Models.cs
git commit -m "feat: add SP1 Transaction Intelligence DTOs"
```

---

## Task 5: Implement SP1 — Transaction Intelligence Engine

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Services/ITransactionIntelligence.cs`
- Create: `Hackathon.ApiService/Features/Runway/Services/TransactionIntelligence.cs`

**Step 1: Write the SP1 interface**

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface ITransactionIntelligence
{
    Task<Sp1Output> AnalyzeAsync(Sp1Input input, CancellationToken ct);
}
```

**Step 2: Write the SP1 implementation**

Uses OpenAI to categorize transactions. Falls back to hardcoded Alex demo data on failure.

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class TransactionIntelligence : ITransactionIntelligence
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<TransactionIntelligence> _logger;

    public TransactionIntelligence(HttpClient httpClient, IConfiguration configuration, ILogger<TransactionIntelligence> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<Sp1Output> AnalyzeAsync(Sp1Input input, CancellationToken ct)
    {
        try
        {
            return await CallOpenAiAsync(input, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SP1 OpenAI call failed, using fallback demo data");
            return GetFallbackOutput();
        }
    }

    private async Task<Sp1Output> CallOpenAiAsync(Sp1Input input, CancellationToken ct)
    {
        var systemMessage = @"You are a financial transaction analyst. Given a bank statement CSV and monthly income, analyze the transactions and return a structured JSON burn profile.

Return ONLY valid JSON matching this exact schema:
{
  ""monthly_burn"": <decimal, average total monthly spending>,
  ""burn_breakdown"": {
    ""fixed"": <decimal, monthly fixed costs like rent, utilities, subscriptions, loan payments>,
    ""variable"": <decimal, monthly variable costs like groceries, transport, healthcare>,
    ""discretionary"": <decimal, monthly lifestyle costs like food delivery, shopping, coffee>
  },
  ""elasticity_score"": <double, (variable + discretionary) / monthly_burn>,
  ""income_to_burn_ratio"": <double, monthly_income / monthly_burn>,
  ""danger_signals"": [
    {
      ""category"": ""<string, merchant category name>"",
      ""monthly_growth_rate"": <double, month-over-month growth rate as decimal e.g. 0.38 for 38%>,
      ""monthly_amount"": <decimal, latest month amount>,
      ""insight"": ""<string, one sentence insight about the trend using actual numbers>""
    }
  ],
  ""top_danger_category"": ""<string, category name of the highest growth signal>""
}

Rules:
- Categorize each transaction as Fixed, Variable, or Discretionary
- Fixed: rent, utilities, subscriptions, loan payments, government deductions
- Variable: groceries, transport, healthcare
- Discretionary: food delivery, online shopping, coffee shops, entertainment
- Identify the top 2 spending categories with the highest month-over-month growth
- Use actual numbers from the data, never generic
- monthly_burn must equal fixed + variable + discretionary";

        var userMessage = $@"Monthly Income: {input.MonthlyIncome:N0}
Months Covered: {input.MonthsCovered}

Bank Statement CSV:
```
{input.CsvContent}
```

Analyze these transactions and return the JSON burn profile.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage },
            },
            response_format = new { type = "json_object" },
            temperature = 0.2,
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("SP1 OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("OpenAI returned empty content");

        return ParseOpenAiResponse(messageContent);
    }

    private static Sp1Output ParseOpenAiResponse(string jsonContent)
    {
        using var doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        var breakdown = root.GetProperty("burn_breakdown");
        var burnBreakdown = new BurnBreakdown
        {
            Fixed = breakdown.GetProperty("fixed").GetDecimal(),
            Variable = breakdown.GetProperty("variable").GetDecimal(),
            Discretionary = breakdown.GetProperty("discretionary").GetDecimal(),
        };

        var dangerSignals = new List<DangerSignal>();
        foreach (var signal in root.GetProperty("danger_signals").EnumerateArray())
        {
            dangerSignals.Add(new DangerSignal
            {
                Category = signal.GetProperty("category").GetString() ?? "",
                MonthlyGrowthRate = signal.GetProperty("monthly_growth_rate").GetDouble(),
                MonthlyAmount = signal.GetProperty("monthly_amount").GetDecimal(),
                Insight = signal.GetProperty("insight").GetString() ?? "",
            });
        }

        return new Sp1Output
        {
            MonthlyBurn = root.GetProperty("monthly_burn").GetDecimal(),
            BurnBreakdown = burnBreakdown,
            ElasticityScore = root.GetProperty("elasticity_score").GetDouble(),
            IncomeToBurnRatio = root.GetProperty("income_to_burn_ratio").GetDouble(),
            DangerSignals = dangerSignals,
            TopDangerCategory = root.GetProperty("top_danger_category").GetString() ?? "",
        };
    }

    private static Sp1Output GetFallbackOutput() => new()
    {
        MonthlyBurn = 52400m,
        BurnBreakdown = new BurnBreakdown
        {
            Fixed = 28000m,
            Variable = 14200m,
            Discretionary = 10200m,
        },
        ElasticityScore = 0.47,
        IncomeToBurnRatio = 1.43,
        DangerSignals =
        [
            new DangerSignal
            {
                Category = "Grab Food & Dining",
                MonthlyGrowthRate = 0.38,
                MonthlyAmount = 6800m,
                Insight = "Your Grab Food orders grew 38% in 4 months. That's an extra \u20b12,100 a month you weren't spending before.",
            },
            new DangerSignal
            {
                Category = "Shopee & Online Shopping",
                MonthlyGrowthRate = 0.22,
                MonthlyAmount = 3400m,
                Insight = "Online shopping charges grew 22% month over month \u2014 consistent, every single month, for 4 months straight.",
            },
        ],
        TopDangerCategory = "Dining",
    };
}
```

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Services/ITransactionIntelligence.cs Hackathon.ApiService/Features/Runway/Services/TransactionIntelligence.cs
git commit -m "feat: implement SP1 Transaction Intelligence Engine with OpenAI"
```

---

## Task 6: Create SP3 Models

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Models/Sp3Models.cs`

**Step 1: Write the SP3 DTOs**

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Models;

// === SP3 Input ===

public class Sp3Input
{
    public string Archetype { get; set; } = string.Empty;
    public double ElasticityScore { get; set; }
    public double IncomeToBurnRatio { get; set; }
    public List<DangerSignal> DangerSignals { get; set; } = [];
    public int BaselineSurvivalDays { get; set; }
    public ScenarioResult TopScenario { get; set; } = new();
    public BurnBreakdown BurnBreakdown { get; set; } = new();
    public decimal MonthlyBurn { get; set; }
}

// === SP3 Output ===

public class Sp3Output
{
    public string Archetype { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string TopRecommendation { get; set; } = string.Empty;
    public string ClosingLine { get; set; } = string.Empty;
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Models/Sp3Models.cs
git commit -m "feat: add SP3 Behavioral Intelligence DTOs"
```

---

## Task 7: Implement SP3 — Behavioral Intelligence

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Services/IBehavioralIntelligence.cs`
- Create: `Hackathon.ApiService/Features/Runway/Services/BehavioralIntelligence.cs`

**Step 1: Write the SP3 interface**

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface IBehavioralIntelligence
{
    Task<Sp3Output> DiagnoseAsync(Sp3Input input, CancellationToken ct);
}
```

**Step 2: Write the SP3 implementation**

Deterministic archetype classification + OpenAI diagnosis. Exact prompt contract from PRD Section 10.

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class BehavioralIntelligence : IBehavioralIntelligence
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<BehavioralIntelligence> _logger;

    private static readonly Dictionary<string, string> FallbackDiagnoses = new()
    {
        ["Lifestyle Inflator"] = "Your lifestyle spend has been growing faster than your income for months. The gap is small now \u2014 but it compounds. One focused cut adds weeks to your runway.",
        ["Stability Builder"] = "You're doing the hard thing right. Low discretionary spend and a healthy income buffer puts you in the top tier of financial resilience.",
        ["Spending Accelerator"] = "Your fastest-growing expense category is accelerating every month. If the trend holds, it will materially reduce your runway within 2 months.",
        ["Balanced Spender"] = "Your spending is well-distributed across categories with no single runaway pattern. Small optimizations in variable costs would push you into Strong territory.",
    };

    public BehavioralIntelligence(HttpClient httpClient, IConfiguration configuration, ILogger<BehavioralIntelligence> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public static string ClassifyArchetype(double elasticityScore, double incomeToBurnRatio, List<DangerSignal> dangerSignals)
    {
        if (elasticityScore > 0.50 && incomeToBurnRatio < 1.20)
            return "Lifestyle Inflator";
        if (elasticityScore < 0.30 && incomeToBurnRatio > 1.50)
            return "Stability Builder";
        if (dangerSignals.Count > 0 && dangerSignals[0].MonthlyGrowthRate > 0.30)
            return "Spending Accelerator";
        return "Balanced Spender";
    }

    public async Task<Sp3Output> DiagnoseAsync(Sp3Input input, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(8));
            return await CallOpenAiAsync(input, cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SP3 OpenAI call failed, using fallback for archetype: {Archetype}", input.Archetype);
            return GetFallback(input.Archetype);
        }
    }

    private async Task<Sp3Output> CallOpenAiAsync(Sp3Input input, CancellationToken ct)
    {
        var systemMessage = @"You are a financial behavioral analyst.
You will receive a user's financial profile and a pre-classified archetype. Your job is to:

1. Write 2-3 sentences explaining WHY this archetype fits, using specific numbers from the data.
2. Write one clear, specific recommendation - the single highest-impact action this person can take.
3. End with one short sentence that is direct and slightly provocative.

Rules:
- Use the actual numbers. Never be generic.
- Do not soften the diagnosis.
- Sound like a smart friend, not a bank.
- Total response must be under 120 words.
- Return valid JSON only. No explanation outside the JSON.
- JSON schema: { ""archetype"": ""string"", ""diagnosis"": ""string"", ""top_recommendation"": ""string"", ""closing_line"": ""string"" }";

        var topDanger = input.DangerSignals.FirstOrDefault();
        var userMessage = $@"Archetype: {input.Archetype}

Financial Profile:
- Survival days: {input.BaselineSurvivalDays}
- Monthly burn: \u20b1{input.MonthlyBurn:N0}
- Fixed: \u20b1{input.BurnBreakdown.Fixed:N0} | Variable: \u20b1{input.BurnBreakdown.Variable:N0} | Lifestyle: \u20b1{input.BurnBreakdown.Discretionary:N0}
- Elasticity score: {input.ElasticityScore:F2}
  (percentage of burn that is cuttable)
- Fastest growing category: {topDanger?.Category ?? "N/A"}
  at {(topDanger?.MonthlyGrowthRate ?? 0) * 100:F0}% monthly growth
- Highest-impact scenario: {input.TopScenario.Label}
  adds {input.TopScenario.DeltaDays} survival days

Generate the behavioral diagnosis.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage },
            },
            response_format = new { type = "json_object" },
            temperature = 0.4,
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("SP3 OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("OpenAI returned empty content");

        var result = JsonSerializer.Deserialize<Sp3Output>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("Failed to deserialize SP3 response");

        return result;
    }

    private static Sp3Output GetFallback(string archetype)
    {
        var diagnosis = FallbackDiagnoses.GetValueOrDefault(archetype, FallbackDiagnoses["Balanced Spender"]);
        return new Sp3Output
        {
            Archetype = archetype,
            Diagnosis = diagnosis,
            TopRecommendation = "Review your highest-growth spending category and set a monthly cap.",
            ClosingLine = "The best time to fix this was last month. The second best time is now.",
        };
    }
}
```

**Step 3: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Services/IBehavioralIntelligence.cs Hackathon.ApiService/Features/Runway/Services/BehavioralIntelligence.cs
git commit -m "feat: implement SP3 Behavioral Intelligence with archetype classification"
```

---

## Task 8: Create Orchestrator Models + Implementation

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/Models/OrchestratorModels.cs`
- Create: `Hackathon.ApiService/Features/Runway/Services/IRunwayOrchestrator.cs`
- Create: `Hackathon.ApiService/Features/Runway/Services/RunwayOrchestrator.cs`

**Step 1: Write the orchestrator DTOs**

```csharp
namespace Hackathon.ApiService.Features.Runway.Models;

// === Analyze Request (from frontend) ===

public class RunwayAnalyzeRequest
{
    public IFormFile? CsvFile { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal LiquidSavings { get; set; }
    public bool UseDemoData { get; set; }
}

// === Analyze Response (to frontend) ===

public class RunwayAnalyzeResponse
{
    public Sp1Output Sp1 { get; set; } = new();
    public Sp2Output Sp2 { get; set; } = new();
}

// === Scenarios Request (from frontend, on toggle) ===

public class RunwayScenariosRequest
{
    public decimal MonthlyBurn { get; set; }
    public BurnBreakdown BurnBreakdown { get; set; } = new();
    public decimal MonthlyIncome { get; set; }
    public decimal LiquidSavings { get; set; }
    public string PriorityScenario { get; set; } = "cut_lifestyle";
    public List<string> ActiveScenarios { get; set; } = [];
}

// === Profile Request (from frontend, on CTA) ===

public class RunwayProfileRequest
{
    public double ElasticityScore { get; set; }
    public double IncomeToBurnRatio { get; set; }
    public List<DangerSignal> DangerSignals { get; set; } = [];
    public int BaselineSurvivalDays { get; set; }
    public ScenarioResult TopScenario { get; set; } = new();
    public BurnBreakdown BurnBreakdown { get; set; } = new();
    public decimal MonthlyBurn { get; set; }
}
```

**Step 2: Write the orchestrator interface**

```csharp
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface IRunwayOrchestrator
{
    Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct);
    Sp2Output RecalculateScenarios(RunwayScenariosRequest request);
    Task<Sp3Output> RevealProfileAsync(RunwayProfileRequest request, CancellationToken ct);
}
```

**Step 3: Write the orchestrator implementation**

Contains the routing decision. Loads demo CSV from embedded resource.

```csharp
using System.Reflection;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class RunwayOrchestrator : IRunwayOrchestrator
{
    private readonly ITransactionIntelligence _sp1;
    private readonly ISurvivalSimulator _sp2;
    private readonly IBehavioralIntelligence _sp3;
    private readonly ILogger<RunwayOrchestrator> _logger;

    public RunwayOrchestrator(
        ITransactionIntelligence sp1,
        ISurvivalSimulator sp2,
        IBehavioralIntelligence sp3,
        ILogger<RunwayOrchestrator> logger)
    {
        _sp1 = sp1;
        _sp2 = sp2;
        _sp3 = sp3;
        _logger = logger;
    }

    public async Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // Step 1: Get CSV content
        string csvContent;
        if (request.UseDemoData || request.CsvFile is null)
        {
            csvContent = LoadDemoCsv();
        }
        else
        {
            using var reader = new StreamReader(request.CsvFile.OpenReadStream());
            csvContent = await reader.ReadToEndAsync(ct);
        }

        // Step 2: Run SP1
        _logger.LogInformation("Running SP1 Transaction Intelligence...");
        var sp1Result = await _sp1.AnalyzeAsync(new Sp1Input
        {
            CsvContent = csvContent,
            MonthlyIncome = request.MonthlyIncome,
            MonthsCovered = 4,
        }, ct);

        // Step 3: Routing decision
        var priorityScenario = Route(sp1Result.TopDangerCategory, sp1Result.IncomeToBurnRatio);
        _logger.LogInformation("Routing decision: top_danger={Category}, priority={Scenario}",
            sp1Result.TopDangerCategory, priorityScenario);

        // Step 4: Run SP2
        _logger.LogInformation("Running SP2 Survival Simulator...");
        var sp2Result = _sp2.Calculate(new Sp2Input
        {
            MonthlyBurn = sp1Result.MonthlyBurn,
            BurnBreakdown = sp1Result.BurnBreakdown,
            MonthlyIncome = request.MonthlyIncome,
            LiquidSavings = request.LiquidSavings,
            PriorityScenario = priorityScenario,
            ActiveScenarios = [],
        });

        return new RunwayAnalyzeResponse
        {
            Sp1 = sp1Result,
            Sp2 = sp2Result,
        };
    }

    public Sp2Output RecalculateScenarios(RunwayScenariosRequest request)
    {
        return _sp2.Calculate(new Sp2Input
        {
            MonthlyBurn = request.MonthlyBurn,
            BurnBreakdown = request.BurnBreakdown,
            MonthlyIncome = request.MonthlyIncome,
            LiquidSavings = request.LiquidSavings,
            PriorityScenario = request.PriorityScenario,
            ActiveScenarios = request.ActiveScenarios,
        });
    }

    public async Task<Sp3Output> RevealProfileAsync(RunwayProfileRequest request, CancellationToken ct)
    {
        var archetype = BehavioralIntelligence.ClassifyArchetype(
            request.ElasticityScore,
            request.IncomeToBurnRatio,
            request.DangerSignals);

        _logger.LogInformation("SP3 archetype classified: {Archetype}", archetype);

        return await _sp3.DiagnoseAsync(new Sp3Input
        {
            Archetype = archetype,
            ElasticityScore = request.ElasticityScore,
            IncomeToBurnRatio = request.IncomeToBurnRatio,
            DangerSignals = request.DangerSignals,
            BaselineSurvivalDays = request.BaselineSurvivalDays,
            TopScenario = request.TopScenario,
            BurnBreakdown = request.BurnBreakdown,
            MonthlyBurn = request.MonthlyBurn,
        }, ct);
    }

    private static string Route(string topDangerCategory, double incomeToBurnRatio)
    {
        if (new[] { "Dining", "Food", "Entertainment", "Shopping", "Transport", "Ride-share" }
            .Contains(topDangerCategory, StringComparer.OrdinalIgnoreCase))
            return "cut_lifestyle";
        if (incomeToBurnRatio < 1.10)
            return "side_hustle";
        return "cut_lifestyle";
    }

    private static string LoadDemoCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("AlexTransactions.csv"))
            ?? throw new InvalidOperationException("Demo CSV not found as embedded resource");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
```

**Step 4: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/Models/OrchestratorModels.cs Hackathon.ApiService/Features/Runway/Services/IRunwayOrchestrator.cs Hackathon.ApiService/Features/Runway/Services/RunwayOrchestrator.cs
git commit -m "feat: implement Runway Orchestrator with data-driven routing"
```

---

## Task 9: Create API Endpoints

**Files:**
- Create: `Hackathon.ApiService/Features/Runway/AnalyzeEndpoint.cs`
- Create: `Hackathon.ApiService/Features/Runway/ScenariosEndpoint.cs`
- Create: `Hackathon.ApiService/Features/Runway/ProfileEndpoint.cs`

**Step 1: Write the Analyze endpoint (orchestrator entry point)**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class AnalyzeEndpoint : Endpoint<RunwayAnalyzeRequest, RunwayAnalyzeResponse>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public AnalyzeEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/analyze");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayAnalyzeRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.AnalyzeAsync(req, ct);
        await SendOkAsync(result, ct);
    }
}
```

**Step 2: Write the Scenarios endpoint (SP2 re-run)**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class ScenariosEndpoint : Endpoint<RunwayScenariosRequest, Sp2Output>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public ScenariosEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/scenarios");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayScenariosRequest req, CancellationToken ct)
    {
        var result = _orchestrator.RecalculateScenarios(req);
        await SendOkAsync(result, ct);
    }
}
```

**Step 3: Write the Profile endpoint (SP3 trigger)**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class ProfileEndpoint : Endpoint<RunwayProfileRequest, Sp3Output>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public ProfileEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/profile");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayProfileRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.RevealProfileAsync(req, ct);
        await SendOkAsync(result, ct);
    }
}
```

**Step 4: Commit**

```bash
git add Hackathon.ApiService/Features/Runway/AnalyzeEndpoint.cs Hackathon.ApiService/Features/Runway/ScenariosEndpoint.cs Hackathon.ApiService/Features/Runway/ProfileEndpoint.cs
git commit -m "feat: add Runway API endpoints (analyze, scenarios, profile)"
```

---

## Task 10: Register Services in Program.cs + Add CORS

**Files:**
- Modify: `Hackathon.ApiService/Program.cs`

**Step 1: Register all Runway services and add CORS for frontend**

Add these lines after the existing `AddHttpClient<IOpenAiService, OpenAiService>()` block:

```csharp
// Runway Superpowers services
builder.Services.AddHttpClient<ITransactionIntelligence, TransactionIntelligence>()
    .RemoveAllResilienceHandlers()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromMinutes(3));
builder.Services.AddHttpClient<IBehavioralIntelligence, BehavioralIntelligence>()
    .RemoveAllResilienceHandlers()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddScoped<ISurvivalSimulator, SurvivalSimulator>();
builder.Services.AddScoped<IRunwayOrchestrator, RunwayOrchestrator>();
```

Add the correct `using` statements at the top:

```csharp
using Hackathon.ApiService.Features.Runway.Services;
```

Add CORS for the Vue frontend (before `var app = builder.Build();`):

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

And use it after `var app = builder.Build();`:

```csharp
app.UseCors("AllowFrontend");
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Program.cs
git commit -m "feat: register Runway services and add CORS for Vue frontend"
```

---

## Task 11: Verify Backend Builds and Runs

**Step 1: Build the solution**

Run: `dotnet build Hackathon.slnx`

Expected: Build succeeds with 0 errors.

**Step 2: Quick smoke test**

Run: `dotnet run --project Hackathon.ApiService`

Verify the service starts and the Swagger UI shows the new `/api/v1/runway/*` endpoints.

**Step 3: Commit any fixes**

If build errors occur, fix them and commit.

---

## Task 12: Scaffold Vue Frontend

**Files:**
- Create: `Hackathon.Frontend/` (entire Vue project)

**Step 1: Create Vite + Vue 3 + TypeScript project**

```bash
cd "/c/Users/Richard Losande/source/repos/Hackathon"
npm create vite@latest Hackathon.Frontend -- --template vue-ts
```

**Step 2: Install dependencies**

```bash
cd Hackathon.Frontend
npm install
npm install design-system-next pinia axios
npm install -D tailwindcss @tailwindcss/vite
```

**Step 3: Configure Tailwind CSS**

Create `Hackathon.Frontend/src/style.css`:

```css
@import "tailwindcss";
@import "design-system-next/style.css";
```

Update `Hackathon.Frontend/vite.config.ts`:

```ts
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  server: {
    port: 5173,
  },
})
```

**Step 4: Configure main.ts**

Replace `Hackathon.Frontend/src/main.ts`:

```ts
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import SproutDesignSystem from 'design-system-next'
import './style.css'
import App from './App.vue'

const app = createApp(App)
app.use(createPinia())
app.use(SproutDesignSystem)
app.mount('#app')
```

**Step 5: Commit**

```bash
cd "/c/Users/Richard Losande/source/repos/Hackathon"
git add Hackathon.Frontend/package.json Hackathon.Frontend/package-lock.json Hackathon.Frontend/tsconfig.json Hackathon.Frontend/tsconfig.app.json Hackathon.Frontend/tsconfig.node.json Hackathon.Frontend/vite.config.ts Hackathon.Frontend/index.html Hackathon.Frontend/src/main.ts Hackathon.Frontend/src/style.css Hackathon.Frontend/src/vite-env.d.ts Hackathon.Frontend/env.d.ts
git commit -m "feat: scaffold Vue 3 + Vite frontend with Sprout Design System"
```

---

## Task 13: Create API Client + TypeScript Types

**Files:**
- Create: `Hackathon.Frontend/src/api/types.ts`
- Create: `Hackathon.Frontend/src/api/client.ts`

**Step 1: Write TypeScript types matching backend DTOs**

```ts
// src/api/types.ts

// === SP1 ===
export interface DangerSignal {
  category: string
  monthlyGrowthRate: number
  monthlyAmount: number
  insight: string
}

export interface BurnBreakdown {
  fixed: number
  variable: number
  discretionary: number
}

export interface Sp1Output {
  monthlyBurn: number
  burnBreakdown: BurnBreakdown
  elasticityScore: number
  incomeToBurnRatio: number
  dangerSignals: DangerSignal[]
  topDangerCategory: string
}

// === SP2 ===
export interface BaselineResult {
  survivalDays: number
  humanLabel: string
  monthlyBurn: number
  monthlySurplus: number
  burnRate: number
  stabilityZone: string
}

export interface ScenarioResult {
  id: string
  label: string
  survivalDays: number
  deltaDays: number
  isPriority: boolean
}

export interface StackedResult {
  activeScenarios: string[]
  survivalDays: number
  deltaDays: number
}

export interface Sp2Output {
  baseline: BaselineResult
  scenarios: ScenarioResult[]
  stackedResult: StackedResult
}

// === SP3 ===
export interface Sp3Output {
  archetype: string
  diagnosis: string
  topRecommendation: string
  closingLine: string
}

// === API Responses ===
export interface AnalyzeResponse {
  sp1: Sp1Output
  sp2: Sp2Output
}
```

**Step 2: Write API client**

```ts
// src/api/client.ts
import axios from 'axios'
import type { AnalyzeResponse, Sp2Output, Sp3Output, BurnBreakdown, DangerSignal, ScenarioResult } from './types'

const api = axios.create({
  baseURL: 'http://localhost:5000',
  headers: { 'Content-Type': 'application/json' },
})

export async function analyzeRunway(
  monthlyIncome: number,
  liquidSavings: number,
  csvFile: File | null,
  useDemoData: boolean,
): Promise<AnalyzeResponse> {
  const formData = new FormData()
  formData.append('monthlyIncome', monthlyIncome.toString())
  formData.append('liquidSavings', liquidSavings.toString())
  formData.append('useDemoData', useDemoData.toString())
  if (csvFile) {
    formData.append('csvFile', csvFile)
  }

  const { data } = await api.post<AnalyzeResponse>('/api/v1/runway/analyze', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data
}

export async function recalculateScenarios(
  monthlyBurn: number,
  burnBreakdown: BurnBreakdown,
  monthlyIncome: number,
  liquidSavings: number,
  priorityScenario: string,
  activeScenarios: string[],
): Promise<Sp2Output> {
  const { data } = await api.post<Sp2Output>('/api/v1/runway/scenarios', {
    monthlyBurn,
    burnBreakdown,
    monthlyIncome,
    liquidSavings,
    priorityScenario,
    activeScenarios,
  })
  return data
}

export async function revealProfile(
  elasticityScore: number,
  incomeToBurnRatio: number,
  dangerSignals: DangerSignal[],
  baselineSurvivalDays: number,
  topScenario: ScenarioResult,
  burnBreakdown: BurnBreakdown,
  monthlyBurn: number,
): Promise<Sp3Output> {
  const { data } = await api.post<Sp3Output>('/api/v1/runway/profile', {
    elasticityScore,
    incomeToBurnRatio,
    dangerSignals,
    baselineSurvivalDays,
    topScenario,
    burnBreakdown,
    monthlyBurn,
  })
  return data
}
```

**Step 3: Commit**

```bash
git add Hackathon.Frontend/src/api/types.ts Hackathon.Frontend/src/api/client.ts
git commit -m "feat: add API client and TypeScript types for Runway endpoints"
```

---

## Task 14: Create Pinia Store

**Files:**
- Create: `Hackathon.Frontend/src/stores/runway.ts`

**Step 1: Write the Pinia store**

```ts
// src/stores/runway.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Sp1Output, Sp2Output, Sp3Output } from '../api/types'
import { analyzeRunway, recalculateScenarios, revealProfile } from '../api/client'

export const useRunwayStore = defineStore('runway', () => {
  // State
  const currentScreen = ref(1)
  const monthlyIncome = ref(75000)
  const liquidSavings = ref(180000)
  const csvFile = ref<File | null>(null)
  const useDemoData = ref(true)

  const sp1Result = ref<Sp1Output | null>(null)
  const sp2Result = ref<Sp2Output | null>(null)
  const sp3Result = ref<Sp3Output | null>(null)

  const activeScenarios = ref<string[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const priorityScenario = computed(() =>
    sp2Result.value?.scenarios.find(s => s.isPriority)?.id ?? 'cut_lifestyle'
  )

  // Actions
  async function analyze() {
    isLoading.value = true
    error.value = null
    currentScreen.value = 2 // Show processing screen

    try {
      const result = await analyzeRunway(
        monthlyIncome.value,
        liquidSavings.value,
        csvFile.value,
        useDemoData.value,
      )
      sp1Result.value = result.sp1
      sp2Result.value = result.sp2
      activeScenarios.value = []
      currentScreen.value = 3 // Show intelligence report
    } catch (e: any) {
      error.value = e.message || 'Analysis failed'
      currentScreen.value = 1 // Go back to input
    } finally {
      isLoading.value = false
    }
  }

  async function toggleScenario(scenarioId: string) {
    if (!sp1Result.value || !sp2Result.value) return

    const index = activeScenarios.value.indexOf(scenarioId)
    if (index >= 0) {
      activeScenarios.value.splice(index, 1)
    } else {
      if (activeScenarios.value.length >= 3) return // Max 3
      activeScenarios.value.push(scenarioId)
    }

    try {
      sp2Result.value = await recalculateScenarios(
        sp1Result.value.monthlyBurn,
        sp1Result.value.burnBreakdown,
        monthlyIncome.value,
        liquidSavings.value,
        priorityScenario.value,
        activeScenarios.value,
      )
    } catch (e: any) {
      error.value = e.message || 'Scenario calculation failed'
    }
  }

  async function revealMyProfile() {
    if (!sp1Result.value || !sp2Result.value) return

    isLoading.value = true
    error.value = null

    try {
      const topScenario = sp2Result.value.scenarios[0]
      sp3Result.value = await revealProfile(
        sp1Result.value.elasticityScore,
        sp1Result.value.incomeToBurnRatio,
        sp1Result.value.dangerSignals,
        sp2Result.value.baseline.survivalDays,
        topScenario,
        sp1Result.value.burnBreakdown,
        sp1Result.value.monthlyBurn,
      )
      currentScreen.value = 5
    } catch (e: any) {
      error.value = e.message || 'Profile analysis failed'
    } finally {
      isLoading.value = false
    }
  }

  function goToScreen(screen: number) {
    currentScreen.value = screen
  }

  function restart() {
    currentScreen.value = 1
    sp1Result.value = null
    sp2Result.value = null
    sp3Result.value = null
    activeScenarios.value = []
    error.value = null
    liquidSavings.value = 180000
  }

  return {
    currentScreen,
    monthlyIncome,
    liquidSavings,
    csvFile,
    useDemoData,
    sp1Result,
    sp2Result,
    sp3Result,
    activeScenarios,
    isLoading,
    error,
    priorityScenario,
    analyze,
    toggleScenario,
    revealMyProfile,
    goToScreen,
    restart,
  }
})
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/stores/runway.ts
git commit -m "feat: add Pinia store for Runway state management"
```

---

## Task 15: Build Screen 1 — Input

**Files:**
- Create: `Hackathon.Frontend/src/components/InputScreen.vue`

**Step 1: Write Screen 1 component**

Uses Sprout `spr-input`, `spr-input-currency`, `spr-file-upload`, `spr-button`, and `spr-card`.

```vue
<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8 spr-background-color">
    <div class="w-full max-w-lg">
      <h1 class="spr-heading-lg spr-text-color-strong mb-2">Runway</h1>
      <p class="spr-body-md-regular spr-text-color-base mb-8">
        Find out how long your savings would last — and what moves that number.
      </p>

      <spr-card class="p-6 mb-6">
        <div class="flex flex-col gap-6">
          <!-- Monthly Income (read-only) -->
          <spr-input
            :model-value="store.monthlyIncome.toLocaleString()"
            label="Monthly Net Income"
            disabled
          />

          <!-- Fixed Deductions Summary -->
          <div class="spr-body-sm-regular spr-text-color-base p-3 spr-background-color-surface spr-rounded-border-radius-md">
            <p class="spr-body-sm-bold mb-1">Fixed Deductions</p>
            <p>SSS / PhilHealth / Pag-IBIG: ₱8,500</p>
            <p>Sprout Salary Loan: ₱5,000</p>
          </div>

          <!-- Liquid Savings -->
          <spr-input
            v-model="savingsInput"
            label="Liquid Savings (₱)"
            placeholder="Enter your total savings"
            type="number"
          />

          <!-- CSV Upload -->
          <div>
            <label class="spr-body-sm-bold spr-text-color-strong block mb-2">Transaction History</label>
            <div class="flex gap-3">
              <spr-button
                variant="secondary"
                @click="store.useDemoData = true; store.csvFile = null"
                :class="{ 'spr-bg-kangkong-100': store.useDemoData }"
              >
                Use Demo Data
              </spr-button>
              <input
                type="file"
                ref="fileInput"
                accept=".csv"
                class="hidden"
                @change="onFileSelected"
              />
              <spr-button
                variant="tertiary"
                @click="($refs.fileInput as HTMLInputElement)?.click()"
              >
                Upload CSV
              </spr-button>
            </div>
            <p v-if="store.csvFile" class="spr-body-sm-regular spr-text-color-base mt-2">
              {{ store.csvFile.name }}
            </p>
          </div>

          <!-- Privacy Note -->
          <p class="spr-body-sm-regular spr-text-color-base italic">
            Processed on your device. Nothing stored.
          </p>

          <!-- CTA -->
          <spr-button
            tone="success"
            size="large"
            @click="onSubmit"
            :disabled="!isValid"
            class="w-full"
          >
            Show Me My Runway →
          </spr-button>
        </div>
      </spr-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const fileInput = ref<HTMLInputElement>()

const savingsInput = computed({
  get: () => store.liquidSavings.toString(),
  set: (val: string) => { store.liquidSavings = Number(val) || 0 },
})

const isValid = computed(() => store.liquidSavings > 0)

function onFileSelected(event: Event) {
  const target = event.target as HTMLInputElement
  if (target.files?.[0]) {
    store.csvFile = target.files[0]
    store.useDemoData = false
  }
}

function onSubmit() {
  store.analyze()
}
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/InputScreen.vue
git commit -m "feat: add Screen 1 Input form with Sprout components"
```

---

## Task 16: Build Screen 2 — Processing

**Files:**
- Create: `Hackathon.Frontend/src/components/ProcessingScreen.vue`

**Step 1: Write the processing animation screen**

```vue
<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8 spr-background-color">
    <div class="w-full max-w-md text-center">
      <h2 class="spr-heading-md spr-text-color-strong mb-8">Analyzing your transactions...</h2>

      <div class="text-left space-y-3">
        <div
          v-for="(line, index) in visibleLines"
          :key="index"
          class="flex items-center gap-2 spr-body-sm-regular"
        >
          <span class="spr-text-kangkong-600">{{ line.icon }}</span>
          <span :class="line.bold ? 'spr-body-sm-bold' : 'spr-text-color-base'">
            {{ line.text }}
          </span>
        </div>
      </div>

      <div v-if="visibleLines.length < feedLines.length" class="mt-6">
        <spr-progress-bar :value="progress" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()

const feedLines = [
  { icon: '✓', text: '847 transactions found', bold: true },
  { icon: '✓', text: 'Removing duplicates', bold: false },
  { icon: '⟳', text: 'Categorizing your spend...', bold: true },
  { icon: '  ', text: 'Grab Food → Dining', bold: false },
  { icon: '  ', text: 'Shopee → Shopping', bold: false },
  { icon: '  ', text: 'Meralco → Utilities', bold: false },
  { icon: '  ', text: 'Netflix → Subscriptions', bold: false },
  { icon: '  ', text: 'Mercury Drug → Healthcare', bold: false },
  { icon: '✓', text: 'Building burn profile...', bold: true },
  { icon: '✓', text: 'Detecting danger signals...', bold: true },
]

const visibleCount = ref(0)
const visibleLines = computed(() => feedLines.slice(0, visibleCount.value))
const progress = computed(() => Math.round((visibleCount.value / feedLines.length) * 100))

let timer: ReturnType<typeof setInterval>

onMounted(() => {
  timer = setInterval(() => {
    if (visibleCount.value < feedLines.length) {
      visibleCount.value++
    }
  }, 400)
})

onUnmounted(() => {
  clearInterval(timer)
})
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/ProcessingScreen.vue
git commit -m "feat: add Screen 2 Processing animation"
```

---

## Task 17: Build Screen 3 — Intelligence Report

**Files:**
- Create: `Hackathon.Frontend/src/components/IntelligenceReport.vue`

**Step 1: Write the intelligence report screen**

```vue
<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8 spr-background-color">
    <div class="w-full max-w-lg">
      <h2 class="spr-heading-md spr-text-color-strong mb-6">Here's what we found</h2>

      <!-- Burn Breakdown Bar -->
      <spr-card class="p-6 mb-6">
        <p class="spr-body-sm-bold spr-text-color-strong mb-4">Monthly Burn Breakdown</p>
        <div class="flex h-8 spr-rounded-border-radius-md overflow-hidden mb-4">
          <div
            :style="{ width: fixedPct + '%' }"
            class="spr-bg-blueberry-500 flex items-center justify-center spr-body-sm-bold text-white"
          >
            {{ fixedPct }}%
          </div>
          <div
            :style="{ width: variablePct + '%' }"
            class="spr-bg-kangkong-500 flex items-center justify-center spr-body-sm-bold text-white"
          >
            {{ variablePct }}%
          </div>
          <div
            :style="{ width: discretionaryPct + '%' }"
            class="spr-bg-tomato-500 flex items-center justify-center spr-body-sm-bold text-white"
          >
            {{ discretionaryPct }}%
          </div>
        </div>
        <div class="flex justify-between spr-body-sm-regular spr-text-color-base">
          <span><span class="inline-block w-3 h-3 spr-bg-blueberry-500 spr-rounded-border-radius-sm mr-1"></span>Fixed ₱{{ sp1.burnBreakdown.fixed.toLocaleString() }}</span>
          <span><span class="inline-block w-3 h-3 spr-bg-kangkong-500 spr-rounded-border-radius-sm mr-1"></span>Variable ₱{{ sp1.burnBreakdown.variable.toLocaleString() }}</span>
          <span><span class="inline-block w-3 h-3 spr-bg-tomato-500 spr-rounded-border-radius-sm mr-1"></span>Lifestyle ₱{{ sp1.burnBreakdown.discretionary.toLocaleString() }}</span>
        </div>
        <p class="spr-heading-sm spr-text-color-strong mt-4 text-center">
          Total: ₱{{ sp1.monthlyBurn.toLocaleString() }}/month
        </p>
      </spr-card>

      <!-- Danger Signals -->
      <div class="space-y-4 mb-6">
        <spr-card
          v-for="signal in sp1.dangerSignals"
          :key="signal.category"
          class="p-4 border-l-4"
          style="border-left-color: #F97316;"
        >
          <p class="spr-body-sm-bold spr-text-color-strong">{{ signal.category }}</p>
          <p class="spr-body-sm-regular spr-text-color-base mt-1">{{ signal.insight }}</p>
          <div class="flex gap-4 mt-2 spr-body-sm-regular">
            <span class="spr-text-tomato-600">↑ {{ (signal.monthlyGrowthRate * 100).toFixed(0) }}% growth</span>
            <span class="spr-text-color-base">₱{{ signal.monthlyAmount.toLocaleString() }}/mo</span>
          </div>
        </spr-card>
      </div>

      <!-- CTA -->
      <spr-button
        tone="success"
        size="large"
        @click="store.goToScreen(4)"
        class="w-full"
      >
        See My Survival Days →
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp1 = computed(() => store.sp1Result!)

const fixedPct = computed(() => Math.round((sp1.value.burnBreakdown.fixed / sp1.value.monthlyBurn) * 100))
const variablePct = computed(() => Math.round((sp1.value.burnBreakdown.variable / sp1.value.monthlyBurn) * 100))
const discretionaryPct = computed(() => 100 - fixedPct.value - variablePct.value)
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/IntelligenceReport.vue
git commit -m "feat: add Screen 3 Intelligence Report with burn breakdown"
```

---

## Task 18: Build Screen 4 — Survival Dashboard

**Files:**
- Create: `Hackathon.Frontend/src/components/SurvivalDashboard.vue`

**Step 1: Write the survival dashboard screen**

This is the most complex screen with scenario toggles and live updates.

```vue
<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8 spr-background-color">
    <div class="w-full max-w-lg">
      <!-- Survival Days Hero -->
      <div class="text-center mb-8">
        <p class="spr-body-sm-regular spr-text-color-base mb-2">Your financial runway</p>
        <p class="text-7xl font-bold spr-text-color-strong">{{ displayDays }}</p>
        <p class="spr-body-md-regular spr-text-color-base mt-2">days</p>
        <p class="spr-body-sm-regular spr-text-color-base mt-1">{{ sp2.baseline.humanLabel }}</p>
      </div>

      <!-- Stability Zone Badge -->
      <div class="flex justify-center mb-6">
        <spr-badge :tone="zoneTone">{{ zoneLabel }}</spr-badge>
      </div>

      <!-- Zone Legend -->
      <div class="flex justify-center gap-3 mb-8 flex-wrap spr-body-sm-regular">
        <span><span class="inline-block w-2 h-2 rounded-full mr-1" style="background: #F43F5E;"></span>Critical &lt;30</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1" style="background: #F97316;"></span>Fragile 30-59</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1" style="background: #EAB308;"></span>Stable 60-119</span>
        <span><span class="inline-block w-2 h-2 rounded-full mr-1" style="background: #10B981;"></span>Strong 120+</span>
      </div>

      <!-- Scenario Toggle Cards -->
      <div class="space-y-3 mb-6">
        <spr-card
          v-for="scenario in sp2.scenarios"
          :key="scenario.id"
          class="p-4 flex items-center justify-between cursor-pointer"
          @click="onToggle(scenario.id)"
        >
          <div>
            <p class="spr-body-sm-bold spr-text-color-strong">
              {{ scenario.label }}
              <spr-lozenge v-if="scenario.isPriority" tone="information" class="ml-2">Recommended</spr-lozenge>
            </p>
            <p class="spr-body-sm-regular mt-1" :class="scenario.deltaDays >= 0 ? 'spr-text-kangkong-600' : 'spr-text-tomato-600'">
              {{ scenario.deltaDays >= 0 ? '+' : '' }}{{ scenario.deltaDays }} days
            </p>
          </div>
          <spr-switch
            :model-value="store.activeScenarios.includes(scenario.id)"
            @update:model-value="onToggle(scenario.id)"
          />
        </spr-card>
      </div>

      <!-- Max Warning -->
      <p
        v-if="store.activeScenarios.length >= 3"
        class="spr-body-sm-regular spr-text-color-base text-center mb-4"
      >
        You've hit the max — deselect one to try another
      </p>

      <!-- Stacked Result -->
      <div v-if="store.activeScenarios.length > 0" class="text-center mb-6 p-4 spr-background-color-surface spr-rounded-border-radius-md">
        <p class="spr-body-sm-regular spr-text-color-base">With selected scenarios</p>
        <p class="text-3xl font-bold spr-text-color-strong">{{ sp2.stackedResult.survivalDays }} days</p>
        <p class="spr-body-sm-regular" :class="sp2.stackedResult.deltaDays >= 0 ? 'spr-text-kangkong-600' : 'spr-text-tomato-600'">
          {{ sp2.stackedResult.deltaDays >= 0 ? '+' : '' }}{{ sp2.stackedResult.deltaDays }} from baseline
        </p>
      </div>

      <!-- CTA -->
      <spr-button
        tone="success"
        size="large"
        @click="store.revealMyProfile()"
        :disabled="store.isLoading"
        class="w-full"
      >
        {{ store.isLoading ? 'Analyzing...' : 'Reveal My Profile →' }}
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp2 = computed(() => store.sp2Result!)

const displayDays = computed(() =>
  store.activeScenarios.length > 0
    ? sp2.value.stackedResult.survivalDays
    : sp2.value.baseline.survivalDays
)

const currentZone = computed(() => {
  const days = displayDays.value
  if (days < 30) return 'critical'
  if (days < 60) return 'fragile'
  if (days < 120) return 'stable'
  return 'strong'
})

const zoneTone = computed(() => {
  switch (currentZone.value) {
    case 'critical': return 'danger'
    case 'fragile': return 'caution'
    case 'stable': return 'caution'
    case 'strong': return 'success'
    default: return 'information'
  }
})

const zoneLabel = computed(() => {
  switch (currentZone.value) {
    case 'critical': return 'One paycheck away'
    case 'fragile': return 'Worth paying attention to'
    case 'stable': return 'Doing okay — room to improve'
    case 'strong': return "You're in good shape"
    default: return ''
  }
})

function onToggle(scenarioId: string) {
  store.toggleScenario(scenarioId)
}
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/SurvivalDashboard.vue
git commit -m "feat: add Screen 4 Survival Dashboard with scenario toggles"
```

---

## Task 19: Build Screen 5 — Behavioral Diagnosis

**Files:**
- Create: `Hackathon.Frontend/src/components/DiagnosisScreen.vue`

**Step 1: Write the diagnosis screen**

```vue
<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8 spr-background-color">
    <div class="w-full max-w-lg">
      <!-- Archetype -->
      <div class="text-center mb-8">
        <p class="spr-body-sm-regular spr-text-color-base mb-2">Your financial archetype</p>
        <h1 class="spr-heading-lg spr-text-color-strong">{{ sp3.archetype }}</h1>
      </div>

      <!-- Diagnosis -->
      <spr-card class="p-6 mb-6">
        <p class="spr-body-md-regular spr-text-color-strong leading-relaxed">
          {{ sp3.diagnosis }}
        </p>
      </spr-card>

      <!-- Top Recommendation -->
      <spr-card class="p-6 mb-6 spr-background-color-surface" style="border-left: 4px solid #10B981;">
        <p class="spr-body-sm-bold spr-text-kangkong-700 mb-2">Top Recommendation</p>
        <p class="spr-body-md-regular spr-text-color-strong">
          {{ sp3.topRecommendation }}
        </p>
      </spr-card>

      <!-- Closing Line -->
      <p class="spr-body-sm-regular spr-text-color-base italic text-center mb-8">
        {{ sp3.closingLine }}
      </p>

      <!-- Restart -->
      <spr-button
        variant="secondary"
        size="large"
        @click="store.restart()"
        class="w-full"
      >
        ← Try Different Numbers
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const sp3 = computed(() => store.sp3Result!)
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/components/DiagnosisScreen.vue
git commit -m "feat: add Screen 5 Behavioral Diagnosis display"
```

---

## Task 20: Wire Up App.vue with Screen Navigation

**Files:**
- Modify: `Hackathon.Frontend/src/App.vue`

**Step 1: Replace App.vue with screen router**

```vue
<template>
  <div class="min-h-screen spr-background-color">
    <InputScreen v-if="store.currentScreen === 1" />
    <ProcessingScreen v-else-if="store.currentScreen === 2" />
    <IntelligenceReport v-else-if="store.currentScreen === 3" />
    <SurvivalDashboard v-else-if="store.currentScreen === 4" />
    <DiagnosisScreen v-else-if="store.currentScreen === 5" />
  </div>
</template>

<script setup lang="ts">
import { useRunwayStore } from './stores/runway'
import InputScreen from './components/InputScreen.vue'
import ProcessingScreen from './components/ProcessingScreen.vue'
import IntelligenceReport from './components/IntelligenceReport.vue'
import SurvivalDashboard from './components/SurvivalDashboard.vue'
import DiagnosisScreen from './components/DiagnosisScreen.vue'

const store = useRunwayStore()
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/App.vue
git commit -m "feat: wire up App.vue with 5-screen navigation flow"
```

---

## Task 21: Verify Frontend Builds and Runs

**Step 1: Build the frontend**

```bash
cd Hackathon.Frontend
npm run build
```

Expected: Build succeeds with 0 errors.

**Step 2: Run the frontend**

```bash
npm run dev
```

Expected: Vite dev server starts on http://localhost:5173.

**Step 3: Fix any build errors and commit**

If TypeScript or build errors occur, fix and commit.

---

## Task 22: End-to-End Smoke Test

**Step 1: Start both services**

Terminal 1:
```bash
dotnet run --project Hackathon.ApiService
```

Terminal 2:
```bash
cd Hackathon.Frontend && npm run dev
```

**Step 2: Verify the happy path**

1. Open http://localhost:5173
2. Screen 1: Click "Use Demo Data", enter savings, click "Show Me My Runway →"
3. Screen 2: Processing animation should play
4. Screen 3: Burn breakdown and danger signals should display
5. Screen 4: Survival days number, toggle scenarios, verify max 3 enforcement
6. Screen 5: Click "Reveal My Profile →", verify archetype and diagnosis appear

**Step 3: Fix any issues and commit**

Fix any API connection issues (CORS, port mismatch, JSON serialization), and commit all fixes.

---

## Task 23: Final Cleanup

**Step 1: Remove old FinancialRunway code (optional)**

The old `Features/FinancialRunway/` folder can be removed since the new `Features/Runway/` replaces it. Only do this if the old endpoints are confirmed unnecessary.

**Step 2: Update API base URL**

Check `Hackathon.Frontend/src/api/client.ts` — the `baseURL` should match the actual ApiService port. Check `Hackathon.ApiService/Properties/launchSettings.json` for the correct port.

**Step 3: Final commit**

```bash
git add -A
git commit -m "chore: final cleanup and integration fixes"
```
