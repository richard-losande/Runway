using FastEndpoints;
using Hackathon.ApiService.Features.Runway.Models;
using Hackathon.ApiService.Features.Runway.Services;

namespace Hackathon.ApiService.Features.Runway;

public class ProfileEndpoint : Endpoint<RunwayProfileRequest, Sp3Output>
{
    private readonly IRunwayOrchestrator _orchestrator;

    public ProfileEndpoint(IRunwayOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public override void Configure()
    {
        Post("/api/v1/runway/profile");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayProfileRequest req, CancellationToken ct)
    {
        var result = await _orchestrator.RevealProfileAsync(req, ct);
        await Send.OkAsync(result, cancellation: ct);
    }
}
