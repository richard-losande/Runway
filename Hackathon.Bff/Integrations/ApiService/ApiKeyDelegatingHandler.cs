namespace Hackathon.Bff.Integrations.ApiService;

public class ApiKeyDelegatingHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public ApiKeyDelegatingHandler(IConfiguration configuration)
    {
        _apiKey = configuration["Auth:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: Auth:ApiKey");
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
