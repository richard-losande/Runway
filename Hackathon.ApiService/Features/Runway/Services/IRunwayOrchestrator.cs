using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface IRunwayOrchestrator
{
    Task<RunwayAnalyzeResponse> AnalyzeAsync(RunwayAnalyzeRequest request, CancellationToken ct);
    Sp2Output RecalculateScenarios(RunwayScenariosRequest request);
    Task<Sp3Output> RevealProfileAsync(RunwayProfileRequest request, CancellationToken ct);
}
