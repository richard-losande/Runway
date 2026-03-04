using Xunit;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.Tests;

public class RunwayEngineTests
{
    private readonly RunwayEngine _engine = new();

    private static RunwayState AlexGarciaState() => new()
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

    // ── ComputeBaseline ──────────────────────────────────────────────

    [Fact]
    public void ComputeBaseline_AlexGarcia_Returns103()
    {
        var state = AlexGarciaState();
        var result = _engine.ComputeBaseline(state);
        Assert.Equal(103, result);
    }

    [Fact]
    public void ComputeBaseline_ZeroLiquidCash_Returns0()
    {
        var state = new RunwayState
        {
            LiquidCash = 0m,
            MonthlyBurn = 52_400m,
            TakeHome = 28_500m,
            Categories = new Dictionary<CategoryKey, decimal>()
        };
        var result = _engine.ComputeBaseline(state);
        Assert.Equal(0, result);
    }

    [Fact]
    public void ComputeBaseline_ZeroMonthlyBurn_ReturnsMaxValue()
    {
        var state = new RunwayState
        {
            LiquidCash = 180_000m,
            MonthlyBurn = 0m,
            TakeHome = 28_500m,
            Categories = new Dictionary<CategoryKey, decimal>()
        };
        var result = _engine.ComputeBaseline(state);
        Assert.Equal(int.MaxValue, result);
    }

    // ── ComputeScenarioDays ──────────────────────────────────────────

