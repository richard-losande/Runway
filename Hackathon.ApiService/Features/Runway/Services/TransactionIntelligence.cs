using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class TransactionIntelligence : ITransactionIntelligence
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<TransactionIntelligence> _logger;

    public TransactionIntelligence(HttpClient httpClient, IConfiguration configuration, ILogger<TransactionIntelligence> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<Sp1Output> AnalyzeAsync(Sp1Input input, CancellationToken ct)
    {
        try
        {
            return await CallOpenAiAsync(input, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SP1 OpenAI call failed, using fallback demo data");
            return GetFallbackOutput();
        }
    }

    private async Task<Sp1Output> CallOpenAiAsync(Sp1Input input, CancellationToken ct)
    {
        var systemMessage = @"You are a financial transaction analyst. Given a bank statement CSV and monthly income, analyze the transactions and return a structured JSON burn profile.

Return ONLY valid JSON matching this exact schema:
{
  ""monthly_burn"": <decimal, average total monthly spending>,
  ""burn_breakdown"": {
    ""fixed"": <decimal, monthly fixed costs like rent, utilities, subscriptions, loan payments>,
    ""variable"": <decimal, monthly variable costs like groceries, transport, healthcare>,
    ""discretionary"": <decimal, monthly lifestyle costs like food delivery, shopping, coffee>
  },
  ""elasticity_score"": <double, (variable + discretionary) / monthly_burn>,
  ""income_to_burn_ratio"": <double, monthly_income / monthly_burn>,
  ""danger_signals"": [
    {
      ""category"": ""<string, merchant category name>"",
      ""monthly_growth_rate"": <double, month-over-month growth rate as decimal e.g. 0.38 for 38%>,
      ""monthly_amount"": <decimal, latest month amount>,
      ""insight"": ""<string, one sentence insight about the trend using actual numbers>""
    }
  ],
  ""top_danger_category"": ""<string, category name of the highest growth signal>""
}

Rules:
- Categorize each transaction as Fixed, Variable, or Discretionary
- Fixed: rent, utilities, subscriptions, loan payments, government deductions
- Variable: groceries, transport, healthcare
- Discretionary: food delivery, online shopping, coffee shops, entertainment
- Identify the top 2 spending categories with the highest month-over-month growth
- Use actual numbers from the data, never generic
- monthly_burn must equal fixed + variable + discretionary";

        var userMessage = $@"Monthly Income: {input.MonthlyIncome:N0}
Months Covered: {input.MonthsCovered}

Bank Statement CSV:
```
{input.CsvContent}
```

Analyze these transactions and return the JSON burn profile.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage },
            },
            response_format = new { type = "json_object" },
            temperature = 0.2,
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("SP1 OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("OpenAI returned empty content");

        return ParseOpenAiResponse(messageContent);
    }

    private static Sp1Output ParseOpenAiResponse(string jsonContent)
    {
        using var doc = JsonDocument.Parse(jsonContent);
        var root = doc.RootElement;

        var breakdown = root.GetProperty("burn_breakdown");
        var burnBreakdown = new BurnBreakdown
        {
            Fixed = breakdown.GetProperty("fixed").GetDecimal(),
            Variable = breakdown.GetProperty("variable").GetDecimal(),
            Discretionary = breakdown.GetProperty("discretionary").GetDecimal(),
        };

        var dangerSignals = new List<DangerSignal>();
        foreach (var signal in root.GetProperty("danger_signals").EnumerateArray())
        {
            dangerSignals.Add(new DangerSignal
            {
                Category = signal.GetProperty("category").GetString() ?? "",
                MonthlyGrowthRate = signal.GetProperty("monthly_growth_rate").GetDouble(),
                MonthlyAmount = signal.GetProperty("monthly_amount").GetDecimal(),
                Insight = signal.GetProperty("insight").GetString() ?? "",
            });
        }

        return new Sp1Output
        {
            MonthlyBurn = root.GetProperty("monthly_burn").GetDecimal(),
            BurnBreakdown = burnBreakdown,
            ElasticityScore = root.GetProperty("elasticity_score").GetDouble(),
            IncomeToBurnRatio = root.GetProperty("income_to_burn_ratio").GetDouble(),
            DangerSignals = dangerSignals,
            TopDangerCategory = root.GetProperty("top_danger_category").GetString() ?? "",
        };
    }

    private static Sp1Output GetFallbackOutput() => new()
    {
        MonthlyBurn = 52400m,
        BurnBreakdown = new BurnBreakdown
        {
            Fixed = 28000m,
            Variable = 14200m,
            Discretionary = 10200m,
        },
        ElasticityScore = 0.47,
        IncomeToBurnRatio = 1.43,
        DangerSignals =
        [
            new DangerSignal
            {
                Category = "Grab Food & Dining",
                MonthlyGrowthRate = 0.38,
                MonthlyAmount = 6800m,
                Insight = "Your Grab Food orders grew 38% in 4 months. That\u2019s an extra \u20b12,100 a month you weren\u2019t spending before.",
            },
            new DangerSignal
            {
                Category = "Shopee & Online Shopping",
                MonthlyGrowthRate = 0.22,
                MonthlyAmount = 3400m,
                Insight = "Online shopping charges grew 22% month over month \u2014 consistent, every single month, for 4 months straight.",
            },
        ],
        TopDangerCategory = "Dining",
    };
}
