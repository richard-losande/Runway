using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IScenarioGenerator
{
    List<Scenario> Generate(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        InsightProfile insightProfile);
}

public class ScenarioGenerator : IScenarioGenerator
{
    private static readonly Dictionary<CategoryKey, CategoryTier> TierMap = new()
    {
        [CategoryKey.FoodDining] = CategoryTier.Discretionary,
        [CategoryKey.Groceries] = CategoryTier.Essential,
        [CategoryKey.BillsUtilities] = CategoryTier.Essential,
        [CategoryKey.Transport] = CategoryTier.Essential,
        [CategoryKey.Shopping] = CategoryTier.Discretionary,
        [CategoryKey.HealthWellness] = CategoryTier.Essential,
        [CategoryKey.Housing] = CategoryTier.Essential,
        [CategoryKey.Transfers] = CategoryTier.Committed,
        [CategoryKey.EntertainmentSubs] = CategoryTier.Discretionary,
        [CategoryKey.Misc] = CategoryTier.Discretionary,
    };

    public List<Scenario> Generate(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        InsightProfile insightProfile)
    {
        var scenarios = new List<Scenario>();
        int idCounter = 0;

        // Generate spending cut scenarios for discretionary categories with meaningful spend
        foreach (var (catKey, entry) in categories)
        {
            if (entry.MonthlyAverage < 500) continue;

            var tier = TierMap.GetValueOrDefault(catKey, CategoryTier.Discretionary);
            if (tier != CategoryTier.Discretionary) continue;

            var cutPct = 0.30m; // 30% cut
            var cutAmount = Math.Round(entry.MonthlyAverage * cutPct, 0);
            var catLabel = FormatCategoryLabel(catKey);

            idCounter++;
            scenarios.Add(new Scenario
            {
                Id = $"sc_{idCounter:D2}",
                Type = ScenarioType.SpendingCut,
                Label = $"Cut {catLabel} by 30%",
                Effort = EffortTag.Habit,
                Recurrence = Recurrence.Recurring,
                Params = new ScenarioParams
                {
                    Category = catKey,
                    CutPct = cutPct,
                    CutAmount = cutAmount,
                },
                Assumption = $"Reduce {catLabel} from ₱{entry.MonthlyAverage:N0} to ₱{entry.MonthlyAverage - cutAmount:N0}/mo",
            });
        }

        // Generate a "cook more at home" scenario if food dining is significant
        if (categories.TryGetValue(CategoryKey.FoodDining, out var foodEntry) && foodEntry.MonthlyAverage > 2000)
        {
            var cutAmount = Math.Round(foodEntry.MonthlyAverage * 0.50m, 0);
            idCounter++;
            scenarios.Add(new Scenario
            {
                Id = $"sc_{idCounter:D2}",
                Type = ScenarioType.SpendingCut,
                Label = "Cook more at home",
                Effort = EffortTag.Habit,
                Recurrence = Recurrence.Recurring,
                Params = new ScenarioParams
                {
                    Category = CategoryKey.FoodDining,
                    CutAmount = cutAmount,
                },
                Assumption = $"Cut food delivery/dining by 50% (save ₱{cutAmount:N0}/mo)",
            });
        }

        // Side hustle scenario
        if (state.TakeHome > 0)
        {
            var gainAmount = Math.Round(state.TakeHome * 0.15m, 0);
            idCounter++;
            scenarios.Add(new Scenario
            {
                Id = $"sc_{idCounter:D2}",
                Type = ScenarioType.IncomeGain,
                Label = "Side hustle income",
                Effort = EffortTag.Life,
                Recurrence = Recurrence.Recurring,
                Params = new ScenarioParams
                {
                    GainAmount = gainAmount,
                },
                Assumption = $"Earn an extra ₱{gainAmount:N0}/mo from freelance or side work",
            });
        }

        // Emergency fund injection
        if (state.LiquidCash < state.MonthlyBurn * 3)
        {
            var injectAmount = Math.Round(state.MonthlyBurn, 0);
            idCounter++;
            scenarios.Add(new Scenario
            {
                Id = $"sc_{idCounter:D2}",
                Type = ScenarioType.OneTimeInject,
                Label = "Sell unused items",
                Effort = EffortTag.Quick,
                Recurrence = Recurrence.OneTime,
                Params = new ScenarioParams
                {
                    InjectAmount = injectAmount,
                },
                Assumption = $"Liquidate ₱{injectAmount:N0} from unused items or assets",
            });
        }

        // Cancel subscriptions
        if (categories.TryGetValue(CategoryKey.EntertainmentSubs, out var subsEntry) && subsEntry.MonthlyAverage > 500)
        {
            var cutAmount = Math.Round(subsEntry.MonthlyAverage * 0.50m, 0);
            idCounter++;
            scenarios.Add(new Scenario
            {
                Id = $"sc_{idCounter:D2}",
                Type = ScenarioType.SpendingCut,
                Label = "Cancel unused subscriptions",
                Effort = EffortTag.Quick,
                Recurrence = Recurrence.Recurring,
                Params = new ScenarioParams
                {
                    Category = CategoryKey.EntertainmentSubs,
                    CutAmount = cutAmount,
                },
                Assumption = $"Cancel or downgrade subscriptions (save ₱{cutAmount:N0}/mo)",
            });
        }

        return scenarios;
    }

    private static string FormatCategoryLabel(CategoryKey key)
    {
        return key switch
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
}
