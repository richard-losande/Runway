namespace Hackathon.ApiService.Integrations.SproutPayroll;

public class SproutPayrollDelegatingHandler : DelegatingHandler
{
    private readonly string _apiKey;

    public SproutPayrollDelegatingHandler(IConfiguration configuration)
    {
        _apiKey = configuration["SproutPayroll:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: SproutPayroll:ApiKey");
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("x-api-key", _apiKey);
        request.Headers.Add("x-payroll-PayrollPieDatabase", "Payroll_GC03");
        return base.SendAsync(request, cancellationToken);
    }
}
