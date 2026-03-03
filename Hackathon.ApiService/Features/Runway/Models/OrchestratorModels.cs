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
