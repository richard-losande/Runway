using System.Text.Json;
using FastEndpoints;
using Hackathon.ApiService.Databases.DbContexts;

namespace Hackathon.ApiService.Features.FinancialRunway;

public class AnalyzeRequest
{
    public IFormFile CsvFile { get; set; } = null!;
    public decimal MonthlySalary { get; set; }
    public decimal TotalSavings { get; set; }
    public string? LifeEventsJson { get; set; }
}

public class AnalyzeEndpoint : Endpoint<AnalyzeRequest, AnalyzeResponse>
{
    private readonly IOpenAiService _openAiService;
    private readonly MainDbContext _dbContext;

    public AnalyzeEndpoint(IOpenAiService openAiService, MainDbContext dbContext)
    {
        _openAiService = openAiService;
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post("/api/v1/financial-runway/analyze");
        AllowFileUploads();
    }

    public override async Task HandleAsync(AnalyzeRequest req, CancellationToken ct)
    {
        // Read CSV content
        using var reader = new StreamReader(req.CsvFile.OpenReadStream());
        var csvContent = await reader.ReadToEndAsync(ct);

        // Parse life events
        var lifeEvents = new List<LifeEventInput>();
        if (!string.IsNullOrWhiteSpace(req.LifeEventsJson))
        {
            lifeEvents = JsonSerializer.Deserialize<List<LifeEventInput>>(req.LifeEventsJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }

        // Call OpenAI
        var result = await _openAiService.AnalyzeFinancialDataAsync(
            csvContent, req.MonthlySalary, req.TotalSavings, lifeEvents, ct);

        // Save to DB
        var entity = new FinancialAnalysis
        {
            Id = Guid.NewGuid(),
            MonthlySalary = req.MonthlySalary,
            TotalSavings = req.TotalSavings,
            RunwayMonths = result.RunwayMonths,
            AdjustedRunwayMonths = result.AdjustedRunwayMonths,
            MonthlyBurnRate = result.MonthlyBurnRate,
            ResponseJson = JsonSerializer.Serialize(result),
            AnalyzedAt = result.AnalyzedAt
        };

        _dbContext.FinancialAnalyses.Add(entity);
        await _dbContext.SaveChangesAsync(ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}
