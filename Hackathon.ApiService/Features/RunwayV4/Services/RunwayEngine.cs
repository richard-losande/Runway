using System.Globalization;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IRunwayEngine
{
    int ComputeBaseline(RunwayState state);
    int ComputeScenarioDays(Scenario scenario, RunwayState state);
    int ComputeDelta(Scenario scenario, RunwayState state);
    ZoneName GetZone(int days);
    ZoneInfo GetZoneInfo(int days);
    string DaysToDate(int days, DateTime referenceDate);
    List<Scenario> FindFastestPath(int targetDays, RunwayState state, List<Scenario> scenarios);
    string? FindFastestWinId(List<Scenario> scenarios, RunwayState state);
}

public class RunwayEngine : IRunwayEngine
{
    private static readonly CultureInfo PhCulture = new("en-PH");

    public static readonly Dictionary<ZoneName, ZoneInfo> Zones = new()
    {
        [ZoneName.Critical] = new ZoneInfo
        {
            Name = ZoneName.Critical,
            ColourToken = "TOMATO",
            Description = "Less than 30 days of runway. Immediate action needed to avoid running out of funds."
        },
        [ZoneName.Fragile] = new ZoneInfo
        {
            Name = ZoneName.Fragile,
            ColourToken = "MANGO",
            Description = "30\u201359 days of runway. One unexpected expense could push you into the critical zone."
        },
        [ZoneName.Stable] = new ZoneInfo
        {
            Name = ZoneName.Stable,
            ColourToken = "BLUEBERRY",
            Description = "60\u2013119 days of runway. A solid buffer, but there\u2019s room to strengthen it."
        },
        [ZoneName.Strong] = new ZoneInfo
        {
            Name = ZoneName.Strong,
            ColourToken = "KANGKONG",
            Description = "120+ days of runway. You have a strong financial cushion."
        }
    };

    public int ComputeBaseline(RunwayState state)
    {
        if (state.LiquidCash <= 0) return 0;
        if (state.MonthlyBurn <= 0) return 9999;

        return (int)Math.Floor(state.LiquidCash / (state.MonthlyBurn / 30m));
    }

    public int ComputeScenarioDays(Scenario scenario, RunwayState state)
    {
        var liquidCash = state.LiquidCash;
        var monthlyBurn = state.MonthlyBurn;

        decimal newBurn;
        decimal newCash;

        switch (scenario.Type)
        {
            case ScenarioType.SpendingCut:
            {
                var category = scenario.Params.Category;
                var catSpend = category.HasValue && state.Categories.TryGetValue(category.Value, out var spend)
                    ? spend
                    : 0m;

                var reduction = scenario.Params.CutAmount
                                ?? (catSpend * (scenario.Params.CutPct ?? 0m));

                newBurn = monthlyBurn - reduction;
                newCash = liquidCash;
                break;
            }

            case ScenarioType.IncomeGain:
            {
                var monthlyGap = monthlyBurn - state.TakeHome;
                var newGap = Math.Max(0m, monthlyGap - (scenario.Params.GainAmount ?? 0m));
                var effectiveBurn = state.TakeHome + newGap;

                newBurn = effectiveBurn;
                newCash = liquidCash;
                break;
            }

            case ScenarioType.OneTimeInject:
            {
                newCash = liquidCash + (scenario.Params.InjectAmount ?? 0m);
                newBurn = monthlyBurn;
                break;
            }

            case ScenarioType.HousingChange:
            {
                newBurn = monthlyBurn + (scenario.Params.RentDelta ?? 0m);
                newCash = liquidCash;
                break;
            }

            case ScenarioType.Custom:
            {
                var monthlyAmount = scenario.Params.MonthlyAmount ?? 0m;

                if (monthlyAmount < 0)
                {
                    // Negative = spending cut (reduce burn)
                    newBurn = monthlyBurn + monthlyAmount; // monthlyAmount is negative, so this subtracts
                }
                else
                {
                    // Positive = income gain (reduce burn)
                    newBurn = monthlyBurn - monthlyAmount;
                }

                newBurn = Math.Max(1m, newBurn);
                newCash = liquidCash;
                break;
            }

            default:
                return ComputeBaseline(state);
        }

        var tempState = new RunwayState
        {
            LiquidCash = newCash,
            MonthlyBurn = newBurn,
            TakeHome = state.TakeHome,
            Categories = state.Categories
        };

        return ComputeBaseline(tempState);
    }

    public int ComputeDelta(Scenario scenario, RunwayState state)
    {
        return ComputeScenarioDays(scenario, state) - ComputeBaseline(state);
    }

    public ZoneName GetZone(int days)
    {
        return days switch
        {
            < 30 => ZoneName.Critical,
            < 60 => ZoneName.Fragile,
            < 120 => ZoneName.Stable,
            _ => ZoneName.Strong
        };
    }

    public ZoneInfo GetZoneInfo(int days)
    {
        var zone = GetZone(days);
        return Zones[zone];
    }

    public string DaysToDate(int days, DateTime referenceDate)
    {
        return referenceDate.AddDays(days).ToString("MMMM d, yyyy", PhCulture);
    }

    public List<Scenario> FindFastestPath(int targetDays, RunwayState state, List<Scenario> scenarios)
    {
        var baseline = ComputeBaseline(state);
        var result = new List<Scenario>();

        // Filter to scenarios with positive deltas and sort descending by delta
        var candidates = scenarios
            .Select(s => new { Scenario = s, Delta = ComputeDelta(s, state) })
            .Where(x => x.Delta > 0)
            .OrderByDescending(x => x.Delta)
            .ToList();

        var accumulated = baseline;

        foreach (var candidate in candidates)
        {
            if (accumulated >= targetDays) break;

            result.Add(candidate.Scenario);
            accumulated += candidate.Delta;
        }

        return result;
    }

    public string? FindFastestWinId(List<Scenario> scenarios, RunwayState state)
    {
        return scenarios
            .Where(s => s.Effort is EffortTag.Quick or EffortTag.Habit)
            .Select(s => new { Scenario = s, Delta = ComputeDelta(s, state) })
            .Where(x => x.Delta > 0)
            .OrderByDescending(x => x.Delta)
            .Select(x => x.Scenario.Id)
            .FirstOrDefault();
    }
}
