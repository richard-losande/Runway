namespace Hackathon.ApiService.Features.RunwayV4.Models;

// ── Enums ────────────────────────────────────────────────────────────

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

// ── Classes ──────────────────────────────────────────────────────────

public class RunwayState
{
    public decimal LiquidCash { get; set; }
    public decimal MonthlyBurn { get; set; }
    public decimal TakeHome { get; set; }
    public Dictionary<CategoryKey, decimal> Categories { get; set; } = new();
}

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
    public object? Confidence { get; set; }
}

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

public class ScenarioParams
{
    public CategoryKey? Category { get; set; }
    public decimal? CutPct { get; set; }
    public decimal? CutAmount { get; set; }
    public decimal? GainAmount { get; set; }
    public decimal? InjectAmount { get; set; }
    public decimal? RentDelta { get; set; }
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
    public int Delta { get; set; }
}

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
    public string Direction { get; set; } = string.Empty;
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

public class DiagnosisContent
{
    public string ArchetypeName { get; set; } = string.Empty;
    public string WhatIsHappening { get; set; } = string.Empty;
    public string WhatToDoAboutIt { get; set; } = string.Empty;
    public string HonestTake { get; set; } = string.Empty;
}

public class CorrectionCandidate
{
    public Transaction Transaction { get; set; } = new();
    public CategoryKey AssignedCategory { get; set; }
    public decimal ConfidenceScore { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class ZoneInfo
{
    public ZoneName Name { get; set; }
    public string ColourToken { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
