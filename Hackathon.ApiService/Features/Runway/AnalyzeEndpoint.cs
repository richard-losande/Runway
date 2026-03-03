using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class AnalyzeEndpoint : Endpoint<RunwayAnalyzeRequest, RunwayAnalyzeResponse>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public AnalyzeEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/analyze");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayAnalyzeRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.AnalyzeAsync(req, ct);
        await SendOkAsync(result, ct);
    }
}
