using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class AnalyzeV4Endpoint : Endpoint<RunwayAnalyzeRequest, RunwayAnalyzeResponse>
{
    private readonly IRunwayV4Orchestrator _orchestrator;

    public AnalyzeV4Endpoint(IRunwayV4Orchestrator orchestrator) => _orchestrator = orchestrator;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/analyze");
        AllowAnonymous();
        AllowFileUploads();
    }

    public override async Task HandleAsync(RunwayAnalyzeRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.AnalyzeAsync(req, ct);
        await Send.OkAsync(result, cancellation: ct);
    }
}
