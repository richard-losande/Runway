using Refit;

namespace Hackathon.Bff.Integrations.ApiService;

public class SayHelloResponse
{
    public required string Message { get; set; }
}

public class FinancialRunwayResponse
{
    public int RunwayMonths { get; set; }
    public decimal MonthlyBurnRate { get; set; }
    public List<object> CategorizedExpenses { get; set; } = [];
    public List<object> MonthlyProjections { get; set; } = [];
    public List<object> LifeEventImpacts { get; set; } = [];
    public int AdjustedRunwayMonths { get; set; }
    public string Narrative { get; set; } = string.Empty;
    public DateTime AnalyzedAt { get; set; }
}

[Headers("Accept: application/json")]
public interface IApiServiceClient
{
    [Get("/api/v1/say-hello")]
    Task<SayHelloResponse> SayHelloAsync([Query] string name, CancellationToken cancellationToken = default);

    [Multipart]
    [Post("/api/v1/financial-runway/analyze")]
    Task<FinancialRunwayResponse> AnalyzeFinancialRunwayAsync(
        [AliasAs("csvFile")] StreamPart csvFile,
        [AliasAs("monthlySalary")] decimal monthlySalary,
        [AliasAs("totalSavings")] decimal totalSavings,
        [AliasAs("lifeEventsJson")] string? lifeEventsJson,
        CancellationToken cancellationToken = default);
}
