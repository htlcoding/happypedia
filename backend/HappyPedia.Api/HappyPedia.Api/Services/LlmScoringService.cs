using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HappyPedia.Api.Services;

public class LlmScoringService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LlmScoringService> _logger;

    public LlmScoringService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<LlmScoringService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<double> CalculateScoreAsync(string title, string description, string source)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        var model = _configuration["OpenAI:Model"] ?? "gpt-4.1-nano";

        if (string.IsNullOrWhiteSpace(apiKey))
            return 1;

        var payload = new
        {
            model,
            instructions =
                "Bewerte, wie gut dieser Artikel für eine Good-News-Plattform passt. " +
                "Gib NUR einen Score zwischen 1 und 10 zurück. " +
                "1 = sehr negativ / schlechte Nachricht, " +
                "5 = neutral, " +
                "10 = sehr positiv / gute Nachricht. " +
                "Berücksichtige Positivität, Hoffnung, Lösungen und gesellschaftlichen Nutzen.",
            input = $"Quelle: {source}\nTitel: {title}\nBeschreibung: {description}",
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "article_score",
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new
                        {
                            score = new
                            {
                                type = "number",
                                minimum = 1,
                                maximum = 10
                            }
                        },
                        required = new[] { "score" }
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        try
        {
            using var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OpenAI scoring failed: {StatusCode} {Error}", response.StatusCode, error);
                return 1;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("output_text", out var outputText))
            {
                var text = outputText.GetString();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    using var inner = JsonDocument.Parse(text);

                    if (inner.RootElement.TryGetProperty("score", out var scoreProp) &&
                        scoreProp.TryGetDouble(out var score))
                    {
                        return Math.Clamp(Math.Round(score, 2), 1, 10);
                    }
                }
            }

            return 1;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("OpenAI scoring timeout");
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI scoring crashed");
            return 1;
        }
    }
}