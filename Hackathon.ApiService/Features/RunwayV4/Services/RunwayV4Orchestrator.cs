using Hackathon.ApiService.Features.RunwayV4.DemoData;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IRunwayV4Orchestrator
{
    Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct);
    RunwayComputeScenariosResponse ComputeScenarios(RunwayComputeScenariosRequest request);
}

public class RunwayV4Orchestrator : IRunwayV4Orchestrator
{
    private readonly IRunwayEngine _engine;
    private readonly bool _demoMode;

    public RunwayV4Orchestrator(IRunwayEngine engine, IConfiguration configuration)
    {
        _engine = engine;
        _demoMode = configuration.GetValue("DemoMode", true);
    }

    public Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // In demo mode (or production placeholder): start from fixture state,
        // but override LiquidCash and TakeHome from the request.
        var state = AlexGarciaFixtures.State;
        state.LiquidCash = request.LiquidSavings > 0 ? request.LiquidSavings : state.LiquidCash;
        state.TakeHome = request.MonthlyIncome > 0 ? request.MonthlyIncome : state.TakeHome;

        var baselineDays = _engine.ComputeBaseline(state);
        var zone = _engine.GetZone(baselineDays);

        // In demo mode use fixture scenarios; production would generate them
        var scenarios = AlexGarciaFixtures.Scenarios;

        // Compute delta for each scenario
        foreach (var scenario in scenarios)
        {
            scenario.Delta = _engine.ComputeDelta(scenario, state);
        }

        var fastestWinId = _engine.FindFastestWinId(scenarios, state);

        var response = new RunwayAnalyzeResponse
        {
            State = state,
            BaselineDays = baselineDays,
            Zone = zone,
            Categories = AlexGarciaFixtures.CategoryBreakdown,
            InsightProfile = AlexGarciaFixtures.InsightProfile,
            Scenarios = scenarios,
            FastestWinId = fastestWinId,
            DangerSignals = AlexGarciaFixtures.InsightProfile.DangerSignals,
            CorrectionCandidates = new List<CorrectionCandidate>(),
            AnalysisDate = DateTime.UtcNow,
        };

        return Task.FromResult(response);
    }

    public RunwayComputeScenariosResponse ComputeScenarios(RunwayComputeScenariosRequest request)
    {
        var state = request.State;
        var baselineDays = _engine.ComputeBaseline(state);
        var baselineZone = _engine.GetZone(baselineDays);

        // Merge custom scenario into the list if provided
        var scenarios = new List<Scenario>(request.Scenarios);
        if (request.CustomScenario is not null)
        {
            scenarios.Add(request.CustomScenario);
        }

        // Compute ScenarioWithDelta for each scenario
        var scenarioDeltas = scenarios.Select(s =>
        {
            var delta = _engine.ComputeDelta(s, state);
            var newDays = baselineDays + delta;
            var newZone = _engine.GetZone(newDays);
            return new ScenarioWithDelta
            {
                Id = s.Id,
                Delta = delta,
                NewDays = newDays,
                NewZone = newZone,
            };
        }).ToList();

        // Compute stacked result: sum of active scenario deltas, additive to baseline
        var activeIds = new HashSet<string>(request.ActiveScenarioIds);
        var stackedDelta = scenarioDeltas
            .Where(sd => activeIds.Contains(sd.Id))
            .Sum(sd => sd.Delta);
        var stackedDays = baselineDays + stackedDelta;
        var stackedZone = _engine.GetZone(stackedDays);
        var stackedDate = _engine.DaysToDate(stackedDays, DateTime.UtcNow);

        // Reverse mode: find fastest path to target
        List<string>? reverseModeIds = null;
        if (request.ReverseTarget.HasValue)
        {
            var fastestPath = _engine.FindFastestPath(request.ReverseTarget.Value, state, scenarios);
            reverseModeIds = fastestPath.Select(s => s.Id).ToList();
        }

        return new RunwayComputeScenariosResponse
        {
            BaselineDays = baselineDays,
            BaselineZone = baselineZone,
            StackedDays = stackedDays,
            StackedDelta = stackedDelta,
            StackedZone = stackedZone,
            StackedDate = stackedDate,
            ScenarioDeltas = scenarioDeltas,
            ReverseModeIds = reverseModeIds,
        };
    }
}
