using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Hackathon.ApiService.Features.RunwayV4.DemoData;
using Hackathon.ApiService.Features.RunwayV4.Models;

namespace Hackathon.ApiService.Features.RunwayV4.Services;

public interface IDiagnosisNarrativeAgent
{
    Task<DiagnosisContent> GenerateAsync(RunwayDiagnoseRequest request, CancellationToken ct);
}

public class DiagnosisNarrativeAgent : IDiagnosisNarrativeAgent
{
    private static readonly Regex BannedWordsRegex = new(
        @"\b(crisis|danger|urgent|warning|overspending|irresponsible|reckless)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private const int MaxWhatIsHappening = 280;
    private const int MaxWhatToDoAboutIt = 240;
    private const int MaxHonestTake = 180;

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiagnosisNarrativeAgent> _logger;
    private readonly bool _demoMode;
    private readonly string _model;

    public DiagnosisNarrativeAgent(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DiagnosisNarrativeAgent> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        _demoMode = configuration.GetValue("DemoMode", true);
        _model = configuration.GetValue<string>("OpenAI:Model") ?? "gpt-4o";

        // BaseAddress and Authorization are configured in Program.cs via AddHttpClient
    }

    public async Task<DiagnosisContent> GenerateAsync(RunwayDiagnoseRequest request, CancellationToken ct)
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

            var systemPrompt = BuildSystemPrompt();
            var userPrompt = BuildUserPrompt(request);

            var chatRequest = new
            {
                model = _model,
                temperature = 0.4,
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
                _logger.LogWarning("Empty response from OpenAI, using fallback");
                return GetFallback(request);
            }

            var diagnosis = JsonSerializer.Deserialize<DiagnosisContent>(messageContent, JsonOptions);
            if (diagnosis is null)
            {
                _logger.LogWarning("Failed to deserialize DiagnosisContent from OpenAI response, using fallback");
                return GetFallback(request);
            }

            // Post-processing: check for banned words and replace affected fields
            var fallback = GetFallback(request);

            if (ContainsBannedWord(diagnosis.WhatIsHappening))
            {
                _logger.LogInformation("Banned word found in WhatIsHappening, replacing with fallback");
                diagnosis.WhatIsHappening = fallback.WhatIsHappening;
            }

            if (ContainsBannedWord(diagnosis.WhatToDoAboutIt))
            {
                _logger.LogInformation("Banned word found in WhatToDoAboutIt, replacing with fallback");
                diagnosis.WhatToDoAboutIt = fallback.WhatToDoAboutIt;
            }

            if (ContainsBannedWord(diagnosis.HonestTake))
            {
                _logger.LogInformation("Banned word found in HonestTake, replacing with fallback");
                diagnosis.HonestTake = fallback.HonestTake;
            }

            // Truncate fields to char limits at sentence boundary
            diagnosis.WhatIsHappening = TruncateAtSentence(diagnosis.WhatIsHappening, MaxWhatIsHappening);
            diagnosis.WhatToDoAboutIt = TruncateAtSentence(diagnosis.WhatToDoAboutIt, MaxWhatToDoAboutIt);
            diagnosis.HonestTake = TruncateAtSentence(diagnosis.HonestTake, MaxHonestTake);

            // Ensure archetype name is set
            if (string.IsNullOrWhiteSpace(diagnosis.ArchetypeName))
            {
                diagnosis.ArchetypeName = request.InsightProfile.Archetype.Name;
            }

            return diagnosis;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Diagnosis generation timed out, using fallback");
            return GetFallback(request);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "OpenAI API request failed, using fallback");
            return GetFallback(request);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse OpenAI response, using fallback");
            return GetFallback(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in diagnosis generation, using fallback");
            return GetFallback(request);
        }
    }

    private DiagnosisContent GetFallback(RunwayDiagnoseRequest request)
    {
        return BuildFallbackDiagnosis(request);
    }

    internal static DiagnosisContent BuildFallbackDiagnosis(RunwayDiagnoseRequest request)
    {
        var state = request.State;
        var profile = request.InsightProfile;
        var days = request.BaselineDays;
        var zone = request.Zone;
        var burnFmt = Fmt(state.MonthlyBurn);
        var savingsFmt = Fmt(state.LiquidCash);

        // Zone-specific problem statements (per spec)
        var whatIsHappening = zone switch
        {
            ZoneName.Critical =>
                $"At \u20b1{burnFmt}/mo burn, your \u20b1{savingsFmt} buffer runs out in {days} days. You need breathing room today.",
            ZoneName.Fragile =>
                $"Your savings cover {days} days. One unexpected expense could push you into the red.",
            ZoneName.Stable =>
                $"At \u20b1{burnFmt}/mo burn, your \u20b1{savingsFmt} buffer runs out in {days} days. " +
                "The goal is to grow this before your spending pattern closes the gap.",
            ZoneName.Strong =>
                $"At {days} days, your buffer is solid. The opportunity now is to make this money work harder while you have room.",
            _ =>
                $"Your monthly burn is \u20b1{burnFmt} with {days} days of runway.",
        };

        var reductionDays = state.MonthlyBurn > 0
            ? (int)Math.Round(5000m / (state.MonthlyBurn / 30m))
            : 0;
        var whatToDoAboutIt =
            $"Your highest-impact move is to reduce your discretionary spend. " +
            $"Even a \u20b15,000 monthly reduction adds {reductionDays} days to your runway.";

        // Tone matches archetype (per spec)
        var honestTake = profile.Archetype.Key switch
        {
            ArchetypeKey.CrisisMode =>
                "Focus on one clear next action — every small step buys you time.",
            ArchetypeKey.LifestyleInflator =>
                "The gap between what comes in and what goes out has been quietly widening.",
            ArchetypeKey.SteadySpender =>
                "The habit is good — the missing piece is the buffer.",
            ArchetypeKey.ResilientSaver =>
                "Your buffer is growing. The next move is making it work harder.",
            _ =>
                "The number that matters most is the gap between what comes in and what goes out.",
        };

        return new DiagnosisContent
        {
            ArchetypeName = profile.Archetype.Name,
            WhatIsHappening = whatIsHappening,
            WhatToDoAboutIt = whatToDoAboutIt,
            HonestTake = honestTake,
        };
    }

    internal static string TruncateAtSentence(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
        {
            return text;
        }

        var truncated = text[..maxLength];
        var lastPeriod = truncated.LastIndexOf('.');

        if (lastPeriod > 0)
        {
            return truncated[..(lastPeriod + 1)];
        }

        // No sentence boundary found — return what we can
        return truncated;
    }

    private static bool ContainsBannedWord(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        return BannedWordsRegex.IsMatch(text);
    }

    private static string Fmt(decimal value)
    {
        return value.ToString("N0", CultureInfo.InvariantCulture);
    }

    private static string BuildSystemPrompt()
    {
        return """
            You are writing a personal financial resilience report for a Sprout payroll user in the Philippines. Your tone is direct, honest, and grounded — not alarming, not encouraging, not salesy. You write like a trusted friend who happens to know finance, not like a financial advisor covering liability. Generate a DiagnosisContent object as valid JSON only. No preamble.

            Rules for whatIsHappening:
            - Use the user's actual numbers. Every sentence must contain a specific figure.
            - Name the top growing merchant explicitly (e.g. "Grab Food alone is ₱10,200").
            - State the monthly gap in plain terms if burn > takeHome.
            - Do not editorialize. State facts.
            - 2–4 sentences. Max 280 characters.

            Rules for whatToDoAboutIt:
            - Reference the fastestWin scenario by its exact label.
            - State the before and after runway days explicitly.
            - Frame as one specific change, not a list.
            - 2–3 sentences. Max 240 characters.

            Rules for honestTake:
            - Do not repeat numbers from the sections above.
            - Name the underlying pattern (e.g. lifestyle inflation, income-spend gap).
            - No alarm language: never use "crisis", "danger", "urgent", "warning".
            - No shame language: never use "overspending", "irresponsible", "reckless".
            - Italicised in UI — write in a reflective, not instructional, register.
            - 1–2 sentences. Max 180 characters.

            If remittanceNote is present in InsightProfile:
            - Include it verbatim in whatIsHappening as a standalone sentence.
            - Never suggest cutting remittances in whatToDoAboutIt.

            Character limits are hard limits. If you exceed them, truncate cleanly at a sentence boundary — never mid-sentence.
            """;
    }

    private static string BuildUserPrompt(RunwayDiagnoseRequest request)
    {
        var state = request.State;
        var profile = request.InsightProfile;
        var gap = state.MonthlyBurn - state.TakeHome;
        var topTrend = profile.Trends.FirstOrDefault(t => t.Notable);

        var sb = new StringBuilder();
        sb.AppendLine($"Archetype: {profile.Archetype.Name}");
        sb.AppendLine($"Monthly Burn: \u20b1{Fmt(state.MonthlyBurn)}");
        sb.AppendLine($"Take-Home: \u20b1{Fmt(state.TakeHome)}");
        sb.AppendLine($"Monthly Gap: \u20b1{Fmt(gap)}");
        sb.AppendLine($"Baseline Days: {request.BaselineDays}");
        sb.AppendLine($"Zone: {request.Zone}");

        if (topTrend is not null)
        {
            sb.AppendLine($"Top Growing Category: {topTrend.Category}, +{topTrend.PctChange}%");
            sb.AppendLine($"Top Merchant: {topTrend.TopMerchant}, \u20b1{Fmt(topTrend.TopMerchantAmount)}");
        }

        sb.AppendLine($"Fastest Win: {request.FastestWinLabel}");
        sb.AppendLine($"Fastest Win Delta: +{request.FastestWinDelta} days");
        sb.AppendLine($"Fastest Win New Days: {request.FastestWinNewDays} days");

        if (!string.IsNullOrEmpty(profile.RemittanceNote))
        {
            sb.AppendLine($"Remittance Note: {profile.RemittanceNote}");
        }

        sb.AppendLine();
        sb.AppendLine("Respond with a JSON object containing: archetypeName, whatIsHappening, whatToDoAboutIt, honestTake.");

        return sb.ToString();
    }

    // ── OpenAI response DTOs ────────────────────────────────────────────

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
