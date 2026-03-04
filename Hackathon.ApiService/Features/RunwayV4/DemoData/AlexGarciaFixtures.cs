using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.DemoData;

/// <summary>
/// Demo fixture data for Alex Garcia persona, matching spec section 12.5.
/// All values are canonical and used by unit tests.
/// </summary>
public static class AlexGarciaFixtures
{
    // ── 12.5.1 RunwayState ──────────────────────────────────────────────

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

    // ── 12.5.2 InsightProfile (Agent 2 fixture) ─────────────────────────

    public static InsightProfile InsightProfile => new()
    {
        Archetype = new ArchetypeInfo
        {
            Key = ArchetypeKey.LifestyleInflator,
            Name = "The Lifestyle Inflator",
            Signal = "Monthly burn exceeds take-home by \u20b123,900 and food & dining has grown 38% over 3 months.",
        },
        DangerSignals = new List<DangerSignal>
        {
            new()
            {
                Severity = "danger",
                Title = "Monthly spend exceeds take-home pay",
                Detail = "Your \u20b152,400 monthly burn is \u20b123,900 above your \u20b128,500 take-home. Your savings are covering the gap.",
                Metric = "\u20b123,900 gap",
                Category = null,
            },
            new()
            {
                Severity = "caution",
                Title = "Food & dining growing fast",
                Detail = "Grab Food alone is \u20b110,200 this month \u2014 up 38% from your 3-month average of \u20b17,390.",
                Metric = "+38%",
                Category = CategoryKey.FoodDining,
            },
        },
        Trends = new List<TrendInfo>
        {
            new()
            {
                Category = CategoryKey.FoodDining,
                Direction = "growing",
                PctChange = 38m,
                Notable = true,
                TopMerchant = "Grab Food",
                TopMerchantAmount = 10_200m,
            },
            new()
            {
                Category = CategoryKey.Shopping,
                Direction = "stable",
                PctChange = 4m,
                Notable = false,
                TopMerchant = "Lazada",
                TopMerchantAmount = 2_800m,
            },
            new()
            {
                Category = CategoryKey.EntertainmentSubs,
                Direction = "stable",
                PctChange = 0m,
                Notable = false,
                TopMerchant = "Netflix",
                TopMerchantAmount = 899m,
            },
            new()
            {
                Category = CategoryKey.Transfers,
                Direction = "stable",
                PctChange = 0m,
                Notable = false,
                TopMerchant = "GCash Send Money",
                TopMerchantAmount = 5_000m,
            },
        },
        RemittanceNote = null, // transfers = 5000 / 52400 = 9.5% — below 15% threshold
        FlexibleBurn = 22_900m, // food_dining(14200) + shopping(5000) + entertainment_subs(3300) + misc(400)
        FixedBurn = 29_500m,    // groceries(8000) + bills(7200) + transport(6500) + health(2800) + housing(0) + transfers(5000)
    };

