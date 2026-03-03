using FastEndpoints;
using Hackathon.Bff.Integrations.ApiService;
using Refit;

namespace Hackathon.Bff.Features.FinancialRunway;

public class AnalyzeRequest
{
    public IFormFile CsvFile { get; set; } = null!;
    public decimal MonthlySalary { get; set; }
    public decimal TotalSavings { get; set; }
    public string? LifeEventsJson { get; set; }
}

public class AnalyzeEndpoint : Endpoint<AnalyzeRequest, FinancialRunwayResponse>
{
    private readonly IApiServiceClient _apiServiceClient;

    public AnalyzeEndpoint(IApiServiceClient apiServiceClient)
    {
        _apiServiceClient = apiServiceClient;
    }

    public override void Configure()
    {
        Post("/api/v1/financial-runway/analyze");
        AllowFileUploads();
    }

    public override async Task HandleAsync(AnalyzeRequest req, CancellationToken ct)
    {
        var stream = req.CsvFile.OpenReadStream();
        var streamPart = new StreamPart(stream, req.CsvFile.FileName, req.CsvFile.ContentType);

        var result = await _apiServiceClient.AnalyzeFinancialRunwayAsync(
            streamPart, req.MonthlySalary, req.TotalSavings, req.LifeEventsJson, ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}
