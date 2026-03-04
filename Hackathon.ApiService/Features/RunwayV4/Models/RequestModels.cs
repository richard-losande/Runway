using Microsoft.AspNetCore.Http;

namespace Hackathon.ApiService.Features.RunwayV4.Models;

// ── Analyze ──────────────────────────────────────────────────────────

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

// ── Diagnose ─────────────────────────────────────────────────────────

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

// ── Compute Scenarios ────────────────────────────────────────────────

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
