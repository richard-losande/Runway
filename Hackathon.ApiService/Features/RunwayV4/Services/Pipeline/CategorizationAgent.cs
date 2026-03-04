using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services.Pipeline;

public interface ICategorizationAgent
{
    Task CategorizeAsync(List<Transaction> unresolved, CancellationToken ct);
}

public class CategorizationAgent : ICategorizationAgent
{
    private const int BatchSize = 50;
    private const int TimeoutSeconds = 10;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static readonly Dictionary<string, CategoryKey> CategoryMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["food_dining"] = CategoryKey.FoodDining,
        ["groceries"] = CategoryKey.Groceries,
        ["bills_utilities"] = CategoryKey.BillsUtilities,
        ["transport"] = CategoryKey.Transport,
        ["shopping"] = CategoryKey.Shopping,
        ["health_wellness"] = CategoryKey.HealthWellness,
        ["housing"] = CategoryKey.Housing,
        ["transfers"] = CategoryKey.Transfers,
        ["entertainment_subs"] = CategoryKey.EntertainmentSubs,
        ["government_deductions"] = CategoryKey.GovernmentDeductions,
        ["misc"] = CategoryKey.Misc,
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<CategorizationAgent> _logger;

    public CategorizationAgent(HttpClient httpClient, ILogger<CategorizationAgent> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task CategorizeAsync(List<Transaction> unresolved, CancellationToken ct)
    {
        if (unresolved.Count == 0) return;

        // Process in batches of 50
        for (int i = 0; i < unresolved.Count; i += BatchSize)
        {
            var batch = unresolved.Skip(i).Take(BatchSize).ToList();
            await CategorizeBatchAsync(batch, ct);
        }
    }

    private async Task CategorizeBatchAsync(List<Transaction> batch, CancellationToken ct)
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var systemPrompt = BuildSystemPrompt();
            var userPrompt = BuildUserPrompt(batch);

            var chatRequest = new
            {
                model = "gpt-4o-mini",
                temperature = 0,
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
                _logger.LogWarning("Empty response from categorization agent, falling back to Misc");
                FallbackToMisc(batch);
                return;
            }

            ApplyResults(batch, messageContent);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Categorization batch timed out, falling back to Misc");
            FallbackToMisc(batch);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Categorization API request failed, retrying once");
            await RetryCategorizeBatchAsync(batch, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in categorization, falling back to Misc");
            FallbackToMisc(batch);
        }
    }

    private async Task RetryCategorizeBatchAsync(List<Transaction> batch, CancellationToken ct)
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var chatRequest = new
            {
                model = "gpt-4o-mini",
                temperature = 0,
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                    new { role = "system", content = BuildSystemPrompt() },
                    new { role = "user", content = BuildUserPrompt(batch) },
                },
            };

            var jsonBody = JsonSerializer.Serialize(chatRequest, JsonOptions);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("v1/chat/completions", httpContent, linkedCts.Token);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync(linkedCts.Token);
            var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(responseBody, JsonOptions);

            var messageContent = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            if (!string.IsNullOrWhiteSpace(messageContent))
            {
                ApplyResults(batch, messageContent);
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Categorization retry also failed");
        }

        FallbackToMisc(batch);
    }

    private void ApplyResults(List<Transaction> batch, string jsonContent)
    {
        try
        {
            var wrapper = JsonSerializer.Deserialize<CategorizationWrapper>(jsonContent, JsonOptions);
            var results = wrapper?.Results;

            if (results is null || results.Count == 0)
            {
                FallbackToMisc(batch);
                return;
            }

            var resultMap = results.ToDictionary(r => r.Id, r => r, StringComparer.OrdinalIgnoreCase);

            foreach (var tx in batch)
            {
                if (resultMap.TryGetValue(tx.Id, out var result))
                {
                    if (CategoryMap.TryGetValue(result.Category, out var categoryKey))
                    {
                        tx.Category = categoryKey;
                    }
                    else
                    {
                        tx.Category = CategoryKey.Misc;
                    }
                    tx.Confidence = result.Confidence;
                    tx.Merchant = result.Merchant;
                }
                else
                {
                    tx.Category = CategoryKey.Misc;
                    tx.Confidence = 0.0;
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse categorization results");
            FallbackToMisc(batch);
        }
    }

    private static void FallbackToMisc(List<Transaction> batch)
    {
        foreach (var tx in batch)
        {
            tx.Category = CategoryKey.Misc;
            tx.Confidence = 0.0;
            tx.Merchant = "Unknown";
        }
    }

    private static string BuildSystemPrompt()
    {
        return """
            You are a financial transaction categoriser for users in the Philippines. Given a list of bank transaction descriptions, assign each to exactly one category from the provided list. Return only valid JSON.

            Categories:
            - food_dining (restaurants, food delivery, coffee shops)
            - groceries (supermarkets, wet market, convenience stores)
            - bills_utilities (electricity, water, internet, phone, streaming bills)
            - transport (ride-hailing, fuel, transit top-up, toll)
            - shopping (retail, online marketplace, clothing, electronics)
            - health_wellness (pharmacy, clinic, hospital, HMO premium, gym)
            - housing (rent, condo dues, HOA, property management)
            - transfers (money transfers, remittances, padala)
            - entertainment_subs (streaming, gaming, events, activities)
            - government_deductions (SSS, PhilHealth, Pag-IBIG/HDMF, BIR withholding tax, government statutory contributions)
            - misc (cannot determine — use sparingly)

            Rules:
            1. Philippine context: Grab Food = food_dining, Angkas = transport, Mercury Drug = health_wellness, Palawan Express = transfers
            2. If description contains a person's name + amount, classify as transfers
            3. Large one-time amounts (>₱10,000) to individuals: likely housing (rent)
            4. ATM withdrawals: always misc
            5. Confidence below 0.7: still assign best guess, set confidence accordingly

            Return JSON in this exact format:
            {"results": [{"id": "tx_001", "category": "food_dining", "confidence": 0.95, "merchant": "Grab Food"}, ...]}
            """;
    }

    private static string BuildUserPrompt(List<Transaction> batch)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Categorise these transactions:");
        foreach (var tx in batch)
        {
            sb.AppendLine($"{tx.Id}: \"{tx.NormDesc}\" \u20b1{tx.Amount:N2}");
        }
        return sb.ToString();
    }

    // ── Response DTOs ─────────────────────────────────────────────────────

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

    private class CategorizationWrapper
    {
        public List<CategorizationResult>? Results { get; set; }
    }

    private class CategorizationResult
    {
        public string Id { get; set; } = string.Empty;
        public string Category { get; set; } = "misc";
        public double Confidence { get; set; }
        public string? Merchant { get; set; }
    }
}
