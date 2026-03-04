using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IInsightProfileBuilder
{
    InsightProfile Build(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        int baselineDays);
}

public class InsightProfileBuilder : IInsightProfileBuilder
{
    private static readonly HashSet<CategoryKey> EssentialCategories =
    [
        CategoryKey.Groceries,
        CategoryKey.BillsUtilities,
        CategoryKey.Transport,
        CategoryKey.HealthWellness,
        CategoryKey.Housing,
    ];

    public InsightProfile Build(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        int baselineDays)
    {
        var profile = new InsightProfile();

        // Compute fixed vs flexible burn
        decimal fixedBurn = 0, flexibleBurn = 0;
        foreach (var (catKey, entry) in categories)
        {
            if (EssentialCategories.Contains(catKey))
                fixedBurn += entry.MonthlyAverage;
            else
                flexibleBurn += entry.MonthlyAverage;
        }

        profile.FixedBurn = fixedBurn;
        profile.FlexibleBurn = flexibleBurn;

        // Determine archetype
        profile.Archetype = DetermineArchetype(state, baselineDays, fixedBurn, flexibleBurn);

        // Build danger signals
        profile.DangerSignals = BuildDangerSignals(state, categories, baselineDays);

        // Build trends from monthly data
        profile.Trends = BuildTrends(categories);

        // Detect remittance patterns
        if (categories.TryGetValue(CategoryKey.Transfers, out var transfers) && transfers.MonthlyAverage > 3000)
        {
            var pct = state.MonthlyBurn > 0
                ? Math.Round(transfers.MonthlyAverage / state.MonthlyBurn * 100, 0)
                : 0;
            if (pct >= 10)
            {
                profile.RemittanceNote = $"₱{transfers.MonthlyAverage:N0}/mo goes to transfers ({pct}% of spend). This may include remittances — we won't suggest cutting those.";
            }
        }

        return profile;
    }

    private static ArchetypeInfo DetermineArchetype(
        RunwayState state, int baselineDays, decimal fixedBurn, decimal flexibleBurn)
    {
        var totalBurn = fixedBurn + flexibleBurn;
        var flexRatio = totalBurn > 0 ? flexibleBurn / totalBurn : 0;
        var gap = state.MonthlyBurn - state.TakeHome;

        if (baselineDays < 30)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.CrisisMode,
                Name = "Crisis Mode",
                Signal = "Less than 30 days of runway with active burn exceeding income.",
            };
        }

        if (gap > 0 && flexRatio > 0.4m)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.LifestyleInflator,
                Name = "Lifestyle Inflator",
                Signal = "Discretionary spending outpaces income growth, compressing runway over time.",
            };
        }

        if (baselineDays >= 120)
        {
            return new ArchetypeInfo
            {
                Key = ArchetypeKey.ResilientSaver,
                Name = "Resilient Saver",
                Signal = "Strong cash buffer with controlled spending. Well-positioned to weather shocks.",
            };
        }

        return new ArchetypeInfo
        {
            Key = ArchetypeKey.SteadySpender,
            Name = "Steady Spender",
            Signal = "Spending is consistent and mostly predictable, but there's room to build a larger buffer.",
        };
    }

    private static List<DangerSignal> BuildDangerSignals(
        RunwayState state,
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        int baselineDays)
    {
        var signals = new List<DangerSignal>();

        // Critical runway
        if (baselineDays < 30)
        {
            signals.Add(new DangerSignal
            {
                Severity = "danger",
                Title = "Runway below 30 days",
                Detail = $"At current burn, savings last only {baselineDays} days.",
                Metric = $"{baselineDays} days",
            });
        }
        else if (baselineDays < 60)
        {
            signals.Add(new DangerSignal
            {
                Severity = "caution",
                Title = "Runway under 60 days",
                Detail = $"One unexpected expense could push you into the critical zone.",
                Metric = $"{baselineDays} days",
            });
        }

        // Income-spend gap
        var gap = state.MonthlyBurn - state.TakeHome;
        if (gap > 0)
        {
            signals.Add(new DangerSignal
            {
                Severity = gap > state.TakeHome * 0.3m ? "danger" : "caution",
                Title = "Spending exceeds income",
                Detail = $"Monthly gap of ₱{gap:N0} is being covered by savings.",
                Metric = $"₱{gap:N0}/mo gap",
            });
        }

        // Single category dominance (>40% of burn)
        foreach (var (catKey, entry) in categories)
        {
            if (state.MonthlyBurn > 0 && entry.MonthlyAverage / state.MonthlyBurn > 0.4m)
            {
                signals.Add(new DangerSignal
                {
                    Severity = "caution",
                    Title = $"{catKey} dominates spending",
                    Detail = $"{catKey} accounts for {entry.MonthlyAverage / state.MonthlyBurn:P0} of total burn.",
                    Metric = $"₱{entry.MonthlyAverage:N0}/mo",
                    Category = catKey,
                });
            }
        }

        return signals;
    }

    private static List<TrendInfo> BuildTrends(Dictionary<CategoryKey, CategoryBreakdownEntry> categories)
    {
        var trends = new List<TrendInfo>();

        foreach (var (catKey, entry) in categories)
        {
            if (entry.MonthlyAmounts.Count < 2) continue;

            var recent = entry.MonthlyAmounts[^1];
            var previous = entry.MonthlyAmounts[^2];
            if (previous == 0) continue;

            var pctChange = Math.Round((recent - previous) / previous * 100, 1);
            var direction = pctChange > 0 ? "up" : pctChange < 0 ? "down" : "flat";
            var notable = Math.Abs(pctChange) >= 15;

            var topMerchant = entry.TopMerchants.FirstOrDefault();

            trends.Add(new TrendInfo
            {
                Category = catKey,
                Direction = direction,
                PctChange = pctChange,
                Notable = notable,
                TopMerchant = topMerchant?.Name ?? string.Empty,
                TopMerchantAmount = topMerchant?.MonthlyAvg ?? 0,
            });
        }

        return trends.OrderByDescending(t => Math.Abs(t.PctChange)).ToList();
    }
}
