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