    // ── 12.5.3 Scenarios (Agent 3 fixture) ──────────────────────────────

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
            Assumption = null,
            // cutAmount = 10200 (Sep) - 7390 (3-month avg) = 2810
            // delta = Math.floor(180000 / ((52400 - 2810) / 30)) - 103 = +5 days
            // fastestWin — highest delta among habit-tier
        },
        new()
        {
            Id = "sc_dining_cut",
            Type = ScenarioType.SpendingCut,
            Label = "Cut dining & delivery 70%",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { Category = CategoryKey.FoodDining, CutPct = 0.70m },
            Assumption = null,
            // cutAmount = 14200 * 0.70 = 9940
            // delta = Math.floor(180000 / ((52400 - 9940) / 30)) - 103 = +20 days
        },
        new()
        {
            Id = "sc_salary_raise",
            Type = ScenarioType.IncomeGain,
            Label = "Salary raise 10%",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { GainAmount = 2_850m },
            Assumption = "Assumes raise takes effect immediately",
            // gainAmount = 28500 * 0.10 = 2850
            // newGap = (52400 - 28500) - 2850 = 21050
            // effectiveBurn = 28500 + 21050 = 49550
            // delta = Math.floor(180000 / (49550 / 30)) - 103 = +6 days
        },
        new()
        {
            Id = "sc_side_hustle",
            Type = ScenarioType.IncomeGain,
            Label = "Side hustle \u20b110k/mo",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { GainAmount = 10_000m },
            Assumption = "Assumes sustained \u20b110,000/month additional income",
            // newGap = (52400 - 28500) - 10000 = 13900
            // effectiveBurn = 28500 + 13900 = 42400
            // delta = Math.floor(180000 / (42400 / 30)) - 103 = +24 days
        },
        new()
        {
            Id = "sc_housing",
            Type = ScenarioType.HousingChange,
            Label = "Move to a bigger unit",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams { RentDelta = 15_000m },
            Assumption = "Assumes +\u20b115,000/month increase in housing cost",
            // newBurn = 52400 + 15000 = 67400
            // delta = Math.floor(180000 / (67400 / 30)) - 103 = -23 days
        },
    };

    // ── Category Breakdown ──────────────────────────────────────────────

    public static Dictionary<CategoryKey, CategoryBreakdownEntry> CategoryBreakdown => new()
    {
        [CategoryKey.FoodDining] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 14_200m,
            MonthlyAmounts = new List<decimal> { 11_800m, 12_600m, 14_200m },
            Tier = CategoryTier.Discretionary,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Grab Food", MonthlyAvg = 10_200m },
                new() { Name = "Jollibee", MonthlyAvg = 2_100m },
                new() { Name = "McDonald's", MonthlyAvg = 1_900m },
            },
            TransactionCount = 68,
        },
        [CategoryKey.Groceries] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 8_000m,
            MonthlyAmounts = new List<decimal> { 7_800m, 8_200m, 8_000m },
            Tier = CategoryTier.Essential,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "SM Supermarket", MonthlyAvg = 5_200m },
                new() { Name = "Puregold", MonthlyAvg = 2_800m },
            },
            TransactionCount = 24,
        },
        [CategoryKey.BillsUtilities] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 7_200m,
            MonthlyAmounts = new List<decimal> { 7_100m, 7_300m, 7_200m },
            Tier = CategoryTier.Essential,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Meralco", MonthlyAvg = 3_120m },
                new() { Name = "Globe", MonthlyAvg = 1_999m },
                new() { Name = "Maynilad", MonthlyAvg = 1_081m },
            },
            TransactionCount = 12,
        },
        [CategoryKey.Transport] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 6_500m,
            MonthlyAmounts = new List<decimal> { 6_200m, 6_800m, 6_500m },
            Tier = CategoryTier.Essential,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Grab Car", MonthlyAvg = 4_200m },
                new() { Name = "Shell", MonthlyAvg = 1_500m },
            },
            TransactionCount = 42,
        },
        [CategoryKey.Shopping] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 5_000m,
            MonthlyAmounts = new List<decimal> { 4_800m, 5_200m, 5_000m },
            Tier = CategoryTier.Discretionary,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Lazada", MonthlyAvg = 2_800m },
                new() { Name = "Shopee", MonthlyAvg = 2_200m },
            },
            TransactionCount = 18,
        },
        [CategoryKey.Transfers] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 5_000m,
            MonthlyAmounts = new List<decimal> { 5_000m, 5_000m, 5_000m },
            Tier = CategoryTier.Committed,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "GCash Send Money", MonthlyAvg = 5_000m },
            },
            TransactionCount = 4,
        },
        [CategoryKey.EntertainmentSubs] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 3_300m,
            MonthlyAmounts = new List<decimal> { 3_300m, 3_300m, 3_300m },
            Tier = CategoryTier.Discretionary,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Netflix", MonthlyAvg = 899m },
                new() { Name = "Spotify", MonthlyAvg = 299m },
                new() { Name = "Steam", MonthlyAvg = 1_200m },
            },
            TransactionCount = 8,
        },
        [CategoryKey.HealthWellness] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 2_800m,
            MonthlyAmounts = new List<decimal> { 2_600m, 3_000m, 2_800m },
            Tier = CategoryTier.Essential,
            TopMerchants = new List<MerchantSummary>
            {
                new() { Name = "Mercury Drug", MonthlyAvg = 1_500m },
                new() { Name = "Anytime Fitness", MonthlyAvg = 1_300m },
            },
            TransactionCount = 10,
        },
        [CategoryKey.Housing] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 0m,
            MonthlyAmounts = new List<decimal> { 0m, 0m, 0m },
            Tier = CategoryTier.Essential,
            TopMerchants = new List<MerchantSummary>(),
            TransactionCount = 0,
        },
        [CategoryKey.Misc] = new CategoryBreakdownEntry
        {
            MonthlyAverage = 400m,
            MonthlyAmounts = new List<decimal> { 500m, 300m, 400m },
            Tier = CategoryTier.Discretionary,
            TopMerchants = new List<MerchantSummary>(),
            TransactionCount = 61,
        },
    };

    // ── 12.5.4 Fallback Diagnosis (Agent 4 timeout fallback) ────────────

    public static DiagnosisContent FallbackDiagnosis => new()
    {
        ArchetypeName = "The Lifestyle Inflator",
        WhatIsHappening =
            "Your monthly burn has grown to \u20b152,400 \u2014 but your take-home is \u20b128,500. " +
            "Your savings are covering a \u20b123,900 monthly gap. " +
            "Grab Food alone is \u20b110,200 this month, up 38% from three months ago.",
        WhatToDoAboutIt =
            "Return Grab Food to July levels \u2014 that single change reduces your monthly burn by \u20b12,810. " +
            "Your runway goes from 103 days to 108 days. " +
            "Stack it with a side hustle and you cross 130 days.",
        HonestTake =
            "103 days is not a crisis \u2014 but the gap between what comes in and what goes out " +
            "has been quietly widening for three months.",
    };

    // ── Demo Transaction Strings (Screen 4 animation) ───────────────────

    public static string[] DemoTransactionStrings => new[]
    {
        "GRAB*FOOD PHILIPPINES 09:42 \u20b1340.00",
        "GCASH SEND MONEY NENA G. \u20b15,000.00",
        "SM SUPERMARKET MOA 14:21 \u20b12,840.00",
        "MERALCO PAYMENT ECPay \u20b13,120.00",
        "LAZADA PAYMENTS PTE LTD \u20b11,299.00",
        "SHOPEE PAY SPAY-241103 \u20b1890.00",
    };

    // ── Demo Category Order (Screen 4 category fade-in) ─────────────────

    public static (CategoryKey Key, string Label, decimal Amount)[] DemoCategoryOrder => new[]
    {
        (CategoryKey.FoodDining, "Food & Dining", 14_200m),
        (CategoryKey.Groceries, "Groceries & Market", 8_000m),
        (CategoryKey.BillsUtilities, "Bills & Utilities", 7_200m),
        (CategoryKey.Transport, "Transport", 6_500m),
        (CategoryKey.Shopping, "Shopping", 5_000m),
        (CategoryKey.Transfers, "Transfers & Family", 5_000m),
        (CategoryKey.EntertainmentSubs, "Entertainment & Subs", 3_300m),
        (CategoryKey.HealthWellness, "Health & Wellness", 2_800m),
        (CategoryKey.Misc, "Miscellaneous", 400m),
    };
}
