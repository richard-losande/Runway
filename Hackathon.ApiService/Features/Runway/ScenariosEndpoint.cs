using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class ScenariosEndpoint : Endpoint<RunwayScenariosRequest, Sp2Output>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public ScenariosEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/scenarios");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayScenariosRequest req, CancellationToken ct)
    {
        var result = _orchestrator.RecalculateScenarios(req);
        await SendOkAsync(result, ct);
    }
}
