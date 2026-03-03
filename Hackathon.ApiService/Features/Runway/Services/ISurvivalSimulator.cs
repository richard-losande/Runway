using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public interface ISurvivalSimulator
{
    Sp2Output Calculate(Sp2Input input);
}
