using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class ComputeScenariosV4Endpoint : Endpoint<RunwayComputeScenariosRequest, RunwayComputeScenariosResponse>
{
    private readonly IRunwayV4Orchestrator _orchestrator;

    public ComputeScenariosV4Endpoint(IRunwayV4Orchestrator orchestrator) => _orchestrator = orchestrator;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/compute-scenarios");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayComputeScenariosRequest req, CancellationToken ct)
    {
        var result = _orchestrator.ComputeScenarios(req);
        await Send.OkAsync(result, cancellation: ct);
    }
}
