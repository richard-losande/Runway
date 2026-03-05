using System.Globalization;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IInsightProfileBuilder
{
    InsightProfile Build(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        int baselineDays);
}

public class InsightProfileBuilder : IInsightProfileBuilder
{
    private static readonly HashSet<CategoryKey> DiscretionaryCategories =
    [
        CategoryKey.FoodDining,
        CategoryKey.Shopping,
        CategoryKey.EntertainmentSubs,
        CategoryKey.Misc,
    ];

    private static readonly HashSet<CategoryKey> FixedCategories =
    [
        CategoryKey.Groceries,
        CategoryKey.BillsUtilities,
        CategoryKey.Transport,
        CategoryKey.HealthWellness,
        CategoryKey.Housing,
        CategoryKey.Transfers,
    ];

    public InsightProfile Build(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        int baselineDays)
    {
        var archetype = DetectArchetype(categories, state, baselineDays);
        var dangerSignals = BuildDangerSignals(categories, state);
        var trends = BuildTrends(categories);
        var remittanceNote = BuildRemittanceNote(categories, state);

        var flexibleBurn = DiscretionaryCategories
            .Where(c => categories.ContainsKey(c))
            .Sum(c => categories[c].MonthlyAverage);

        var fixedBurn = FixedCategories
            .Where(c => categories.ContainsKey(c))
            .Sum(c => categories[c].MonthlyAverage);

        return new InsightProfile
        {
            Archetype = archetype,
            DangerSignals = dangerSignals,
            Trends = trends,
            RemittanceNote = remittanceNote,
            FlexibleBurn = Math.Round(flexibleBurn, 2),
            FixedBurn = Math.Round(fixedBurn, 2),
        };
    }

