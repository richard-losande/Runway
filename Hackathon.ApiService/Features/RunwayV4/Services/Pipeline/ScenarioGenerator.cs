using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface IScenarioGenerator
{
    List<Scenario> Generate(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        InsightProfile profile);
}

public class ScenarioGenerator : IScenarioGenerator
{
    private static readonly HashSet<CategoryKey> DiscretionaryCategories =
    [
        CategoryKey.FoodDining,
        CategoryKey.Shopping,
        CategoryKey.EntertainmentSubs,
    ];

    public List<Scenario> Generate(
        Dictionary<CategoryKey, CategoryBreakdownEntry> categories,
        RunwayState state,
        InsightProfile profile)
    {
        var scenarios = new List<Scenario>();
        int spendingCutCount = 0;

        // 1. SpendingCut scenarios for discretionary categories (max 2)
        foreach (var catKey in DiscretionaryCategories)
        {
            if (spendingCutCount >= 2) break;
            if (!categories.TryGetValue(catKey, out var entry)) continue;
            if (entry.MonthlyAverage <= 0) continue;

            // Check for merchant-specific scenario (top merchant > 20% of category)
            var topMerchant = entry.TopMerchants.FirstOrDefault();
            if (topMerchant != null && topMerchant.MonthlyAvg > entry.MonthlyAverage * 0.2m
                && entry.MonthlyAmounts.Count >= 2)
            {
                var currentMonth = entry.MonthlyAmounts.Last();
                var baselineAvg = entry.MonthlyAmounts.Count > 1
                    ? entry.MonthlyAmounts.Take(entry.MonthlyAmounts.Count - 1).Average()
                    : currentMonth;
                var cutAmount = Math.Max(0, currentMonth - baselineAvg);

                if (cutAmount > 0)
                {
                    var baselineMonth = DateTime.UtcNow.AddMonths(-(entry.MonthlyAmounts.Count - 1));
                    scenarios.Add(new Scenario
                    {
                        Id = $"sc_{topMerchant.Name.ToLowerInvariant().Replace(" ", "_").Replace("'", "")}",
                        Type = ScenarioType.SpendingCut,
                        Label = $"Return {topMerchant.Name} to {baselineMonth:MMMM} levels",
                        Effort = EffortTag.Habit,
                        Recurrence = Recurrence.Recurring,
                        Params = new ScenarioParams
                        {
                            Category = catKey,
                            CutAmount = Math.Round(cutAmount, 0),
                        },
                    });
                    spendingCutCount++;
                    continue;
                }
            }

            // Generic category cut (70%)
            if (spendingCutCount < 2)
            {
                var catLabel = catKey switch
                {
                    CategoryKey.FoodDining => "dining & delivery",
                    CategoryKey.Shopping => "shopping",
                    CategoryKey.EntertainmentSubs => "entertainment & subs",
                    _ => catKey.ToString(),
                };

                scenarios.Add(new Scenario
                {
                    Id = $"sc_{catKey.ToString().ToLowerInvariant()}_cut",
                    Type = ScenarioType.SpendingCut,
                    Label = $"Cut {catLabel} 70%",
                    Effort = EffortTag.Habit,
                    Recurrence = Recurrence.Recurring,
                    Params = new ScenarioParams
                    {
                        Category = catKey,
                        CutPct = 0.70m,
                    },
                });
                spendingCutCount++;
            }
        }

        // 2. Always add IncomeGain: side-hustle
        scenarios.Add(new Scenario
        {
            Id = "sc_side_hustle",
            Type = ScenarioType.IncomeGain,
            Label = "Side hustle \u20b110,000/month",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                GainAmount = 10_000m,
            },
        });

        // 3. Always add IncomeGain: salary raise (10% of take-home)
        var raiseAmount = Math.Round(state.TakeHome * 0.1m, 0);
        scenarios.Add(new Scenario
        {
            Id = "sc_salary_raise",
            Type = ScenarioType.IncomeGain,
            Label = $"Salary raise 10%",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                GainAmount = raiseAmount,
            },
            Assumption = "Assumes raise takes effect immediately",
        });

        // 4. HousingChange if housing data exists
        if (categories.TryGetValue(CategoryKey.Housing, out var housingEntry) && housingEntry.MonthlyAverage > 0)
        {
            scenarios.Add(new Scenario
            {
                Id = "sc_housing",
                Type = ScenarioType.HousingChange,
                Label = "Move to a cheaper place",
                Effort = EffortTag.Life,
                Recurrence = Recurrence.Recurring,
                Params = new ScenarioParams
                {
                    RentDelta = -15_000m,
                },
                Assumption = "Assumes \u20b115,000/month savings on rent",
            });
        }

        return scenarios;
    }
}
