using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface ICategorizationAgent
{
    Task CategorizeAsync(List<Transaction> transactions, CancellationToken ct);
}

public class CategorizationAgent : ICategorizationAgent
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly string[] ValidCategories = Enum.GetNames<CategoryKey>();

    private readonly HttpClient _httpClient;
    private readonly ILogger<CategorizationAgent> _logger;
    private readonly string _model;

    public CategorizationAgent(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<CategorizationAgent> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _model = configuration.GetValue<string>("OpenAI:Model") ?? "gpt-4o-mini";
    }

    public async Task CategorizeAsync(List<Transaction> transactions, CancellationToken ct)
    {
        if (transactions.Count == 0) return;

        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var descriptions = transactions.Select((t, i) => $"{i}: {t.NormDesc}").ToList();
            var categoriesStr = string.Join(", ", ValidCategories);

            var systemPrompt = $"""
                You categorize bank transaction descriptions into one of these categories: {categoriesStr}.
                Respond with a JSON object where keys are the line indices (as strings) and values are objects with "category" and "merchant" fields.
                "category" must be one of the valid categories listed above.
                "merchant" should be the best-guess merchant name.
                Only return the JSON object, no preamble.
                """;

            var userPrompt = string.Join("\n", descriptions);

            var chatRequest = new
            {
                model = _model,
                temperature = 0.1,
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt },
                },
            };

            var jsonBody = JsonSerializer.Serialize(chatRequest, JsonOptions);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("v1/chat/completions", httpContent, linkedCts.Token);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(linkedCts.Token);
            var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(responseBody, JsonOptions);

            var messageContent = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                _logger.LogWarning("Empty response from OpenAI categorization, assigning Misc");
                AssignMiscFallback(transactions);
                return;
            }

            var results = JsonSerializer.Deserialize<Dictionary<string, CategoryResult>>(messageContent, JsonOptions);
            if (results is null)
            {
                AssignMiscFallback(transactions);
                return;
            }

            for (int i = 0; i < transactions.Count; i++)
            {
                if (results.TryGetValue(i.ToString(), out var result) &&
                    Enum.TryParse<CategoryKey>(result.Category, true, out var cat))
                {
                    transactions[i].Category = cat;
                    transactions[i].Merchant = result.Merchant;
                    transactions[i].Confidence = 0.7;
                }
                else
                {
                    transactions[i].Category = CategoryKey.Misc;
                    transactions[i].Confidence = 0.0;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI categorization failed, assigning Misc to {Count} transactions", transactions.Count);
            AssignMiscFallback(transactions);
        }
    }

    private static void AssignMiscFallback(List<Transaction> transactions)
    {
        foreach (var tx in transactions)
        {
            tx.Category ??= CategoryKey.Misc;
            tx.Confidence ??= 0.0;
        }
    }

    private class CategoryResult
    {
        public string Category { get; set; } = string.Empty;
        public string? Merchant { get; set; }
    }

    private class ChatCompletionResponse
    {
        public List<ChatChoice>? Choices { get; set; }
    }

    private class ChatChoice
    {
        public ChatMessage? Message { get; set; }
    }

    private class ChatMessage
    {
        public string? Content { get; set; }
    }
}
