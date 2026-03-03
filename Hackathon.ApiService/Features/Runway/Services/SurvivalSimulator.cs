using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class SurvivalSimulator : ISurvivalSimulator
{
    private static readonly Dictionary<string, string> ScenarioLabels = new()
    {
        ["cut_lifestyle"] = "Cut dining & subscriptions",
        ["side_hustle"] = "Add \u20b110k side hustle",
        ["salary_increase"] = "Get a 10% salary raise",
        ["major_upgrade"] = "Upgrade your lifestyle",
    };

    public Sp2Output Calculate(Sp2Input input)
    {
        var baselineDays = ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn);
        var surplus = input.MonthlyIncome - input.MonthlyBurn;
        var burnRate = input.MonthlyIncome > 0
            ? (double)(input.MonthlyBurn / input.MonthlyIncome)
            : 1.0;
        var zone = ClassifyZone(baselineDays);

        var baseline = new BaselineResult
        {
            SurvivalDays = baselineDays,
            HumanLabel = FormatHumanLabel(baselineDays),
            MonthlyBurn = input.MonthlyBurn,
            MonthlySurplus = surplus,
            BurnRate = Math.Round(burnRate, 3),
            StabilityZone = zone,
        };

        // Compute individual scenario results
        var scenarios = new List<ScenarioResult>
        {
            ComputeScenario("cut_lifestyle", input, baselineDays),
            ComputeScenario("side_hustle", input, baselineDays),
            ComputeScenario("salary_increase", input, baselineDays),
            ComputeScenario("major_upgrade", input, baselineDays),
        };

        // Priority scenario goes first
        var priorityIndex = scenarios.FindIndex(s => s.Id == input.PriorityScenario);
        if (priorityIndex > 0)
        {
            var priority = scenarios[priorityIndex];
            scenarios.RemoveAt(priorityIndex);
            scenarios.Insert(0, priority);
        }

        foreach (var s in scenarios)
            s.IsPriority = s.Id == input.PriorityScenario;

        // Compute stacked result
        var stacked = ComputeStacked(input, baselineDays);

        return new Sp2Output
        {
            Baseline = baseline,
            Scenarios = scenarios,
            StackedResult = stacked,
        };
    }

    private static ScenarioResult ComputeScenario(string scenarioId, Sp2Input input, int baselineDays)
    {
        var scenarioDays = ApplyScenario(scenarioId, input);
        var label = ScenarioLabels.GetValueOrDefault(scenarioId, scenarioId);
        return new ScenarioResult
        {
            Id = scenarioId,
            Label = label,
            SurvivalDays = scenarioDays,
            DeltaDays = scenarioDays - baselineDays,
            IsPriority = false,
        };
    }

    private static int ApplyScenario(string scenarioId, Sp2Input input)
    {
        return scenarioId switch
        {
            "cut_lifestyle" => ComputeSurvivalDays(
                input.LiquidSavings,
                input.MonthlyBurn - (input.BurnBreakdown.Discretionary * 0.70m)),
            "side_hustle" => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn) + 6,
            "salary_increase" => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn) + 15,
            "major_upgrade" => ComputeSurvivalDays(
                input.LiquidSavings,
                input.MonthlyBurn + 15000m),
            _ => ComputeSurvivalDays(input.LiquidSavings, input.MonthlyBurn),
        };
    }

    private static StackedResult ComputeStacked(Sp2Input input, int baselineDays)
    {
        if (input.ActiveScenarios.Count == 0)
        {
            return new StackedResult
            {
                ActiveScenarios = [],
                SurvivalDays = baselineDays,
                DeltaDays = 0,
            };
        }

        // Apply scenarios cumulatively
        var currentBurn = input.MonthlyBurn;
        var bonusDays = 0;

        foreach (var scenario in input.ActiveScenarios.Take(3))
        {
            switch (scenario)
            {
                case "cut_lifestyle":
                    currentBurn -= input.BurnBreakdown.Discretionary * 0.70m;
                    break;
                case "side_hustle":
                    bonusDays += 6;
                    break;
                case "salary_increase":
                    bonusDays += 15;
                    break;
                case "major_upgrade":
                    currentBurn += 15000m;
                    break;
            }
        }

        var stackedDays = Math.Max(0, ComputeSurvivalDays(input.LiquidSavings, currentBurn) + bonusDays);

        return new StackedResult
        {
            ActiveScenarios = input.ActiveScenarios.Take(3).ToList(),
            SurvivalDays = stackedDays,
            DeltaDays = stackedDays - baselineDays,
        };
    }

    private static int ComputeSurvivalDays(decimal liquidSavings, decimal monthlyBurn)
    {
        if (monthlyBurn <= 0) return 9999;
        var days = (int)Math.Floor((double)(liquidSavings / monthlyBurn) * 30);
        return Math.Max(0, days);
    }

    private static string ClassifyZone(int days) => days switch
    {
        < 30 => "critical",
        < 60 => "fragile",
        < 120 => "stable",
        _ => "strong",
    };

    private static string FormatHumanLabel(int days)
    {
        var months = days / 30;
        var remainingDays = days % 30;
        if (months == 0) return $"That's about {remainingDays} days";
        if (remainingDays == 0) return $"That's about {months} month{(months > 1 ? "s" : "")}";
        return $"That's about {months} month{(months > 1 ? "s" : "")} and {remainingDays} day{(remainingDays > 1 ? "s" : "")}";
    }
}