    private static ArchetypeInfo DetectArchetype(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        int baselineDays)
    {
        // Determined by burn trend + gap size (per spec)

        // crisis_mode: burn is 2x+ income, or runway < 30 days
        if (state.MonthlyBurn >= state.TakeHome * 2 || baselineDays < 30)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.CrisisMode,
                Name = "On the Edge",
                Signal = "Your burn far exceeds your income, or your runway is under 30 days.",
            };
        }

        // resilient_saver: burn consistently below income — buffer is growing
        if (state.MonthlyBurn < state.TakeHome * 0.85m)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.ResilientSaver,
                Name = "The Resilient Saver",
                Signal = "Your burn is consistently below your income — your buffer is growing.",
            };
        }

        // lifestyle_inflator: burn > income AND discretionary spending is growing
        bool hasGrowingDiscretionary = false;
        foreach (var catKey in DiscretionaryCategories.Where(c => c != CategoryKey.Misc))
        {
            if (categories.TryGetValue(catKey, out var entry) && entry.MonthlyAmounts.Count >= 2)
            {
                var pctChange = ComputePctChange(entry.MonthlyAmounts);
                if (pctChange > 15) { hasGrowingDiscretionary = true; break; }
            }
        }

        if (state.MonthlyBurn > state.TakeHome && hasGrowingDiscretionary)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.LifestyleInflator,
                Name = "The Lifestyle Inflator",
                Signal = "Spending has expanded to fill and exceed the salary.",
            };
        }

        // steady_spender: burn roughly matches income — stable but no buffer growth
        return new ArchetypeInfo
        {
            Key = ArchetypeKey.SteadySpender,
            Name = "The Steady Spender",
            Signal = "Your burn roughly matches your income — stable but no buffer growth.",
        };
    }

    private static List<DangerSignal> BuildDangerSignals(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state)
    {
        var signals = new List<DangerSignal>();

        // Burn > takeHome
        if (state.MonthlyBurn > state.TakeHome)
        {
            var gap = state.MonthlyBurn - state.TakeHome;
            signals.Add(new DangerSignal
            {
                Severity = "danger",
                Title = "Spending exceeds income",
                Detail = $"Your monthly burn is \u20b1{Fmt(state.MonthlyBurn)} but take-home is \u20b1{Fmt(state.TakeHome)}.",
                Metric = $"\u20b1{Fmt(gap)} gap",
            });
        }

        // Discretionary categories growing > 25%
        foreach (var catKey in DiscretionaryCategories.Where(c => c != CategoryKey.Misc))
        {
            if (!categories.TryGetValue(catKey, out var entry)) continue;
            if (entry.MonthlyAmounts.Count < 2) continue;

            var pctChange = ComputePctChange(entry.MonthlyAmounts);
            if (pctChange > 25)
            {
                var topMerchant = entry.TopMerchants.FirstOrDefault();
                var detail = topMerchant != null
                    ? $"{CatLabel(catKey)} is up {pctChange:F0}%. {topMerchant.Name} alone is \u20b1{Fmt(topMerchant.MonthlyAvg)}/month."
                    : $"{CatLabel(catKey)} is up {pctChange:F0}% month-over-month.";

                signals.Add(new DangerSignal
                {
                    Severity = "danger",
                    Title = $"{CatLabel(catKey)} spike",
                    Detail = detail,
                    Metric = $"+{pctChange:F0}%",
                    Category = catKey,
                });
            }
            else if (pctChange > 10)
            {
                signals.Add(new DangerSignal
                {
                    Severity = "caution",
                    Title = $"{CatLabel(catKey)} growing",
                    Detail = $"{CatLabel(catKey)} has grown {pctChange:F0}% recently.",
                    Metric = $"+{pctChange:F0}%",
                    Category = catKey,
                });
            }
        }

        // Single merchant > 20% of total burn
        foreach (var (catKey, entry) in categories)
        {
            foreach (var merchant in entry.TopMerchants)
            {
                if (state.MonthlyBurn > 0 && merchant.MonthlyAvg > state.MonthlyBurn * 0.2m)
                {
                    signals.Add(new DangerSignal
                    {
                        Severity = "caution",
                        Title = $"{merchant.Name} concentration",
                        Detail = $"{merchant.Name} accounts for \u20b1{Fmt(merchant.MonthlyAvg)}/month — over 20% of your burn.",
                        Metric = $"\u20b1{Fmt(merchant.MonthlyAvg)}",
                        Category = catKey,
                    });
                }
            }
        }

        // Max 3, prioritised by severity then impact
        return signals
            .OrderBy(s => s.Severity == "danger" ? 0 : 1)
            .Take(3)
            .ToList();
    }

    private static List<TrendInfo> BuildTrends(Dictionary<CategoryKey, CategoryBreakdownEntry> categories)
    {
        var trends = new List<TrendInfo>();

        foreach (var (catKey, entry) in categories)
        {
            if (entry.MonthlyAmounts.Count < 2) continue;

            var pctChange = ComputePctChange(entry.MonthlyAmounts);
            var direction = pctChange > 10 ? "growing"
                : pctChange < -10 ? "declining"
                : "stable";
            var notable = Math.Abs(pctChange) > 15 && DiscretionaryCategories.Contains(catKey);

            var topMerchant = entry.TopMerchants.FirstOrDefault();

            trends.Add(new TrendInfo
            {
                Category = catKey,
                Direction = direction,
                PctChange = Math.Round(pctChange, 0),
                Notable = notable,
                TopMerchant = topMerchant?.Name ?? "Unknown",
                TopMerchantAmount = topMerchant?.MonthlyAvg ?? 0,
            });
        }

        return trends.OrderByDescending(t => Math.Abs(t.PctChange)).ToList();
    }

    private static string? BuildRemittanceNote(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state)
    {
        if (!categories.TryGetValue(CategoryKey.Transfers, out var transferEntry))
            return null;

        if (state.MonthlyBurn <= 0) return null;

        var transferPct = transferEntry.MonthlyAverage / state.MonthlyBurn;
        if (transferPct >= 0.15m)
        {
            return $"\u20b1{Fmt(transferEntry.MonthlyAverage)} of your monthly outflow goes to family \u2014 that\u2019s a commitment, not a cut.";
        }

        return null;
    }

    private static decimal ComputePctChange(List<decimal> monthlyAmounts)
    {
        if (monthlyAmounts.Count < 2) return 0;

        var lastTwo = monthlyAmounts.TakeLast(2).ToList();
        var previous = lastTwo[0];
        var current = lastTwo[1];

        if (previous == 0) return current > 0 ? 100 : 0;
        return ((current - previous) / previous) * 100;
    }

    private static string Fmt(decimal value) =>
        value.ToString("N0", CultureInfo.InvariantCulture);

    private static string CatLabel(CategoryKey key) => key switch
    {
        CategoryKey.FoodDining => "Food & Dining",
        CategoryKey.Groceries => "Groceries",
        CategoryKey.BillsUtilities => "Bills & Utilities",
        CategoryKey.Transport => "Transport",
        CategoryKey.Shopping => "Shopping",
        CategoryKey.HealthWellness => "Health & Wellness",
        CategoryKey.Housing => "Housing",
        CategoryKey.Transfers => "Transfers",
        CategoryKey.EntertainmentSubs => "Entertainment & Subs",
        CategoryKey.Misc => "Miscellaneous",
        _ => key.ToString(),
    };
}
