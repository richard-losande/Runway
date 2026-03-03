using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface ITransactionIntelligence
{
    Task<Sp1Output> AnalyzeAsync(Sp1Input input, CancellationToken ct);
}
