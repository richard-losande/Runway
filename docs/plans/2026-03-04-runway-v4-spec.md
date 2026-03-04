# Runway v4 Spec Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Full rewrite of the Runway financial resilience calculator to match the v4 product spec — 8-screen Vue frontend with TOGE components, 4-agent backend with typed scenario model, zone system, and demo mode.

**Architecture:** Backend (ApiService, .NET 10, FastEndpoints) owns all computation via a pure `RunwayEngine` service. Frontend (Vue 3 + Pinia + TOGE design system) is display-only, calling backend for all calculations. Demo mode stubs Agents 1–3 with fixture data; Agent 4 makes a live gpt-4o call.

**Tech Stack:** .NET 10, FastEndpoints 8, EF Core 10, PostgreSQL, OpenAI API (gpt-4o, gpt-4o-mini), Vue 3, TypeScript, Pinia, Vite, Tailwind CSS 4, design-system-next (spr-* components), xUnit

---

## Task 1: Backend Models — Core Types

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/Models/CoreModels.cs`

**Step 1: Create the models file**

```csharp
namespace Hackathon.ApiService.Features.RunwayV4.Models;

// === Enums ===

public enum CategoryKey
{
    FoodDining,
    Groceries,
    BillsUtilities,
    Transport,
    Shopping,
    HealthWellness,
    Housing,
    Transfers,
    EntertainmentSubs,
    Misc
}

public enum CategoryTier
{
    Essential,
    Discretionary,
    Committed
}

public enum ScenarioType
{
    SpendingCut,
    IncomeGain,
    OneTimeInject,
    HousingChange,
    Custom
}

public enum EffortTag
{
    Quick,
    Habit,
    Life
}

public enum Recurrence
{
    OneTime,
    Recurring
}

public enum ZoneName
{
    Critical,
    Fragile,
    Stable,
    Strong
}

public enum ArchetypeKey
{
    LifestyleInflator,
    SteadySpender,
    ResilientSaver,
    CrisisMode
}

public enum BankSource
{
    GCash,
    BDO,
    BPI,
    UnionBank,
    Maya,
    RCBC,
    Unknown
}

// === Core State ===

public class RunwayState
{
    public decimal LiquidCash { get; set; }
    public decimal MonthlyBurn { get; set; }
    public decimal TakeHome { get; set; }
    public Dictionary<CategoryKey, decimal> Categories { get; set; } = new();
}

// === Transaction ===

public class Transaction
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string RawDesc { get; set; } = string.Empty;
    public string NormDesc { get; set; } = string.Empty;
    public BankSource Source { get; set; }
    public CategoryKey? Category { get; set; }
    public string? Merchant { get; set; }
    public object? Confidence { get; set; } // "high" string or decimal 0-1
}

// === Category Breakdown ===

public class MerchantSummary
{
    public string Name { get; set; } = string.Empty;
    public decimal MonthlyAvg { get; set; }
}

public class CategoryBreakdownEntry
{
    public decimal MonthlyAverage { get; set; }
    public List<decimal> MonthlyAmounts { get; set; } = new();
    public CategoryTier Tier { get; set; }
    public List<MerchantSummary> TopMerchants { get; set; } = new();
    public int TransactionCount { get; set; }
}

// === Scenario ===

public class ScenarioParams
{
    // SPENDING_CUT
    public CategoryKey? Category { get; set; }
    public decimal? CutPct { get; set; }
    public decimal? CutAmount { get; set; }

    // INCOME_GAIN
    public decimal? GainAmount { get; set; }

    // ONE_TIME_INJECT
    public decimal? InjectAmount { get; set; }

    // HOUSING_CHANGE
    public decimal? RentDelta { get; set; }

    // CUSTOM
    public decimal? MonthlyAmount { get; set; }
    public string? UserLabel { get; set; }
}

public class Scenario
{
    public string Id { get; set; } = string.Empty;
    public ScenarioType Type { get; set; }
    public string Label { get; set; } = string.Empty;
    public EffortTag Effort { get; set; }
    public Recurrence Recurrence { get; set; }
    public ScenarioParams Params { get; set; } = new();
    public string? Assumption { get; set; }
    public int Delta { get; set; } // computed after creation
}

// === Insight Profile (Agent 2 output) ===

