using Refit;

namespace Hackathon.Bff.Integrations.ApiService;

public class SayHelloResponse
{
    public required string Message { get; set; }
}

[Headers("Accept: application/json")]
public interface IApiServiceClient
{
    [Get("/api/v1/say-hello")]
    Task<SayHelloResponse> SayHelloAsync([Query] string name, CancellationToken cancellationToken = default);
}
