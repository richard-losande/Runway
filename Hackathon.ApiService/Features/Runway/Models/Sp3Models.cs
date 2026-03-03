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
