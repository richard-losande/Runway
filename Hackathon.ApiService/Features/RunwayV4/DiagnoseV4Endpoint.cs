using FastEndpoints;
using Hackathon.ApiService.Features.RunwayV4.Models;
using Hackathon.ApiService.Features.RunwayV4.Services;

namespace Hackathon.ApiService.Features.RunwayV4;

public class DiagnoseV4Endpoint : Endpoint<RunwayDiagnoseRequest, RunwayDiagnoseResponse>
{
    private readonly IDiagnosisNarrativeAgent _agent;

    public DiagnoseV4Endpoint(IDiagnosisNarrativeAgent agent) => _agent = agent;

    public override void Configure()
    {
        Post("/api/v1/runway-v4/diagnose");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RunwayDiagnoseRequest req, CancellationToken ct)
    {
        var diagnosis = await _agent.GenerateAsync(req, ct);
        await Send.OkAsync(new RunwayDiagnoseResponse { Diagnosis = diagnosis }, cancellation: ct);
    }
}
