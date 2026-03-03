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
