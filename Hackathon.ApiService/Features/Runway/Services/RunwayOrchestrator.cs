using System.Reflection;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class RunwayOrchestrator : IRunwayOrchestrator
{
    private readonly ITransactionIntelligence _sp1;
    private readonly ISurvivalSimulator _sp2;
    private readonly IBehavioralIntelligence _sp3;
    private readonly ILogger<RunwayOrchestrator> _logger;

    public RunwayOrchestrator(
        ITransactionIntelligence sp1,
        ISurvivalSimulator sp2,
        IBehavioralIntelligence sp3,
        ILogger<RunwayOrchestrator> logger)
    {
        _sp1 = sp1;
        _sp2 = sp2;
        _sp3 = sp3;
        _logger = logger;
    }

    public async Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct)
    {
        // Step 1: Get CSV content
        string csvContent;
        if (request.UseDemoData || request.CsvFile is null)
        {
            csvContent = LoadDemoCsv();
        }
        else
        {
            using var reader = new StreamReader(request.CsvFile.OpenReadStream());
            csvContent = await reader.ReadToEndAsync(ct);
        }

        // Step 2: Run SP1 — Transaction Intelligence
        _logger.LogInformation("Running SP1 Transaction Intelligence...");
        var sp1Result = await _sp1.AnalyzeAsync(new Sp1Input
        {
            CsvContent = csvContent,
            MonthlyIncome = request.MonthlyIncome,
            MonthsCovered = 4,
        }, ct);

        // Step 3: Routing decision — data-driven, not pre-wired
        var priorityScenario = Route(sp1Result.TopDangerCategory, sp1Result.IncomeToBurnRatio);
        _logger.LogInformation("Routing decision: top_danger={Category}, ratio={Ratio}, priority={Scenario}",
            sp1Result.TopDangerCategory, sp1Result.IncomeToBurnRatio, priorityScenario);

        // Step 4: Run SP2 — Survival & Scenario Simulator
        _logger.LogInformation("Running SP2 Survival Simulator...");
        var sp2Result = _sp2.Calculate(new Sp2Input
        {
            MonthlyBurn = sp1Result.MonthlyBurn,
            BurnBreakdown = sp1Result.BurnBreakdown,
            MonthlyIncome = request.MonthlyIncome,
            LiquidSavings = request.LiquidSavings,
            PriorityScenario = priorityScenario,
            ActiveScenarios = [],
        });

        return new RunwayAnalyzeResponse
        {
            Sp1 = sp1Result,
            Sp2 = sp2Result,
        };
    }

    public Sp2Output RecalculateScenarios(RunwayScenariosRequest request)
    {
        return _sp2.Calculate(new Sp2Input
        {
            MonthlyBurn = request.MonthlyBurn,
            BurnBreakdown = request.BurnBreakdown,
            MonthlyIncome = request.MonthlyIncome,
            LiquidSavings = request.LiquidSavings,
            PriorityScenario = request.PriorityScenario,
            ActiveScenarios = request.ActiveScenarios,
        });
    }

    public async Task<Sp3Output> RevealProfileAsync(RunwayProfileRequest request, CancellationToken ct)
    {
        // Deterministic pre-classification
        var archetype = BehavioralIntelligence.ClassifyArchetype(
            request.ElasticityScore,
            request.IncomeToBurnRatio,
            request.DangerSignals);

        _logger.LogInformation("SP3 archetype classified: {Archetype}", archetype);

        return await _sp3.DiagnoseAsync(new Sp3Input
        {
            Archetype = archetype,
            ElasticityScore = request.ElasticityScore,
            IncomeToBurnRatio = request.IncomeToBurnRatio,
            DangerSignals = request.DangerSignals,
            BaselineSurvivalDays = request.BaselineSurvivalDays,
            TopScenario = request.TopScenario,
            BurnBreakdown = request.BurnBreakdown,
            MonthlyBurn = request.MonthlyBurn,
        }, ct);
    }

    /// <summary>
    /// Data-driven routing decision. The orchestrator reads SP1's output
    /// and decides which scenario SP2 surfaces first. This is the agentic
    /// behavior — the system observes data and makes a decision.
    /// </summary>
    private static string Route(string topDangerCategory, double incomeToBurnRatio)
    {
        if (new[] { "Dining", "Food", "Entertainment", "Shopping", "Transport", "Ride-share" }
            .Contains(topDangerCategory, StringComparer.OrdinalIgnoreCase))
            return "cut_lifestyle";
        if (incomeToBurnRatio < 1.10)
            return "side_hustle";
        return "cut_lifestyle";
    }

    private static string LoadDemoCsv()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("AlexTransactions.csv"))
            ?? throw new InvalidOperationException("Demo CSV not found as embedded resource");

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
