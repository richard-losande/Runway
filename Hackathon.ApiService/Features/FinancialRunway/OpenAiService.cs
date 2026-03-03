using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Hackathon.ApiService.Features.FinancialRunway;

public interface IOpenAiService
{
    Task<AnalyzeResponse> AnalyzeFinancialDataAsync(
        string csvContent,
        decimal monthlySalary,
        decimal totalSavings,
        List<LifeEventInput> lifeEvents,
        CancellationToken ct);
}

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<OpenAiService> _logger;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<AnalyzeResponse> AnalyzeFinancialDataAsync(
        string csvContent,
        decimal monthlySalary,
        decimal totalSavings,
        List<LifeEventInput> lifeEvents,
        CancellationToken ct)
    {
        var lifeEventsText = lifeEvents.Count > 0
            ? string.Join("\n", lifeEvents.Select(e =>
                $"- {e.Type}{(string.IsNullOrEmpty(e.Description) ? "" : $": {e.Description}")} in {e.MonthFromNow} months"))
            : "None";

        var systemMessage = @"You are a financial analyst. Given a bank statement CSV, monthly salary, savings, and planned life events, produce a structured JSON analysis of the user's financial runway.

Return ONLY valid JSON matching this exact schema:
{
  ""runwayMonths"": <int, months until savings depleted assuming current burn rate minus salary>,
  ""monthlyBurnRate"": <decimal, average monthly spending from CSV>,
  ""categorizedExpenses"": [{ ""category"": ""<string>"", ""monthlyAverage"": <decimal>, ""percentage"": <decimal 0-100> }],
  ""monthlyProjections"": [{ ""month"": <int 1-12>, ""balance"": <decimal>, ""income"": <decimal>, ""expenses"": <decimal> }],
  ""lifeEventImpacts"": [{ ""event"": ""<string>"", ""impactOnRunway"": <int, months added or removed>, ""newMonthlyExpense"": <decimal> }],
  ""adjustedRunwayMonths"": <int, runway after all life events>,
  ""narrative"": ""<string, 2-3 paragraph analysis with actionable advice>""
}";

        var userMessage = $@"## Financial Data

**Monthly Salary:** {monthlySalary:C}
**Total Savings:** {totalSavings:C}

## Bank Statement CSV
```
{csvContent}
```

## Planned Life Events
{lifeEventsText}

Analyze this data and return the JSON analysis.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            response_format = new { type = "json_object" },
            temperature = 0.3
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrEmpty(messageContent))
            throw new InvalidOperationException("OpenAI returned empty content");

        var result = JsonSerializer.Deserialize<AnalyzeResponse>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize OpenAI response");

        result.AnalyzedAt = DateTime.UtcNow;
        return result;
    }
}
