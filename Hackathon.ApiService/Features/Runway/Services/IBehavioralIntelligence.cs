using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface IBehavioralIntelligence
{
    Task<Sp3Output> DiagnoseAsync(Sp3Input input, CancellationToken ct);
}