    [Fact]
    public void ComputeScenarioDays_SpendingCut70PctOnFood_ReturnsGreaterThan103()
    {
        // 70% cut on FoodDining (14200): reduction = 14200 * 0.70 = 9940
        // newBurn = 52400 - 9940 = 42460
        // days = Floor(180000 / (42460 / 30)) = 127
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "cut-food",
            Type = ScenarioType.SpendingCut,
            Label = "Cut food spending by 70%",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                Category = CategoryKey.FoodDining,
                CutPct = 0.70m
            }
        };

        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.True(result > 103, $"SpendingCut scenario should return > 103, got {result}");
        Assert.Equal(127, result);
    }

    [Fact]
    public void ComputeScenarioDays_IncomeGain10PctRaise_ReturnsGreaterThan103()
    {
        // 10% raise on TakeHome 28500 => gainAmount = 2850
        // monthlyGap = 52400 - 28500 = 23900
        // newGap = max(0, 23900 - 2850) = 21050
        // effectiveBurn = 28500 + 21050 = 49550
        // days = Floor(180000 / (49550 / 30)) = Floor(180000 / 1651.666...) = 108
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "income-raise",
            Type = ScenarioType.IncomeGain,
            Label = "10% salary raise",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                GainAmount = 2_850m
            }
        };

        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.True(result > 103, $"IncomeGain scenario should return > 103, got {result}");
        Assert.Equal(108, result);
    }

    [Fact]
    public void ComputeScenarioDays_OneTimeInject10000_Returns108()
    {
        // newCash = 180000 + 10000 = 190000
        // days = Floor(190000 / (52400 / 30)) = Floor(190000 / 1746.666) = 108
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "inject-10k",
            Type = ScenarioType.OneTimeInject,
            Label = "One-time cash injection",
            Effort = EffortTag.Quick,
            Recurrence = Recurrence.OneTime,
            Params = new ScenarioParams
            {
                InjectAmount = 10_000m
            }
        };

        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.Equal(108, result);
    }

    [Fact]
    public void ComputeScenarioDays_HousingChangePlus15000_Returns80()
    {
        // newBurn = 52400 + 15000 = 67400
        // days = Floor(180000 / (67400 / 30)) = Floor(180000 / 2246.666) = 80
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "housing-up",
            Type = ScenarioType.HousingChange,
            Label = "New apartment (+15k rent)",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                RentDelta = 15_000m
            }
        };

        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.Equal(80, result);
    }

    [Fact]
    public void ComputeScenarioDays_CustomMinus5000_Returns113()
    {
        // monthlyAmount = -5000 (negative = spending cut, reduce burn)
        // newBurn = 52400 + (-5000) = 47400, max(1, 47400) = 47400
        // days = Floor(180000 / (47400 / 30)) = Floor(180000 / 1580) = 113
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "custom-cut",
            Type = ScenarioType.Custom,
            Label = "Custom spending reduction",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                MonthlyAmount = -5_000m
            }
        };

        var result = _engine.ComputeScenarioDays(scenario, state);
        Assert.Equal(113, result);
    }

    // ── ComputeDelta ─────────────────────────────────────────────────

    [Fact]
    public void ComputeDelta_EqualsScenarioDaysMinusBaseline()
    {
        var state = AlexGarciaState();
        var scenario = new Scenario
        {
            Id = "inject-10k",
            Type = ScenarioType.OneTimeInject,
            Label = "One-time cash injection",
            Effort = EffortTag.Quick,
            Recurrence = Recurrence.OneTime,
            Params = new ScenarioParams
            {
                InjectAmount = 10_000m
            }
        };

        var baseline = _engine.ComputeBaseline(state);
        var scenarioDays = _engine.ComputeScenarioDays(scenario, state);
        var delta = _engine.ComputeDelta(scenario, state);

        Assert.Equal(scenarioDays - baseline, delta);
        Assert.Equal(108 - 103, delta);
    }

    // ── DaysToDate ───────────────────────────────────────────────────

    [Fact]
    public void DaysToDate_103DaysFromSep30_2024_ReturnsJanuary11_2025()
    {
        var referenceDate = new DateTime(2024, 9, 30);
        var result = _engine.DaysToDate(103, referenceDate);
        Assert.Equal("January 11, 2025", result);
    }

    // ── GetZone ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(29, ZoneName.Critical)]
    [InlineData(30, ZoneName.Fragile)]
    [InlineData(59, ZoneName.Fragile)]
    [InlineData(60, ZoneName.Stable)]
    [InlineData(119, ZoneName.Stable)]
    [InlineData(120, ZoneName.Strong)]
    [InlineData(500, ZoneName.Strong)]
    public void GetZone_ReturnsCorrectZone(int days, ZoneName expectedZone)
    {
        var result = _engine.GetZone(days);
        Assert.Equal(expectedZone, result);
    }

    // ── FindFastestPath ──────────────────────────────────────────────

    [Fact]
    public void FindFastestPath_GreedySelectionExcludesNegativeDelta_SelectsPositiveOnes()
    {
        var state = AlexGarciaState();

        var positiveScenario1 = new Scenario
        {
            Id = "cut-food",
            Type = ScenarioType.SpendingCut,
            Label = "Cut food 70%",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                Category = CategoryKey.FoodDining,
                CutPct = 0.70m
            }
        };

        var positiveScenario2 = new Scenario
        {
            Id = "custom-cut",
            Type = ScenarioType.Custom,
            Label = "Custom -5k",
            Effort = EffortTag.Habit,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                MonthlyAmount = -5_000m
            }
        };

        var negativeScenario = new Scenario
        {
            Id = "housing-up",
            Type = ScenarioType.HousingChange,
            Label = "New apartment (+15k)",
            Effort = EffortTag.Life,
            Recurrence = Recurrence.Recurring,
            Params = new ScenarioParams
            {
                RentDelta = 15_000m
            }
        };

        var scenarios = new List<Scenario> { positiveScenario1, positiveScenario2, negativeScenario };

        // Target well above baseline so it picks all positive-delta scenarios
        var result = _engine.FindFastestPath(200, state, scenarios);

        // Should include only positive-delta scenarios, not the housing increase
        Assert.Contains(result, s => s.Id == "cut-food");
        Assert.Contains(result, s => s.Id == "custom-cut");
        Assert.DoesNotContain(result, s => s.Id == "housing-up");

        // The negative scenario (housing +15k) has delta = 80 - 103 = -23, so it is excluded
        var housingDelta = _engine.ComputeDelta(negativeScenario, state);
        Assert.True(housingDelta < 0, $"Housing scenario should have negative delta, got {housingDelta}");
    }
}
