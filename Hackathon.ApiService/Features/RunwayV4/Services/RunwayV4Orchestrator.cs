using Hackathon.ApiService.Features.RunwayV4.DemoData;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IRunwayV4Orchestrator
{
    Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct);
    RunwayComputeScenariosResponse ComputeScenarios(RunwayComputeScenariosRequest request);
}

public class RunwayV4Orchestrator : IRunwayV4Orchestrator
{
    private readonly IRunwayEngine _engine;
    private readonly IFormatDetector _formatDetector;
    private readonly ITransactionNormalizer _normalizer;
    private readonly IMerchantLookup _merchantLookup;
    private readonly ICategorizationAgent _categorizationAgent;
    private readonly IAggregator _aggregator;
    private readonly IScenarioGenerator _scenarioGenerator;
    private readonly IInsightProfileBuilder _insightProfileBuilder;
    private readonly bool _demoMode;

    public RunwayV4Orchestrator(
        IRunwayEngine engine,
        IFormatDetector formatDetector,
        ITransactionNormalizer normalizer,
        IMerchantLookup merchantLookup,
        ICategorizationAgent categorizationAgent,
        IAggregator aggregator,
        IScenarioGenerator scenarioGenerator,
        IInsightProfileBuilder insightProfileBuilder,
        IConfiguration configuration)
    {
        _engine = engine;
        _formatDetector = formatDetector;
        _normalizer = normalizer;
        _merchantLookup = merchantLookup;
        _categorizationAgent = categorizationAgent;
        _aggregator = aggregator;
        _scenarioGenerator = scenarioGenerator;
        _insightProfileBuilder = insightProfileBuilder;
        _demoMode = configuration.GetValue("DemoMode", true);
    }

    public async Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // Real CSV pipeline: if a CSV file is provided and not using demo data
        if (request.CsvFile is not null && !request.UseDemoData)
        {
            return await RunCsvPipelineAsync(request, ct);
        }

        return BuildDemoResponse(request);
    }

    private async Task<RunwayAnalyzeResponse> RunCsvPipelineAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // Read CSV content
        string csvContent;
        using (var reader = new StreamReader(request.CsvFile!.OpenReadStream()))
        {
            csvContent = await reader.ReadToEndAsync(ct);
        }

        var allRows = TransactionNormalizer.ParseCsvLines(csvContent);
        if (allRows.Count < 2)
        {
            // Not enough data — fall back to demo
            return BuildDemoResponse(request);
        }

        // Stage 1: Format Detection
        var headerRow = allRows[0];
        var format = _formatDetector.Detect(headerRow);

        // Stage 2: Transaction Normalisation
        var dataRows = allRows.Skip(1).ToList();
        var transactions = _normalizer.Normalize(dataRows, format);

        if (transactions.Count == 0)
        {
            return BuildDemoResponse(request);
        }

        // Stage 3: Rule-Based Merchant Lookup
        foreach (var tx in transactions)
        {
            if (tx.Category.HasValue) continue; // Already classified (ATM, loan)

            var match = _merchantLookup.Match(tx.NormDesc);
            if (match is not null)
            {
                tx.Category = match.Category;
                tx.Merchant = match.MerchantName;
                tx.Confidence = match.Confidence;
            }
        }

        // Stage 4: OpenAI Fallback for unresolved
        var unresolved = transactions.Where(t => !t.Category.HasValue).ToList();
        if (unresolved.Count > 0)
        {
            await _categorizationAgent.CategorizeAsync(unresolved, ct);
        }

        // Ensure all transactions have a category
        foreach (var tx in transactions)
        {
            tx.Category ??= CategoryKey.Misc;
        }

        // Stage 5: Aggregation
        var aggregation = _aggregator.Aggregate(transactions);

        // Build state — credits from CSV add to take-home income
        var takeHome = request.MonthlyIncome > 0 ? request.MonthlyIncome : 0;
        if (aggregation.MonthlyCredits > 0)
        {
            takeHome += aggregation.MonthlyCredits;
        }

        var state = new RunwayState
        {
            LiquidCash = request.LiquidSavings > 0 ? request.LiquidSavings : 0,
            MonthlyBurn = aggregation.MonthlyBurn,
            TakeHome = takeHome,
            Categories = aggregation.Categories.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.MonthlyAverage),
        };

        var baselineDays = _engine.ComputeBaseline(state);
        var zone = _engine.GetZone(baselineDays);

        // Build InsightProfile from real data
        var insightProfile = _insightProfileBuilder.Build(aggregation.Categories, state, baselineDays);

        // Generate scenarios from real data
        var scenarios = _scenarioGenerator.Generate(aggregation.Categories, state, insightProfile);

        // Compute delta for each scenario
        foreach (var scenario in scenarios)
        {
            scenario.Delta = _engine.ComputeDelta(scenario, state);
        }

        var fastestWinId = _engine.FindFastestWinId(scenarios, state);

        return new RunwayAnalyzeResponse
        {
            State = state,
            BaselineDays = baselineDays,
            Zone = zone,
            Categories = aggregation.Categories,
            InsightProfile = insightProfile,
            Scenarios = scenarios,
            FastestWinId = fastestWinId,
            DangerSignals = insightProfile.DangerSignals,
            CorrectionCandidates = aggregation.CorrectionCandidates,
            AnalysisDate = DateTime.UtcNow,
        };
    }

    private RunwayAnalyzeResponse BuildDemoResponse(RunwayAnalyzeRequest request)
    {
        var state = AlexGarciaFixtures.State;
        state.LiquidCash = request.LiquidSavings > 0 ? request.LiquidSavings : state.LiquidCash;
        state.TakeHome = request.MonthlyIncome > 0 ? request.MonthlyIncome : state.TakeHome;

        var baselineDays = _engine.ComputeBaseline(state);
        var zone = _engine.GetZone(baselineDays);

        var scenarios = AlexGarciaFixtures.Scenarios;
        foreach (var scenario in scenarios)
        {
            scenario.Delta = _engine.ComputeDelta(scenario, state);
        }

        var fastestWinId = _engine.FindFastestWinId(scenarios, state);

        return new RunwayAnalyzeResponse
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