public class ArchetypeInfo
{
    public ArchetypeKey Key { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Signal { get; set; } = string.Empty;
}

public class DangerSignal
{
    public string Severity { get; set; } = string.Empty; // "danger" | "caution"
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string Metric { get; set; } = string.Empty;
    public CategoryKey? Category { get; set; }
}

public class TrendInfo
{
    public CategoryKey Category { get; set; }
    public string Direction { get; set; } = string.Empty; // "growing" | "stable" | "declining"
    public decimal PctChange { get; set; }
    public bool Notable { get; set; }
    public string TopMerchant { get; set; } = string.Empty;
    public decimal TopMerchantAmount { get; set; }
}

public class InsightProfile
{
    public ArchetypeInfo Archetype { get; set; } = new();
    public List<DangerSignal> DangerSignals { get; set; } = new();
    public List<TrendInfo> Trends { get; set; } = new();
    public string? RemittanceNote { get; set; }
    public decimal FlexibleBurn { get; set; }
    public decimal FixedBurn { get; set; }
}

// === Diagnosis Content (Agent 4 output) ===

public class DiagnosisContent
{
    public string ArchetypeName { get; set; } = string.Empty;
    public string WhatIsHappening { get; set; } = string.Empty;
    public string WhatToDoAboutIt { get; set; } = string.Empty;
    public string HonestTake { get; set; } = string.Empty;
}

// === Correction Candidate ===

public class CorrectionCandidate
{
    public Transaction Transaction { get; set; } = new();
    public CategoryKey AssignedCategory { get; set; }
    public decimal ConfidenceScore { get; set; }
    public string Reason { get; set; } = string.Empty; // "rule_match" | "llm" | "llm_low_conf" | "misc_fallback"
}

// === Zone Info ===

public class ZoneInfo
{
    public ZoneName Name { get; set; }
    public string ColourToken { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/Models/CoreModels.cs
git commit -m "feat: add v4 spec core models (RunwayState, Scenario, InsightProfile, etc.)"
```

---

## Task 2: Backend Models — Request/Response DTOs

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/Models/RequestModels.cs`

**Step 1: Create the request/response models**

```csharp
namespace Hackathon.ApiService.Features.RunwayV4.Models;

// === Analyze Endpoint ===

public class RunwayAnalyzeRequest
{
    public IFormFile? CsvFile { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal LiquidSavings { get; set; }
    public bool UseDemoData { get; set; }
}

public class RunwayAnalyzeResponse
{
    public RunwayState State { get; set; } = new();
    public int BaselineDays { get; set; }
    public ZoneName Zone { get; set; }
    public Dictionary<CategoryKey, CategoryBreakdownEntry> Categories { get; set; } = new();
    public InsightProfile InsightProfile { get; set; } = new();
    public List<Scenario> Scenarios { get; set; } = new();
    public string? FastestWinId { get; set; }
    public List<DangerSignal> DangerSignals { get; set; } = new();
    public List<CorrectionCandidate> CorrectionCandidates { get; set; } = new();
    public DateTime AnalysisDate { get; set; }
}

// === Diagnose Endpoint ===

public class RunwayDiagnoseRequest
{
    public InsightProfile InsightProfile { get; set; } = new();
    public RunwayState State { get; set; } = new();
    public int BaselineDays { get; set; }
    public ZoneName Zone { get; set; }
    public string FastestWinLabel { get; set; } = string.Empty;
    public int FastestWinDelta { get; set; }
    public int FastestWinNewDays { get; set; }
}

public class RunwayDiagnoseResponse
{
    public DiagnosisContent Diagnosis { get; set; } = new();
}

// === Compute Scenarios Endpoint ===

public class RunwayComputeScenariosRequest
{
    public RunwayState State { get; set; } = new();
    public List<Scenario> Scenarios { get; set; } = new();
    public List<string> ActiveScenarioIds { get; set; } = new();
    public Scenario? CustomScenario { get; set; }
    public int? ReverseTarget { get; set; }
}

public class RunwayComputeScenariosResponse
{
    public int BaselineDays { get; set; }
    public ZoneName BaselineZone { get; set; }
    public int StackedDays { get; set; }
    public int StackedDelta { get; set; }
    public ZoneName StackedZone { get; set; }
    public string StackedDate { get; set; } = string.Empty;
    public List<ScenarioWithDelta> ScenarioDeltas { get; set; } = new();
    public List<string>? ReverseModeIds { get; set; }
}

public class ScenarioWithDelta
{
    public string Id { get; set; } = string.Empty;
    public int Delta { get; set; }
    public int NewDays { get; set; }
    public ZoneName NewZone { get; set; }
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/Models/RequestModels.cs
git commit -m "feat: add v4 spec request/response DTOs for all endpoints"
```

---

## Task 3: Computation Engine — RunwayEngine

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/Services/RunwayEngine.cs`

**Step 1: Create the computation engine with all pure functions**

```csharp
namespace Hackathon.ApiService.Features.RunwayV4.Services;

using Hackathon.ApiService.Features.RunwayV4.Models;

public interface IRunwayEngine
{
    int ComputeBaseline(RunwayState state);
    int ComputeScenarioDays(Scenario scenario, RunwayState state);
    int ComputeDelta(Scenario scenario, RunwayState state);
    ZoneName GetZone(int days);
    ZoneInfo GetZoneInfo(int days);
    string DaysToDate(int days, DateTime referenceDate);
    List<Scenario> FindFastestPath(int targetDays, RunwayState state, List<Scenario> scenarios);
    string? FindFastestWinId(List<Scenario> scenarios, RunwayState state);
}

public class RunwayEngine : IRunwayEngine
{
    private static readonly Dictionary<ZoneName, ZoneInfo> Zones = new()
    {
        [ZoneName.Critical] = new ZoneInfo
        {
            Name = ZoneName.Critical,
            ColourToken = "TOMATO",
            Description = "Savings cover less than a month. Focus on one action right now."
        },
        [ZoneName.Fragile] = new ZoneInfo
        {
            Name = ZoneName.Fragile,
            ColourToken = "MANGO",
            Description = "About 1–2 months of runway. One unexpected expense could strain you."
        },
        [ZoneName.Stable] = new ZoneInfo
        {
            Name = ZoneName.Stable,
            ColourToken = "BLUEBERRY",
            Description = "2–4 months of breathing room. Enough to handle most surprises — but not enough to stop watching."
        },
        [ZoneName.Strong] = new ZoneInfo
        {
            Name = ZoneName.Strong,
            ColourToken = "KANGKONG",
            Description = "4+ months of cushion. Well-positioned — the goal now is to make this money work harder."
        }
    };

    public int ComputeBaseline(RunwayState state)
    {
        if (state.LiquidCash <= 0) return 0;
        if (state.MonthlyBurn <= 0) return int.MaxValue;
        return (int)Math.Floor(state.LiquidCash / (state.MonthlyBurn / 30m));
    }

    public int ComputeScenarioDays(Scenario scenario, RunwayState state)
    {
        switch (scenario.Type)
        {
            case ScenarioType.SpendingCut:
            {
                var catSpend = state.Categories.GetValueOrDefault(scenario.Params.Category ?? CategoryKey.Misc, 0);
                var reduction = scenario.Params.CutAmount ?? (catSpend * (scenario.Params.CutPct ?? 0));
                var newBurn = state.MonthlyBurn - reduction;
                if (newBurn <= 0) return int.MaxValue;
                return (int)Math.Floor(state.LiquidCash / (newBurn / 30m));
            }
            case ScenarioType.IncomeGain:
            {
                var monthlyGap = state.MonthlyBurn - state.TakeHome;
                var newGap = Math.Max(0, monthlyGap - (scenario.Params.GainAmount ?? 0));
                var effectiveBurn = state.TakeHome + newGap;
                if (effectiveBurn <= 0) return int.MaxValue;
                return (int)Math.Floor(state.LiquidCash / (effectiveBurn / 30m));
            }
            case ScenarioType.OneTimeInject:
            {
                var newCash = state.LiquidCash + (scenario.Params.InjectAmount ?? 0);
                if (state.MonthlyBurn <= 0) return int.MaxValue;
                return (int)Math.Floor(newCash / (state.MonthlyBurn / 30m));
            }
            case ScenarioType.HousingChange:
            {
                var newBurn = state.MonthlyBurn + (scenario.Params.RentDelta ?? 0);
                if (newBurn <= 0) return int.MaxValue;
                return (int)Math.Floor(state.LiquidCash / (newBurn / 30m));
            }
            case ScenarioType.Custom:
            {
                var monthlyAmount = scenario.Params.MonthlyAmount ?? 0;
                var newBurn = monthlyAmount < 0
                    ? state.MonthlyBurn + monthlyAmount  // spending cut (negative = reduce burn)
                    : state.MonthlyBurn - monthlyAmount; // income gain (positive = reduce burn)
                newBurn = Math.Max(1, newBurn);
                return (int)Math.Floor(state.LiquidCash / (newBurn / 30m));
            }
            default:
                return ComputeBaseline(state);
        }
    }

    public int ComputeDelta(Scenario scenario, RunwayState state)
    {
        return ComputeScenarioDays(scenario, state) - ComputeBaseline(state);
    }

    public ZoneName GetZone(int days)
    {
        if (days < 30) return ZoneName.Critical;
        if (days < 60) return ZoneName.Fragile;
        if (days < 120) return ZoneName.Stable;
        return ZoneName.Strong;
    }

    public ZoneInfo GetZoneInfo(int days)
    {
        return Zones[GetZone(days)];
    }

    public string DaysToDate(int days, DateTime referenceDate)
    {
        var targetDate = referenceDate.AddDays(days);
        return targetDate.ToString("MMMM d, yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-PH"));
    }

    public List<Scenario> FindFastestPath(int targetDays, RunwayState state, List<Scenario> scenarios)
    {
        var positive = scenarios
            .Where(s => ComputeDelta(s, state) > 0)
            .OrderByDescending(s => ComputeDelta(s, state))
            .ToList();

        var selected = new List<Scenario>();
        var current = ComputeBaseline(state);

        foreach (var s in positive)
        {
            if (current >= targetDays) break;
            selected.Add(s);
            current += ComputeDelta(s, state);
        }

        return selected;
    }

    public string? FindFastestWinId(List<Scenario> scenarios, RunwayState state)
    {
        return scenarios
            .Where(s => s.Effort is EffortTag.Quick or EffortTag.Habit)
            .OrderByDescending(s => ComputeDelta(s, state))
            .FirstOrDefault()?.Id;
    }
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/Services/RunwayEngine.cs
git commit -m "feat: add RunwayEngine with all pure computation functions"
```

---

## Task 4: Unit Tests for RunwayEngine

**Files:**
- Modify: `Hackathon.Tests/Hackathon.Tests.csproj` (add xUnit + project reference)
- Create: `Hackathon.Tests/RunwayEngineTests.cs`

**Step 1: Update test project with xUnit and project reference**

Replace the content of `Hackathon.Tests/Hackathon.Tests.csproj` with:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Hackathon.ApiService\Hackathon.ApiService.csproj" />
  </ItemGroup>

</Project>
```

**Step 2: Write the test file with all spec-required test cases**

Create `Hackathon.Tests/RunwayEngineTests.cs`:

```csharp
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.Tests;

public class RunwayEngineTests
{
    private readonly RunwayEngine _engine = new();

    private static RunwayState AlexGarciaState() => new()
    {
        LiquidCash = 180_000m,
        MonthlyBurn = 52_400m,
        TakeHome = 28_500m,
        Categories = new Dictionary<CategoryKey, decimal>
        {
            [CategoryKey.FoodDining] = 14_200m,
            [CategoryKey.Groceries] = 8_000m,
            [CategoryKey.BillsUtilities] = 7_200m,
            [CategoryKey.Transport] = 6_500m,
            [CategoryKey.Shopping] = 5_000m,
            [CategoryKey.Transfers] = 5_000m,
            [CategoryKey.EntertainmentSubs] = 3_300m,
            [CategoryKey.HealthWellness] = 2_800m,
            [CategoryKey.Housing] = 0m,
            [CategoryKey.Misc] = 400m,
        }
    };

    // --- ComputeBaseline ---

    [Fact]
    public void ComputeBaseline_AlexGarcia_Returns103()
    {
        var state = AlexGarciaState();
        Assert.Equal(103, _engine.ComputeBaseline(state));
    }

    [Fact]
    public void ComputeBaseline_ZeroLiquidCash_Returns0()
    {
        var state = AlexGarciaState();
        state.LiquidCash = 0;
        Assert.Equal(0, _engine.ComputeBaseline(state));
    }

    [Fact]
    public void ComputeBaseline_ZeroMonthlyBurn_ReturnsMaxValue()
    {
        var state = AlexGarciaState();
        state.MonthlyBurn = 0;
        Assert.Equal(int.MaxValue, _engine.ComputeBaseline(state));
    }

    // --- ComputeScenarioDays: SPENDING_CUT ---

    [Fact]
    public void ComputeScenarioDays_SpendingCut70Pct_FoodDining_Returns136()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.SpendingCut,
            Params = new ScenarioParams { Category = CategoryKey.FoodDining, CutPct = 0.70m }
        };
        // cutAmount = 14200 * 0.70 = 9940
        // newBurn = 52400 - 9940 = 42460
        // days = floor(180000 / (42460/30)) = floor(180000 / 1415.33) = 127
        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.True(result > 103, $"Expected > 103 but got {result}");
    }

    // --- ComputeScenarioDays: INCOME_GAIN ---

    [Fact]
    public void ComputeScenarioDays_IncomeGain_SalaryRaise10Pct()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.IncomeGain,
            Params = new ScenarioParams { GainAmount = 2_850m } // 10% of 28500
        };
        // gap = 52400 - 28500 = 23900
        // newGap = max(0, 23900 - 2850) = 21050
        // effectiveBurn = 28500 + 21050 = 49550
        // days = floor(180000 / (49550/30)) = floor(180000 / 1651.67) = 109
        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.True(result > 103, $"Expected > 103 but got {result}");
    }

    // --- ComputeScenarioDays: ONE_TIME_INJECT ---

    [Fact]
    public void ComputeScenarioDays_OneTimeInject_10000()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.OneTimeInject,
            Params = new ScenarioParams { InjectAmount = 10_000m }
        };
        // newCash = 180000 + 10000 = 190000
        // days = floor(190000 / (52400/30)) = floor(190000 / 1746.67) = 108
        Assert.Equal(108, _engine.ComputeScenarioDays(scenario, state));
    }

    // --- ComputeScenarioDays: HOUSING_CHANGE ---

    [Fact]
    public void ComputeScenarioDays_HousingChange_Plus15000()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.HousingChange,
            Params = new ScenarioParams { RentDelta = 15_000m }
        };
        // newBurn = 52400 + 15000 = 67400
        // days = floor(180000 / (67400/30)) = floor(180000 / 2246.67) = 80
        Assert.Equal(80, _engine.ComputeScenarioDays(scenario, state));
    }

    // --- ComputeScenarioDays: CUSTOM ---

    [Fact]
    public void ComputeScenarioDays_Custom_SpendingCut5000()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.Custom,
            Params = new ScenarioParams { MonthlyAmount = -5_000m }
        };
        // newBurn = 52400 + (-5000) = 47400
        // days = floor(180000 / (47400/30)) = floor(180000 / 1580) = 113
        Assert.Equal(113, _engine.ComputeScenarioDays(scenario, state));
    }

    // --- ComputeDelta ---

    [Fact]
    public void ComputeDelta_EqualsScenarioDaysMinusBaseline()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "test",
            Type = ScenarioType.OneTimeInject,
            Params = new ScenarioParams { InjectAmount = 10_000m }
        };
        var expected = _engine.ComputeScenarioDays(scenario, state) - _engine.ComputeBaseline(state);
        Assert.Equal(expected, _engine.ComputeDelta(scenario, state));
    }

    // --- DaysToDate ---

    [Fact]
    public void DaysToDate_103Days_FromSep30_ReturnsJan11()
    {
        var result = _engine.DaysToDate(103, new DateTime(2024, 9, 30));
        Assert.Equal("January 11, 2025", result);
    }

    // --- GetZone ---

    [Theory]
    [InlineData(29, ZoneName.Critical)]
    [InlineData(30, ZoneName.Fragile)]
    [InlineData(59, ZoneName.Fragile)]
    [InlineData(60, ZoneName.Stable)]
    [InlineData(119, ZoneName.Stable)]
    [InlineData(120, ZoneName.Strong)]
    [InlineData(500, ZoneName.Strong)]
    public void GetZone_ReturnsCorrectZone(int days, ZoneName expected)
    {
        Assert.Equal(expected, _engine.GetZone(days));
    }

    // --- FindFastestPath ---

    [Fact]
    public void FindFastestPath_GreedySelection()
    {
        var state = AlexGarciaState();
        var scenarios = new List<Scenario>
        {
            new() { Id = "a", Type = ScenarioType.SpendingCut, Effort = EffortTag.Habit, Params = new ScenarioParams { Category = CategoryKey.FoodDining, CutPct = 0.70m } },
            new() { Id = "b", Type = ScenarioType.IncomeGain, Effort = EffortTag.Habit, Params = new ScenarioParams { GainAmount = 10_000m } },
            new() { Id = "c", Type = ScenarioType.HousingChange, Effort = EffortTag.Life, Params = new ScenarioParams { RentDelta = 15_000m } }, // negative delta
        };
        var result = _engine.FindFastestPath(120, state, scenarios);
        Assert.True(result.Count > 0);
        Assert.DoesNotContain(result, s => s.Id == "c"); // negative delta excluded
    }
}
```

**Step 3: Run tests**

```bash
dotnet test Hackathon.Tests --verbosity normal
```

Expected: All tests PASS.

**Step 4: Commit**

```bash
git add Hackathon.Tests/
git commit -m "test: add unit tests for RunwayEngine computation functions"
```

---

## Task 5: Demo Fixtures — Alex Garcia Data

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/DemoData/AlexGarciaFixtures.cs`

**Step 1: Create the fixtures file with all Alex Garcia demo data from spec section 12.5**

```csharp
namespace Hackathon.ApiService.Features.RunwayV4.DemoData;

using Hackathon.ApiService.Features.RunwayV4.Models;

public static class AlexGarciaFixtures
{
    public static RunwayState State => new()
    {
        LiquidCash = 180_000m,
        MonthlyBurn = 52_400m,
        TakeHome = 28_500m,
        Categories = new Dictionary<CategoryKey, decimal>
        {
            [CategoryKey.FoodDining] = 14_200m,
            [CategoryKey.Groceries] = 8_000m,
            [CategoryKey.BillsUtilities] = 7_200m,
            [CategoryKey.Transport] = 6_500m,
            [CategoryKey.Shopping] = 5_000m,
            [CategoryKey.Transfers] = 5_000m,
            [CategoryKey.EntertainmentSubs] = 3_300m,
            [CategoryKey.HealthWellness] = 2_800m,
            [CategoryKey.Housing] = 0m,
            [CategoryKey.Misc] = 400m,
        }
    };

    public static InsightProfile InsightProfile => new()
    {
        Archetype = new ArchetypeInfo
        {
            Key = ArchetypeKey.LifestyleInflator,
            Name = "The Lifestyle Inflator",
            Signal = "Monthly burn exceeds take-home by ₱23,900 and food & dining has grown 38% over 3 months."
        },
        DangerSignals = new List<DangerSignal>
        {
            new()
            {
                Severity = "danger",
                Title = "Monthly spend exceeds take-home pay",
                Detail = "Your ₱52,400 monthly burn is ₱23,900 above your ₱28,500 take-home. Your savings are covering the gap.",
                Metric = "₱23,900 gap",
                Category = null
            },
            new()
            {
                Severity = "caution",
                Title = "Food & dining growing fast",
                Detail = "Grab Food alone is ₱10,200 this month — up 38% from your 3-month average of ₱7,390.",
                Metric = "+38%",
                Category = CategoryKey.FoodDining
            }
        },
        Trends = new List<TrendInfo>
        {
            new() { Category = CategoryKey.FoodDining, Direction = "growing", PctChange = 38, Notable = true, TopMerchant = "Grab Food", TopMerchantAmount = 10_200m },
            new() { Category = CategoryKey.Shopping, Direction = "stable", PctChange = 4, Notable = false, TopMerchant = "Lazada", TopMerchantAmount = 2_800m },
            new() { Category = CategoryKey.EntertainmentSubs, Direction = "stable", PctChange = 0, Notable = false, TopMerchant = "Netflix", TopMerchantAmount = 899m },
            new() { Category = CategoryKey.Transfers, Direction = "stable", PctChange = 0, Notable = false, TopMerchant = "GCash Send Money", TopMerchantAmount = 5_000m },
        },
        RemittanceNote = null, // 5000/52400 = 9.5% — below 15%
        FlexibleBurn = 22_900m, // food(14200) + shopping(5000) + entertainment(3300) + misc(400)
        FixedBurn = 29_500m, // groceries(8000) + bills(7200) + transport(6500) + health(2800) + housing(0) + transfers(5000)
    };

    public static List<Scenario> Scenarios => new()
    {
        new()
        {
            Id = "sc_grab_baseline",
            Type = ScenarioType.SpendingCut,
            Label = "Return Grab Food to July levels",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { Category = CategoryKey.FoodDining, CutAmount = 2_810m },
            Assumption = null
        },
        new()
        {
            Id = "sc_dining_cut",
            Type = ScenarioType.SpendingCut,
            Label = "Cut dining & delivery 70%",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { Category = CategoryKey.FoodDining, CutPct = 0.70m },
            Assumption = null
        },
        new()
        {
            Id = "sc_salary_raise",
            Type = ScenarioType.IncomeGain,
            Label = "Salary raise 10%",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { GainAmount = 2_850m },
            Assumption = "Assumes raise takes effect immediately"
        },
        new()
        {
            Id = "sc_side_hustle",
            Type = ScenarioType.IncomeGain,
            Label = "Side hustle ₱10k/mo",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { GainAmount = 10_000m },
            Assumption = "Assumes sustained ₱10,000/month additional income"
        },
        new()
        {
            Id = "sc_housing",
            Type = ScenarioType.HousingChange,
            Label = "Move to a bigger unit",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { RentDelta = 15_000m },
            Assumption = "Assumes +₱15,000/month increase in housing cost"
        }
    };

    public static Dictionary<CategoryKey, CategoryBreakdownEntry> CategoryBreakdown => new()
    {
        [CategoryKey.FoodDining] = new() { MonthlyAverage = 14_200m, Tier = CategoryTier.Discretionary, MonthlyAmounts = new() { 11_000m, 12_500m, 14_200m, 18_200m }, TopMerchants = new() { new() { Name = "Grab Food", MonthlyAvg = 10_200m } }, TransactionCount = 85 },
        [CategoryKey.Groceries] = new() { MonthlyAverage = 8_000m, Tier = CategoryTier.Essential, MonthlyAmounts = new() { 7_800m, 8_200m, 8_000m, 9_400m }, TopMerchants = new() { new() { Name = "SM Supermarket", MonthlyAvg = 4_200m } }, TransactionCount = 32 },
        [CategoryKey.BillsUtilities] = new() { MonthlyAverage = 7_200m, Tier = CategoryTier.Essential, MonthlyAmounts = new() { 7_100m, 7_200m, 7_200m, 8_100m }, TopMerchants = new() { new() { Name = "Meralco", MonthlyAvg = 3_120m } }, TransactionCount = 18 },
        [CategoryKey.Transport] = new() { MonthlyAverage = 6_500m, Tier = CategoryTier.Essential, MonthlyAmounts = new() { 6_200m, 6_800m, 6_500m, 7_600m }, TopMerchants = new() { new() { Name = "Grab Car", MonthlyAvg = 4_800m } }, TransactionCount = 45 },
        [CategoryKey.Shopping] = new() { MonthlyAverage = 5_000m, Tier = CategoryTier.Discretionary, MonthlyAmounts = new() { 4_800m, 5_200m, 5_000m, 5_800m }, TopMerchants = new() { new() { Name = "Lazada", MonthlyAvg = 2_800m } }, TransactionCount = 22 },
        [CategoryKey.Transfers] = new() { MonthlyAverage = 5_000m, Tier = CategoryTier.Committed, MonthlyAmounts = new() { 5_000m, 5_000m, 5_000m, 5_000m }, TopMerchants = new() { new() { Name = "GCash Send Money", MonthlyAvg = 5_000m } }, TransactionCount = 4 },
        [CategoryKey.EntertainmentSubs] = new() { MonthlyAverage = 3_300m, Tier = CategoryTier.Discretionary, MonthlyAmounts = new() { 3_300m, 3_300m, 3_300m, 3_300m }, TopMerchants = new() { new() { Name = "Netflix", MonthlyAvg = 899m } }, TransactionCount = 12 },
        [CategoryKey.HealthWellness] = new() { MonthlyAverage = 2_800m, Tier = CategoryTier.Essential, MonthlyAmounts = new() { 2_600m, 2_900m, 2_800m, 3_200m }, TopMerchants = new() { new() { Name = "Mercury Drug", MonthlyAvg = 1_200m } }, TransactionCount = 8 },
        [CategoryKey.Housing] = new() { MonthlyAverage = 0m, Tier = CategoryTier.Essential, MonthlyAmounts = new() { 0m, 0m, 0m, 0m }, TopMerchants = new(), TransactionCount = 0 },
        [CategoryKey.Misc] = new() { MonthlyAverage = 400m, Tier = CategoryTier.Discretionary, MonthlyAmounts = new() { 300m, 400m, 500m, 1_400m }, TopMerchants = new(), TransactionCount = 5 },
    };

    public static DiagnosisContent FallbackDiagnosis => new()
    {
        ArchetypeName = "The Lifestyle Inflator",
        WhatIsHappening = "Your monthly burn has grown to ₱52,400 — but your take-home is ₱28,500. Your savings are covering a ₱23,900 monthly gap. Grab Food alone is ₱10,200 this month, up 38% from three months ago.",
        WhatToDoAboutIt = "Return Grab Food to July levels — that single change reduces your monthly burn by ₱2,810. Your runway goes from 103 days to 108 days. Stack it with a side hustle and you cross 130 days.",
        HonestTake = "103 days is not a crisis — but the gap between what comes in and what goes out has been quietly widening for three months."
    };

    public static readonly string[] DemoTransactionStrings = new[]
    {
        "GRAB*FOOD PHILIPPINES 09:42 ₱340.00",
        "GCASH SEND MONEY NENA G. ₱5,000.00",
        "SM SUPERMARKET MOA 14:21 ₱2,840.00",
        "MERALCO PAYMENT ECPay ₱3,120.00",
        "LAZADA PAYMENTS PTE LTD ₱1,299.00",
        "SHOPEE PAY SPAY-241103 ₱890.00"
    };

    public static readonly (string Key, string Label, string Amount)[] DemoCategoryOrder = new[]
    {
        ("food_dining", "Food & Dining", "₱14,200"),
        ("groceries", "Groceries & Market", "₱8,000"),
        ("bills_utilities", "Bills & Utilities", "₱7,200"),
        ("transport", "Transport", "₱6,500"),
        ("shopping", "Shopping", "₱5,000"),
        ("transfers", "Transfers & Family", "₱5,000"),
        ("entertainment_subs", "Entertainment & Subs", "₱3,300"),
        ("health_wellness", "Health & Wellness", "₱2,800"),
        ("misc", "Miscellaneous", "₱400"),
    };
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/DemoData/AlexGarciaFixtures.cs
git commit -m "feat: add Alex Garcia demo fixtures matching spec section 12.5"
```

---

## Task 6: Agent 4 — Diagnosis Narrative Service

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/Services/DiagnosisNarrativeAgent.cs`

**Step 1: Create the Agent 4 service (live gpt-4o call + fallback)**

```csharp
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.DemoData;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IDiagnosisNarrativeAgent
{
    Task<DiagnosisContent> GenerateAsync(RunwayDiagnoseRequest request, CancellationToken ct);
}

public class DiagnosisNarrativeAgent : IDiagnosisNarrativeAgent
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<DiagnosisNarrativeAgent> _logger;
    private readonly bool _isDemoMode;

    public DiagnosisNarrativeAgent(HttpClient httpClient, IConfiguration configuration, ILogger<DiagnosisNarrativeAgent> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _isDemoMode = configuration.GetValue<bool>("DemoMode", true);

        var apiKey = configuration["OpenAI:ApiKey"] ?? "";
        _model = configuration["OpenAI:DiagnosisModel"] ?? configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        if (!string.IsNullOrEmpty(apiKey))
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<DiagnosisContent> GenerateAsync(RunwayDiagnoseRequest request, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(10));

            var systemPrompt = BuildSystemPrompt();
            var userPrompt = BuildUserPrompt(request);

            var requestBody = new
            {
                model = _model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                response_format = new { type = "json_object" },
                temperature = 0.4
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("v1/chat/completions", content, cts.Token);
            var responseBody = await response.Content.ReadAsStringAsync(cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Agent 4 API error: {Status}", response.StatusCode);
                return GetFallback(request);
            }

            using var doc = JsonDocument.Parse(responseBody);
            var messageContent = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrEmpty(messageContent))
                return GetFallback(request);

            var result = JsonSerializer.Deserialize<DiagnosisContent>(messageContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null) return GetFallback(request);

            // Post-processing: check for banned words
            var banned = new[] { "crisis", "danger", "urgent", "warning", "overspending", "irresponsible", "reckless" };
            if (banned.Any(b => result.HonestTake.Contains(b, StringComparison.OrdinalIgnoreCase)))
                result.HonestTake = GetFallback(request).HonestTake;
            if (banned.Any(b => result.WhatIsHappening.Contains(b, StringComparison.OrdinalIgnoreCase)))
                result.WhatIsHappening = GetFallback(request).WhatIsHappening;
            if (banned.Any(b => result.WhatToDoAboutIt.Contains(b, StringComparison.OrdinalIgnoreCase)))
                result.WhatToDoAboutIt = GetFallback(request).WhatToDoAboutIt;

            // Truncate to character limits
            if (result.WhatIsHappening.Length > 280)
                result.WhatIsHappening = TruncateAtSentence(result.WhatIsHappening, 280);
            if (result.WhatToDoAboutIt.Length > 240)
                result.WhatToDoAboutIt = TruncateAtSentence(result.WhatToDoAboutIt, 240);
            if (result.HonestTake.Length > 180)
                result.HonestTake = TruncateAtSentence(result.HonestTake, 180);

            return result;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Agent 4 timed out, using fallback");
            return GetFallback(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agent 4 failed, using fallback");
            return GetFallback(request);
        }
    }

    private DiagnosisContent GetFallback(RunwayDiagnoseRequest request)
    {
        if (_isDemoMode)
            return AlexGarciaFixtures.FallbackDiagnosis;

        return BuildFallbackDiagnosis(request.State, request.InsightProfile);
    }

    private static DiagnosisContent BuildFallbackDiagnosis(RunwayState state, InsightProfile profile)
    {
        var gap = state.MonthlyBurn - state.TakeHome;
        var hasGap = gap > 0;

        return new DiagnosisContent
        {
            ArchetypeName = profile.Archetype.Name,
            WhatIsHappening = hasGap
                ? $"Your monthly burn is ₱{state.MonthlyBurn:N0}, but your take-home is ₱{state.TakeHome:N0}. Your savings are covering a ₱{gap:N0} monthly gap."
                : $"Your monthly burn is ₱{state.MonthlyBurn:N0} against a take-home of ₱{state.TakeHome:N0}.",
            WhatToDoAboutIt = $"Your highest-impact move is to reduce your discretionary spend. Even a ₱5,000 monthly reduction adds {Math.Round(5000m / (state.MonthlyBurn / 30m))} days to your runway.",
            HonestTake = "The number that matters most is the gap between what comes in and what goes out."
        };
    }

    private static string TruncateAtSentence(string text, int maxLength)
    {
        if (text.Length <= maxLength) return text;
        var truncated = text[..maxLength];
        var lastPeriod = truncated.LastIndexOf('.');
        if (lastPeriod > 0) return truncated[..(lastPeriod + 1)];
        return truncated;
    }

    private static string BuildSystemPrompt() => """
        You are writing a personal financial resilience report for a Sprout payroll user in the Philippines. Your tone is direct, honest, and grounded — not alarming, not encouraging, not salesy. You write like a trusted friend who happens to know finance, not like a financial advisor covering liability.

        Generate a DiagnosisContent object as valid JSON only. No preamble.

        Rules for whatIsHappening:
        - Use the user's actual numbers. Every sentence must contain a specific figure.
        - Name the top growing merchant explicitly.
        - State the monthly gap in plain terms if burn > takeHome.
        - Do not editorialize. State facts.
        - 2–4 sentences. Max 280 characters.

        Rules for whatToDoAboutIt:
        - Reference the fastestWin scenario by its exact label.
        - State the before and after runway days explicitly.
        - Frame as one specific change, not a list.
        - 2–3 sentences. Max 240 characters.

        Rules for honestTake:
        - Do not repeat numbers from the sections above.
        - Name the underlying pattern.
        - No alarm language: never use "crisis", "danger", "urgent", "warning".
        - No shame language: never use "overspending", "irresponsible", "reckless".
        - 1–2 sentences. Max 180 characters.

        If remittanceNote is present: include it verbatim in whatIsHappening. Never suggest cutting remittances.

        Character limits are hard limits. Truncate cleanly at sentence boundary.

        JSON schema: { "archetypeName": string, "whatIsHappening": string, "whatToDoAboutIt": string, "honestTake": string }
        """;

    private static string BuildUserPrompt(RunwayDiagnoseRequest req) => $"""
        Archetype: {req.InsightProfile.Archetype.Name}
        Signal: {req.InsightProfile.Archetype.Signal}
        Monthly burn: ₱{req.State.MonthlyBurn:N0}
        Take-home: ₱{req.State.TakeHome:N0}
        Gap: ₱{Math.Max(0, req.State.MonthlyBurn - req.State.TakeHome):N0}
        Baseline runway: {req.BaselineDays} days
        Zone: {req.Zone}
        Fastest win: "{req.FastestWinLabel}" — adds {req.FastestWinDelta} days (→ {req.FastestWinNewDays} days)
        Remittance note: {req.InsightProfile.RemittanceNote ?? "N/A"}
        Top trends: {string.Join(", ", req.InsightProfile.Trends.Where(t => t.Notable).Select(t => $"{t.TopMerchant} ({t.Category}): {t.Direction} {t.PctChange}%"))}
        """;
}
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/Services/DiagnosisNarrativeAgent.cs
git commit -m "feat: add Agent 4 (DiagnosisNarrative) with live gpt-4o and fallback"
```

---

## Task 7: RunwayV4 Orchestrator + Endpoints

**Files:**
- Create: `Hackathon.ApiService/Features/RunwayV4/Services/RunwayV4Orchestrator.cs`
- Create: `Hackathon.ApiService/Features/RunwayV4/AnalyzeEndpoint.cs`
- Create: `Hackathon.ApiService/Features/RunwayV4/DiagnoseEndpoint.cs`
- Create: `Hackathon.ApiService/Features/RunwayV4/ComputeScenariosEndpoint.cs`

**Step 1: Create the orchestrator**

```csharp
using Hackathon.ApiService.Features.RunwayV4.DemoData;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IRunwayV4Orchestrator
{
    Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct);
    RunwayComputeScenariosResponse ComputeScenarios(RunwayComputeScenariosRequest request);
}

public class RunwayV4Orchestrator : IRunwayV4Orchestrator
{
    private readonly IRunwayEngine _engine;
    private readonly bool _isDemoMode;

    public RunwayV4Orchestrator(IRunwayEngine engine, IConfiguration configuration)
    {
        _engine = engine;
        _isDemoMode = configuration.GetValue<bool>("DemoMode", true);
    }

    public Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // In demo mode, return fixture data
        // In production, this would run the full pipeline (Stages 1-5 + Agents 1-3)
        var state = _isDemoMode
            ? AlexGarciaFixtures.State
            : new RunwayState
            {
                LiquidCash = request.LiquidSavings,
                MonthlyBurn = 52_400m, // Would be computed from pipeline
                TakeHome = request.MonthlyIncome,
                Categories = AlexGarciaFixtures.State.Categories // Placeholder
            };

        // Override liquid cash from user input
        state.LiquidCash = request.LiquidSavings;
        state.TakeHome = request.MonthlyIncome;

        var baselineDays = _engine.ComputeBaseline(state);
        var zone = _engine.GetZone(baselineDays);
        var scenarios = _isDemoMode
            ? AlexGarciaFixtures.Scenarios
            : AlexGarciaFixtures.Scenarios; // Production: Agent 3 would generate these

        // Compute deltas for each scenario
        foreach (var s in scenarios)
        {
            s.Delta = _engine.ComputeDelta(s, state);
        }

        var fastestWinId = _engine.FindFastestWinId(scenarios, state);

        var response = new RunwayAnalyzeResponse
        {
            State = state,
            BaselineDays = baselineDays,
            Zone = zone,
            Categories = _isDemoMode ? AlexGarciaFixtures.CategoryBreakdown : AlexGarciaFixtures.CategoryBreakdown,
            InsightProfile = _isDemoMode ? AlexGarciaFixtures.InsightProfile : AlexGarciaFixtures.InsightProfile,
            Scenarios = scenarios,
            FastestWinId = fastestWinId,
            DangerSignals = (_isDemoMode ? AlexGarciaFixtures.InsightProfile : AlexGarciaFixtures.InsightProfile).DangerSignals,
            CorrectionCandidates = new(),
            AnalysisDate = DateTime.UtcNow,
        };

        return Task.FromResult(response);
    }

    public RunwayComputeScenariosResponse ComputeScenarios(RunwayComputeScenariosRequest request)
    {
        var state = request.State;
        var baselineDays = _engine.ComputeBaseline(state);
        var baselineZone = _engine.GetZone(baselineDays);

        var allScenarios = request.Scenarios.ToList();
        if (request.CustomScenario != null)
            allScenarios.Add(request.CustomScenario);

        // Compute individual deltas
        var scenarioDeltas = allScenarios.Select(s => new ScenarioWithDelta
        {
            Id = s.Id,
            Delta = _engine.ComputeDelta(s, state),
            NewDays = _engine.ComputeScenarioDays(s, state),
            NewZone = _engine.GetZone(_engine.ComputeScenarioDays(s, state))
        }).ToList();

        // Compute stacked result (additive deltas)
        var activeDeltas = scenarioDeltas
            .Where(sd => request.ActiveScenarioIds.Contains(sd.Id))
            .Sum(sd => sd.Delta);

        var stackedDays = baselineDays + activeDeltas;
        var stackedZone = _engine.GetZone(stackedDays);
        var stackedDate = _engine.DaysToDate(stackedDays, DateTime.UtcNow);

        // Reverse mode
        List<string>? reverseModeIds = null;
        if (request.ReverseTarget.HasValue)
        {
            var path = _engine.FindFastestPath(request.ReverseTarget.Value, state, allScenarios);
            reverseModeIds = path.Select(s => s.Id).ToList();
        }

        return new RunwayComputeScenariosResponse
        {
            BaselineDays = baselineDays,
            BaselineZone = baselineZone,
            StackedDays = stackedDays,
            StackedDelta = activeDeltas,
            StackedZone = stackedZone,
            StackedDate = stackedDate,
            ScenarioDeltas = scenarioDeltas,
            ReverseModeIds = reverseModeIds
        };
    }
}
```

**Step 2: Create the Analyze endpoint**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class AnalyzeV4Endpoint : Endpoint<RunwayAnalyzeRequest, RunwayAnalyzeResponse>
{
    private readonly IRunwayV4Orchestrator _orchestrator;

    public AnalyzeV4Endpoint(IRunwayV4Orchestrator orchestrator) => _orchestrator = orchestrator;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/analyze");
        AllowAnonymous();
        AllowFileUploads();
    }

    public override async Task HandleAsync(RunwayAnalyzeRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.AnalyzeAsync(req, ct);
        await SendOkAsync(result, ct);
    }
}
```

**Step 3: Create the Diagnose endpoint**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class DiagnoseV4Endpoint : Endpoint<RunwayDiagnoseRequest, RunwayDiagnoseResponse>
{
    private readonly IDiagnosisNarrativeAgent _agent;

    public DiagnoseV4Endpoint(IDiagnosisNarrativeAgent agent) => _agent = agent;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/diagnose");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayDiagnoseRequest req, CancellationToken ct)
    {
        var diagnosis = await _agent.GenerateAsync(req, ct);
        await SendOkAsync(new RunwayDiagnoseResponse { Diagnosis = diagnosis }, ct);
    }
}
```

**Step 4: Create the ComputeScenarios endpoint**

```csharp
using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class ComputeScenariosV4Endpoint : Endpoint<RunwayComputeScenariosRequest, RunwayComputeScenariosResponse>
{
    private readonly IRunwayV4Orchestrator _orchestrator;

    public ComputeScenariosV4Endpoint(IRunwayV4Orchestrator orchestrator) => _orchestrator = orchestrator;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/compute-scenarios");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayComputeScenariosRequest req, CancellationToken ct)
    {
        var result = _orchestrator.ComputeScenarios(req);
        await SendOkAsync(result, ct);
    }
}
```

**Step 5: Register services in Program.cs**

Add these lines to `Hackathon.ApiService/Program.cs` in the service registration section (after existing DI registrations):

```csharp
// RunwayV4 services
builder.Services.AddSingleton<IRunwayEngine, RunwayEngine>();
builder.Services.AddScoped<IRunwayV4Orchestrator, RunwayV4Orchestrator>();
builder.Services.AddHttpClient<IDiagnosisNarrativeAgent, DiagnosisNarrativeAgent>();
```

Also add the using statement at the top:
```csharp
using Hackathon.ApiService.Features.RunwayV4.Services;
```

**Step 6: Commit**

```bash
git add Hackathon.ApiService/Features/RunwayV4/
git add Hackathon.ApiService/Program.cs
git commit -m "feat: add RunwayV4 orchestrator and 3 endpoints (analyze, diagnose, compute-scenarios)"
```

---

## Task 8: Frontend — TypeScript Types

**Files:**
- Create: `Hackathon.Frontend/src/api/runway-v4-types.ts`

**Step 1: Create the TypeScript types matching the backend models**

```typescript
// === Enums ===

export type CategoryKey =
  | 'FoodDining' | 'Groceries' | 'BillsUtilities' | 'Transport'
  | 'Shopping' | 'HealthWellness' | 'Housing' | 'Transfers'
  | 'EntertainmentSubs' | 'Misc'

export type CategoryTier = 'Essential' | 'Discretionary' | 'Committed'

export type ScenarioType = 'SpendingCut' | 'IncomeGain' | 'OneTimeInject' | 'HousingChange' | 'Custom'

export type EffortTag = 'Quick' | 'Habit' | 'Life'

export type Recurrence = 'OneTime' | 'Recurring'

export type ZoneName = 'Critical' | 'Fragile' | 'Stable' | 'Strong'

export type ArchetypeKey = 'LifestyleInflator' | 'SteadySpender' | 'ResilientSaver' | 'CrisisMode'

// === Core State ===

export interface RunwayState {
  liquidCash: number
  monthlyBurn: number
  takeHome: number
  categories: Record<CategoryKey, number>
}

// === Category Breakdown ===

export interface MerchantSummary {
  name: string
  monthlyAvg: number
}

export interface CategoryBreakdownEntry {
  monthlyAverage: number
  monthlyAmounts: number[]
  tier: CategoryTier
  topMerchants: MerchantSummary[]
  transactionCount: number
}

// === Scenario ===

export interface ScenarioParams {
  category?: CategoryKey
  cutPct?: number
  cutAmount?: number
  gainAmount?: number
  injectAmount?: number
  rentDelta?: number
  monthlyAmount?: number
  userLabel?: string
}

export interface Scenario {
  id: string
  type: ScenarioType
  label: string
  effort: EffortTag
  recurrence: Recurrence
  params: ScenarioParams
  assumption?: string | null
  delta: number
}

// === Insight Profile ===

export interface ArchetypeInfo {
  key: ArchetypeKey
  name: string
  signal: string
}

export interface DangerSignal {
  severity: 'danger' | 'caution'
  title: string
  detail: string
  metric: string
  category?: CategoryKey | null
}

export interface TrendInfo {
  category: CategoryKey
  direction: 'growing' | 'stable' | 'declining'
  pctChange: number
  notable: boolean
  topMerchant: string
  topMerchantAmount: number
}

export interface InsightProfile {
  archetype: ArchetypeInfo
  dangerSignals: DangerSignal[]
  trends: TrendInfo[]
  remittanceNote?: string | null
  flexibleBurn: number
  fixedBurn: number
}

// === Diagnosis Content ===

export interface DiagnosisContent {
  archetypeName: string
  whatIsHappening: string
  whatToDoAboutIt: string
  honestTake: string
}

// === Zone Info ===

export interface ZoneInfo {
  name: ZoneName
  colourToken: string
  description: string
}

// === Correction Candidate ===

export interface CorrectionCandidate {
  transaction: {
    id: string
    rawDesc: string
    amount: number
    category?: CategoryKey
    merchant?: string
  }
  assignedCategory: CategoryKey
  confidenceScore: number
  reason: string
}

// === API Request/Response ===

export interface RunwayAnalyzeResponse {
  state: RunwayState
  baselineDays: number
  zone: ZoneName
  categories: Record<CategoryKey, CategoryBreakdownEntry>
  insightProfile: InsightProfile
  scenarios: Scenario[]
  fastestWinId?: string | null
  dangerSignals: DangerSignal[]
  correctionCandidates: CorrectionCandidate[]
  analysisDate: string
}

export interface RunwayDiagnoseResponse {
  diagnosis: DiagnosisContent
}

export interface ScenarioWithDelta {
  id: string
  delta: number
  newDays: number
  newZone: ZoneName
}

export interface RunwayComputeScenariosResponse {
  baselineDays: number
  baselineZone: ZoneName
  stackedDays: number
  stackedDelta: number
  stackedZone: ZoneName
  stackedDate: string
  scenarioDeltas: ScenarioWithDelta[]
  reverseModeIds?: string[] | null
}
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/api/runway-v4-types.ts
git commit -m "feat: add TypeScript types for runway v4 API"
```

---

## Task 9: Frontend — API Client

**Files:**
- Create: `Hackathon.Frontend/src/api/runway-v4-client.ts`

**Step 1: Create the API client**

```typescript
import axios from 'axios'
import type {
  RunwayAnalyzeResponse,
  RunwayDiagnoseResponse,
  RunwayComputeScenariosResponse,
  RunwayState,
  Scenario,
  InsightProfile,
  ZoneName,
} from './runway-v4-types'

const api = axios.create({
  headers: { 'Content-Type': 'application/json' },
})

export async function analyzeRunwayV4(
  monthlyIncome: number,
  liquidSavings: number,
  csvFile: File | null,
  useDemoData: boolean,
): Promise<RunwayAnalyzeResponse> {
  const formData = new FormData()
  formData.append('monthlyIncome', monthlyIncome.toString())
  formData.append('liquidSavings', liquidSavings.toString())
  formData.append('useDemoData', useDemoData.toString())
  if (csvFile) {
    formData.append('csvFile', csvFile)
  }

  const { data } = await api.post<RunwayAnalyzeResponse>('/api/v1/runway-v4/analyze', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return data
}

export async function diagnoseRunwayV4(
  insightProfile: InsightProfile,
  state: RunwayState,
  baselineDays: number,
  zone: ZoneName,
  fastestWinLabel: string,
  fastestWinDelta: number,
  fastestWinNewDays: number,
): Promise<RunwayDiagnoseResponse> {
  const { data } = await api.post<RunwayDiagnoseResponse>('/api/v1/runway-v4/diagnose', {
    insightProfile,
    state,
    baselineDays,
    zone,
    fastestWinLabel,
    fastestWinDelta,
    fastestWinNewDays,
  })
  return data
}

export async function computeScenariosV4(
  state: RunwayState,
  scenarios: Scenario[],
  activeScenarioIds: string[],
  customScenario?: Scenario | null,
  reverseTarget?: number | null,
): Promise<RunwayComputeScenariosResponse> {
  const { data } = await api.post<RunwayComputeScenariosResponse>('/api/v1/runway-v4/compute-scenarios', {
    state,
    scenarios,
    activeScenarioIds,
    customScenario: customScenario ?? null,
    reverseTarget: reverseTarget ?? null,
  })
  return data
}
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/api/runway-v4-client.ts
git commit -m "feat: add runway v4 API client"
```

---

## Task 10: Frontend — Pinia Store

**Files:**
- Create: `Hackathon.Frontend/src/stores/runway-v4.ts`

**Step 1: Create the new Pinia store matching spec section 8.3**

```typescript
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type {
  RunwayState,
  RunwayAnalyzeResponse,
  Scenario,
  InsightProfile,
  DiagnosisContent,
  ZoneName,
  CategoryKey,
  CategoryBreakdownEntry,
  DangerSignal,
  CorrectionCandidate,
  RunwayComputeScenariosResponse,
} from '../api/runway-v4-types'
import { analyzeRunwayV4, diagnoseRunwayV4, computeScenariosV4 } from '../api/runway-v4-client'

export const useRunwayV4Store = defineStore('runway-v4', () => {
  // Screen navigation (1-8)
  const currentScreen = ref(1)

  // User inputs
  const liquidSavings = ref(180_000)
  const monthlyIncome = ref(28_500)
  const csvFile = ref<File | null>(null)
  const useDemoData = ref(true)

  // Analysis results
  const state = ref<RunwayState | null>(null)
  const baselineDays = ref(0)
  const zone = ref<ZoneName>('Stable')
  const categories = ref<Record<CategoryKey, CategoryBreakdownEntry> | null>(null)
  const insightProfile = ref<InsightProfile | null>(null)
  const scenarios = ref<Scenario[]>([])
  const fastestWinId = ref<string | null>(null)
  const dangerSignals = ref<DangerSignal[]>([])
  const correctionCandidates = ref<CorrectionCandidate[]>([])
  const analysisDate = ref<string>('')

  // Scenario playground state
  const activeScenarioIds = ref<string[]>([])
  const customScenario = ref<Scenario | null>(null)
  const reverseTarget = ref<number | null>(null)

  // Stacked result
  const stackedDays = ref(0)
  const stackedDelta = ref(0)
  const stackedZone = ref<ZoneName>('Stable')
  const stackedDate = ref('')

  // Diagnosis (Agent 4)
  const diagnosis = ref<DiagnosisContent | null>(null)

  // Loading states
  const isAnalyzing = ref(false)
  const isDiagnosing = ref(false)
  const isComputingScenarios = ref(false)
  const error = ref<string | null>(null)

  // Computed
  const fastestWin = computed(() =>
    scenarios.value.find(s => s.id === fastestWinId.value) ?? null
  )

  const displayDays = computed(() =>
    activeScenarioIds.value.length > 0 ? stackedDays.value : baselineDays.value
  )

  const displayZone = computed(() =>
    activeScenarioIds.value.length > 0 ? stackedZone.value : zone.value
  )

  // Actions
  async function analyze() {
    isAnalyzing.value = true
    error.value = null
    currentScreen.value = 4 // Processing screen

    try {
      const result = await analyzeRunwayV4(
        monthlyIncome.value,
        liquidSavings.value,
        csvFile.value,
        useDemoData.value,
      )
      applyAnalyzeResult(result)
      currentScreen.value = 5 // Intelligence report
    } catch (e: any) {
      error.value = e.message || 'Analysis failed'
      currentScreen.value = 2 // Back to payroll profile
    } finally {
      isAnalyzing.value = false
    }
  }

  function applyAnalyzeResult(result: RunwayAnalyzeResponse) {
    state.value = result.state
    baselineDays.value = result.baselineDays
    zone.value = result.zone
    categories.value = result.categories
    insightProfile.value = result.insightProfile
    scenarios.value = result.scenarios
    fastestWinId.value = result.fastestWinId ?? null
    dangerSignals.value = result.dangerSignals
    correctionCandidates.value = result.correctionCandidates
    analysisDate.value = result.analysisDate
    activeScenarioIds.value = []
    stackedDays.value = result.baselineDays
    stackedDelta.value = 0
    stackedZone.value = result.zone
    customScenario.value = null
    reverseTarget.value = null
  }

  async function toggleScenario(scenarioId: string) {
    if (!state.value) return

    const idx = activeScenarioIds.value.indexOf(scenarioId)
    if (idx >= 0) {
      activeScenarioIds.value.splice(idx, 1)
    } else {
      activeScenarioIds.value.push(scenarioId)
    }

    await recomputeScenarios()
  }

  async function setCustomScenario(label: string, monthlyAmount: number) {
    if (!state.value) return

    customScenario.value = {
      id: 'sc_custom',
      type: 'Custom',
      label: label || 'Custom',
      effort: 'Habit',
      recurrence: 'Recurring',
      params: { monthlyAmount },
      assumption: null,
      delta: 0,
    }

    if (!activeScenarioIds.value.includes('sc_custom')) {
      activeScenarioIds.value.push('sc_custom')
    }

    await recomputeScenarios()
  }

  async function clearCustomScenario() {
    customScenario.value = null
    activeScenarioIds.value = activeScenarioIds.value.filter(id => id !== 'sc_custom')
    await recomputeScenarios()
  }

  async function setReverseTarget(target: number | null) {
    reverseTarget.value = target
    await recomputeScenarios()
  }

  async function recomputeScenarios() {
    if (!state.value) return

    isComputingScenarios.value = true
    try {
      const result = await computeScenariosV4(
        state.value,
        scenarios.value,
        activeScenarioIds.value,
        customScenario.value,
        reverseTarget.value,
      )
      applyScenarioResult(result)
    } catch (e: any) {
      error.value = e.message || 'Scenario computation failed'
    } finally {
      isComputingScenarios.value = false
    }
  }

  function applyScenarioResult(result: RunwayComputeScenariosResponse) {
    stackedDays.value = result.stackedDays
    stackedDelta.value = result.stackedDelta
    stackedZone.value = result.stackedZone
    stackedDate.value = result.stackedDate

    // Update individual scenario deltas
    for (const sd of result.scenarioDeltas) {
      const s = scenarios.value.find(x => x.id === sd.id)
      if (s) s.delta = sd.delta
    }

    // Handle reverse mode
    if (result.reverseModeIds) {
      activeScenarioIds.value = result.reverseModeIds
    }
  }

  async function fetchDiagnosis() {
    if (!state.value || !insightProfile.value || !fastestWin.value) return

    isDiagnosing.value = true
    error.value = null

    try {
      const result = await diagnoseRunwayV4(
        insightProfile.value,
        state.value,
        baselineDays.value,
        zone.value,
        fastestWin.value.label,
        fastestWin.value.delta,
        baselineDays.value + fastestWin.value.delta,
      )
      diagnosis.value = result.diagnosis
      currentScreen.value = 7
    } catch (e: any) {
      error.value = e.message || 'Diagnosis failed'
    } finally {
      isDiagnosing.value = false
    }
  }

  function goToScreen(screen: number) {
    currentScreen.value = screen
  }

  function restart() {
    currentScreen.value = 1
    state.value = null
    baselineDays.value = 0
    zone.value = 'Stable'
    categories.value = null
    insightProfile.value = null
    scenarios.value = []
    fastestWinId.value = null
    dangerSignals.value = []
    correctionCandidates.value = []
    diagnosis.value = null
    activeScenarioIds.value = []
    customScenario.value = null
    reverseTarget.value = null
    error.value = null
  }

  return {
    currentScreen, liquidSavings, monthlyIncome, csvFile, useDemoData,
    state, baselineDays, zone, categories, insightProfile, scenarios,
    fastestWinId, dangerSignals, correctionCandidates, analysisDate,
    activeScenarioIds, customScenario, reverseTarget,
    stackedDays, stackedDelta, stackedZone, stackedDate,
    diagnosis, isAnalyzing, isDiagnosing, isComputingScenarios, error,
    fastestWin, displayDays, displayZone,
    analyze, toggleScenario, setCustomScenario, clearCustomScenario,
    setReverseTarget, fetchDiagnosis, goToScreen, restart,
  }
})
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/stores/runway-v4.ts
git commit -m "feat: add runway v4 Pinia store with full state management"
```

---

## Task 11: Frontend — 8 Screen Components

**Files:**
- Create: `Hackathon.Frontend/src/components/v4/PayFeedScreen.vue` (Screen 1)
- Create: `Hackathon.Frontend/src/components/v4/PayrollProfile.vue` (Screen 2)
- Create: `Hackathon.Frontend/src/components/v4/DataConnection.vue` (Screen 3)
- Create: `Hackathon.Frontend/src/components/v4/ProcessingScreenV4.vue` (Screen 4)
- Create: `Hackathon.Frontend/src/components/v4/IntelligenceReportV4.vue` (Screen 5)
- Create: `Hackathon.Frontend/src/components/v4/SurvivalDashboardV4.vue` (Screen 6)
- Create: `Hackathon.Frontend/src/components/v4/DiagnosisScreenV4.vue` (Screen 7)
- Create: `Hackathon.Frontend/src/components/v4/ActionCard.vue` (Screen 8)

This is the largest task. Each screen follows the spec's screen specifications (Section 6) using spr-* TOGE components.

**Each screen component should be implemented following the spec's element specifications exactly.** The screen components are too large to include complete code inline in this plan — implement each one following its spec section:

- **Screen 1 (PayFeedScreen):** Spec Section 6, Screen 1. Payslip card with runway teaser strip. Green CTA "Check My Runway →".
- **Screen 2 (PayrollProfile):** Spec Section 6, Screen 2. Pre-filled payroll card with spr-badge "Pre-filled by Sprout". Savings input with ₱ prefix. Consent block.
- **Screen 3 (DataConnection):** Spec Section 6, Screen 3. Three tier cards (GCash, CSV, Estimate). Selection highlights active card.
- **Screen 4 (ProcessingScreenV4):** Spec Section 6, Screen 4 + Section 12.3. 3.2s animation with raw transaction strings → category rows. spr-progress-bar. Auto-advance to Screen 5.
- **Screen 5 (IntelligenceReportV4):** Spec Section 6, Screen 5. Burn breakdown with per-category spr-progress-bar rows. Danger signal cards. "Does this look right?" collapsible section.
- **Screen 6 (SurvivalDashboardV4):** Spec Section 6, Screen 6. Large runway number (88px). Zone gradient bar with dot marker. Fastest win banner. Scenario chips. Stacked result. Reverse mode. Custom scenario expander.
- **Screen 7 (DiagnosisScreenV4):** Spec Section 6, Screen 7. Dark green archetype card. Diagnosis body with three blocks. Shimmer loading for Agent 4. Attribution line "Generated from 247 transactions · Sprout AI".
- **Screen 8 (ActionCard):** Spec Section 6, Screen 8. Zone-coloured problem card. Product card (ReadySave/ReadyCash/ReadyWage by zone). Amount chips for ReadySave. Completion state.

**Detailed implementation instructions for each screen are in the spec sections referenced above. Use spr-button, spr-card, spr-chips, spr-progress-bar, spr-badge, spr-status, spr-input, spr-collapsible, spr-logo components as specified in Section 7.**

**Step 2: Commit after each screen**

```bash
git add Hackathon.Frontend/src/components/v4/
git commit -m "feat: add all 8 screen components for runway v4"
```

---

## Task 12: Frontend — Update App.vue Router

**Files:**
- Modify: `Hackathon.Frontend/src/App.vue`

**Step 1: Replace the App.vue to use the v4 store and 8 screens**

```vue
<template>
  <div class="min-h-screen bg-gray-50">
    <PayFeedScreen v-if="store.currentScreen === 1" />
    <PayrollProfile v-else-if="store.currentScreen === 2" />
    <DataConnection v-else-if="store.currentScreen === 3" />
    <ProcessingScreenV4 v-else-if="store.currentScreen === 4" />
    <IntelligenceReportV4 v-else-if="store.currentScreen === 5" />
    <SurvivalDashboardV4 v-else-if="store.currentScreen === 6" />
    <DiagnosisScreenV4 v-else-if="store.currentScreen === 7" />
    <ActionCard v-else-if="store.currentScreen === 8" />
  </div>
</template>

<script setup lang="ts">
import { useRunwayV4Store } from './stores/runway-v4'
import PayFeedScreen from './components/v4/PayFeedScreen.vue'
import PayrollProfile from './components/v4/PayrollProfile.vue'
import DataConnection from './components/v4/DataConnection.vue'
import ProcessingScreenV4 from './components/v4/ProcessingScreenV4.vue'
import IntelligenceReportV4 from './components/v4/IntelligenceReportV4.vue'
import SurvivalDashboardV4 from './components/v4/SurvivalDashboardV4.vue'
import DiagnosisScreenV4 from './components/v4/DiagnosisScreenV4.vue'
import ActionCard from './components/v4/ActionCard.vue'

const store = useRunwayV4Store()
</script>
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/App.vue
git commit -m "feat: wire up App.vue with 8-screen v4 navigation"
```

---

## Task 13: Frontend — Zone Utility Constants

**Files:**
- Create: `Hackathon.Frontend/src/lib/zones.ts`

**Step 1: Create the zone constants (single source of truth per spec 8.5)**

```typescript
import type { ZoneName } from '../api/runway-v4-types'

export const ZONE_CONFIG: Record<ZoneName, {
  colour: string
  colourToken: string
  label: string
  description: string
  product: 'ReadyWage' | 'ReadyCash' | 'ReadySave'
}> = {
  Critical: {
    colour: '#E53E3E', // TOMATO
    colourToken: 'TOMATO',
    label: 'Critical Zone',
    description: 'Savings cover less than a month. Focus on one action right now.',
    product: 'ReadyWage',
  },
  Fragile: {
    colour: '#DD6B20', // MANGO
    colourToken: 'MANGO',
    label: 'Fragile Zone',
    description: 'About 1–2 months of runway. One unexpected expense could strain you.',
    product: 'ReadyCash',
  },
  Stable: {
    colour: '#3182CE', // BLUEBERRY
    colourToken: 'BLUEBERRY',
    label: 'Stable Zone',
    description: '2–4 months of breathing room. Enough to handle most surprises — but not enough to stop watching.',
    product: 'ReadySave',
  },
  Strong: {
    colour: '#38A169', // KANGKONG
    colourToken: 'KANGKONG',
    label: 'Strong Zone',
    description: '4+ months of cushion. Well-positioned — the goal now is to make this money work harder.',
    product: 'ReadySave',
  },
}

export const CATEGORY_LABELS: Record<string, string> = {
  FoodDining: 'Food & Dining',
  Groceries: 'Groceries & Market',
  BillsUtilities: 'Bills & Utilities',
  Transport: 'Transport',
  Shopping: 'Shopping',
  HealthWellness: 'Health & Wellness',
  Housing: 'Housing',
  Transfers: 'Transfers & Family',
  EntertainmentSubs: 'Entertainment & Subs',
  Misc: 'Miscellaneous',
}
```

**Step 2: Commit**

```bash
git add Hackathon.Frontend/src/lib/zones.ts
git commit -m "feat: add zone and category constants"
```

---

## Task 14: Backend — Add DemoMode to appsettings

**Files:**
- Modify: `Hackathon.ApiService/appsettings.Development.json`

**Step 1: Add DemoMode flag**

Add to the JSON root:
```json
"DemoMode": true
```

**Step 2: Commit**

```bash
git add Hackathon.ApiService/appsettings.Development.json
git commit -m "config: add DemoMode flag to appsettings"
```

---

## Task 15: Build Verification + Run Tests

**Step 1: Build the solution**

```bash
dotnet build Hackathon.slnx
```

Expected: Build succeeds.

**Step 2: Run all tests**

```bash
dotnet test Hackathon.Tests --verbosity normal
```

Expected: All tests pass.

**Step 3: Start the application and verify endpoints**

```bash
dotnet run --project Hackathon.ApiService
```

In a separate terminal, test the v4 analyze endpoint:
```bash
curl -X POST http://localhost:5407/api/v1/runway-v4/analyze -F "monthlyIncome=28500" -F "liquidSavings=180000" -F "useDemoData=true"
```

Expected: Returns JSON with state, baselineDays=103, zone=Stable, scenarios, insightProfile.

**Step 4: Test compute-scenarios endpoint**

```bash
curl -X POST http://localhost:5407/api/v1/runway-v4/compute-scenarios -H "Content-Type: application/json" -d '{"state":{"liquidCash":180000,"monthlyBurn":52400,"takeHome":28500,"categories":{"FoodDining":14200}},"scenarios":[{"id":"sc_grab_baseline","type":"SpendingCut","label":"test","effort":"Habit","recurrence":"Recurring","params":{"category":"FoodDining","cutAmount":2810},"delta":0}],"activeScenarioIds":["sc_grab_baseline"]}'
```

Expected: Returns stackedDays > 103, positive stackedDelta.

**Step 5: Commit any fixes**

```bash
git add -A
git commit -m "fix: resolve build issues from integration"
```

---

## Task 16: Frontend Build Verification

**Step 1: Install dependencies and build**

```bash
cd Hackathon.Frontend && npm install && npm run build
```

Expected: Build succeeds (or lists type errors to fix).

**Step 2: Fix any TypeScript errors and commit**

```bash
git add Hackathon.Frontend/
git commit -m "fix: resolve frontend TypeScript build errors"
```

---

## Task 17: Integration Test — Full Flow

**Step 1: Run the full stack via Aspire**

```bash
dotnet run --project Hackathon.AppHost
```

**Step 2: Open browser to frontend URL**

Navigate through all 8 screens manually:
1. Pay Feed → Click "Check My Runway"
2. Payroll Profile → Enter savings → Continue
3. Data Connection → Select demo data → Continue
4. Processing → Watch 3.2s animation
5. Intelligence Report → Review burn breakdown → "Get My Breakdown"
6. Survival Dashboard → Toggle scenarios → "What's My Next Move?"
7. Diagnosis → See AI narrative (live Agent 4 or fallback)
8. Action Card → See product card → Complete

**Step 3: Commit any fixes**

```bash
git add -A
git commit -m "fix: resolve integration issues from full flow test"
```

---

## Summary

| Task | Description | Files |
|------|-------------|-------|
| 1 | Core models (RunwayState, Scenario, etc.) | CoreModels.cs |
| 2 | Request/Response DTOs | RequestModels.cs |
| 3 | RunwayEngine computation functions | RunwayEngine.cs |
| 4 | Unit tests for RunwayEngine | RunwayEngineTests.cs |
| 5 | Alex Garcia demo fixtures | AlexGarciaFixtures.cs |
| 6 | Agent 4 (DiagnosisNarrative) service | DiagnosisNarrativeAgent.cs |
| 7 | Orchestrator + 3 endpoints | RunwayV4Orchestrator.cs + 3 endpoints |
| 8 | Frontend TypeScript types | runway-v4-types.ts |
| 9 | Frontend API client | runway-v4-client.ts |
| 10 | Frontend Pinia store | runway-v4.ts |
| 11 | 8 screen components | v4/*.vue |
| 12 | App.vue router update | App.vue |
| 13 | Zone utility constants | zones.ts |
| 14 | DemoMode config | appsettings.Development.json |
| 15 | Backend build + test verification | — |
| 16 | Frontend build verification | — |
| 17 | Full integration test | — |
