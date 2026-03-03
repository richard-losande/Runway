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
