using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Hackathon.ApiService.Features.Runway.Models;

namespace Hackathon.ApiService.Features.Runway.Services;

public class BehavioralIntelligence : IBehavioralIntelligence
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly ILogger<BehavioralIntelligence> _logger;

    private static readonly Dictionary<string, string> FallbackDiagnoses = new()
    {
        ["Lifestyle Inflator"] = "Your lifestyle spend has been growing faster than your income for months. The gap is small now \u2014 but it compounds. One focused cut adds weeks to your runway.",
        ["Stability Builder"] = "You\u2019re doing the hard thing right. Low discretionary spend and a healthy income buffer puts you in the top tier of financial resilience.",
        ["Spending Accelerator"] = "Your fastest-growing expense category is accelerating every month. If the trend holds, it will materially reduce your runway within 2 months.",
        ["Balanced Spender"] = "Your spending is well-distributed across categories with no single runaway pattern. Small optimizations in variable costs would push you into Strong territory.",
    };

    public BehavioralIntelligence(HttpClient httpClient, IConfiguration configuration, ILogger<BehavioralIntelligence> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        var apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: OpenAI:ApiKey");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o";

        _httpClient.BaseAddress = new Uri("https://api.openai.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public static string ClassifyArchetype(double elasticityScore, double incomeToBurnRatio, List<DangerSignal> dangerSignals)
    {
        if (elasticityScore > 0.50 && incomeToBurnRatio < 1.20)
            return "Lifestyle Inflator";
        if (elasticityScore < 0.30 && incomeToBurnRatio > 1.50)
            return "Stability Builder";
        if (dangerSignals.Count > 0 && dangerSignals[0].MonthlyGrowthRate > 0.30)
            return "Spending Accelerator";
        return "Balanced Spender";
    }

    public async Task<Sp3Output> DiagnoseAsync(Sp3Input input, CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(8));
            return await CallOpenAiAsync(input, cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SP3 OpenAI call failed, using fallback for archetype: {Archetype}", input.Archetype);
            return GetFallback(input.Archetype);
        }
    }

    private async Task<Sp3Output> CallOpenAiAsync(Sp3Input input, CancellationToken ct)
    {
        var systemMessage = @"You are a financial behavioral analyst.
You will receive a user's financial profile and a pre-classified archetype. Your job is to:

1. Write 2-3 sentences explaining WHY this archetype fits, using specific numbers from the data.
2. Write one clear, specific recommendation - the single highest-impact action this person can take.
3. End with one short sentence that is direct and slightly provocative.

Rules:
- Use the actual numbers. Never be generic.
- Do not soften the diagnosis.
- Sound like a smart friend, not a bank.
- Total response must be under 120 words.
- Return valid JSON only. No explanation outside the JSON.
- JSON schema: { ""archetype"": ""string"", ""diagnosis"": ""string"", ""top_recommendation"": ""string"", ""closing_line"": ""string"" }";

        var topDanger = input.DangerSignals.FirstOrDefault();
        var userMessage = $@"Archetype: {input.Archetype}

Financial Profile:
- Survival days: {input.BaselineSurvivalDays}
- Monthly burn: \u20b1{input.MonthlyBurn:N0}
- Fixed: \u20b1{input.BurnBreakdown.Fixed:N0} | Variable: \u20b1{input.BurnBreakdown.Variable:N0} | Lifestyle: \u20b1{input.BurnBreakdown.Discretionary:N0}
- Elasticity score: {input.ElasticityScore:F2}
  (percentage of burn that is cuttable)
- Fastest growing category: {topDanger?.Category ?? "N/A"}
  at {(topDanger?.MonthlyGrowthRate ?? 0) * 100:F0}% monthly growth
- Highest-impact scenario: {input.TopScenario.Label}
  adds {input.TopScenario.DeltaDays} survival days

Generate the behavioral diagnosis.";

        var requestBody = new
        {
            model = _model,
            messages = new object[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage },
            },
            response_format = new { type = "json_object" },
            temperature = 0.4,
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("v1/chat/completions", content, ct);
        var responseBody = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("SP3 OpenAI API error: {StatusCode} {Body}", response.StatusCode, responseBody);
            throw new InvalidOperationException($"OpenAI API returned {response.StatusCode}");
        }

        using var doc = JsonDocument.Parse(responseBody);
        var messageContent = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString()
            ?? throw new InvalidOperationException("OpenAI returned empty content");

        var result = JsonSerializer.Deserialize<Sp3Output>(messageContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }) ?? throw new InvalidOperationException("Failed to deserialize SP3 response");

        return result;
    }

    private static Sp3Output GetFallback(string archetype)
    {
        var diagnosis = FallbackDiagnoses.GetValueOrDefault(archetype, FallbackDiagnoses["Balanced Spender"]);
        return new Sp3Output
        {
            Archetype = archetype,
            Diagnosis = diagnosis,
            TopRecommendation = "Review your highest-growth spending category and set a monthly cap.",
            ClosingLine = "The best time to fix this was last month. The second best time is now.",
        };
    }
}
