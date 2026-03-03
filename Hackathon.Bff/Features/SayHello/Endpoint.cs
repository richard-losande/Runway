using FastEndpoints;
using Hackathon.Bff.Integrations.ApiService;

namespace Hackathon.Bff.Features.SayHello;

public class Request
{
    public required string Name { get; set; }
}

public class Response
{
    public required string Message { get; set; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IApiServiceClient _apiServiceClient;

    public Endpoint(IApiServiceClient apiServiceClient)
    {
        _apiServiceClient = apiServiceClient;
    }

    public override void Configure() => Get("/api/v1/say-hello");

    public override async Task HandleAsync(Request request, CancellationToken ct)
    {
        var result = await _apiServiceClient.SayHelloAsync(request.Name, ct);
        await Send.OkAsync(new Response { Message = result.Message }, cancellation: ct);
    }
}
