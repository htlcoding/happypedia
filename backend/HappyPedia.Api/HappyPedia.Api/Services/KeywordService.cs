using System.Text.Json;

namespace HappyPedia.Api.Services;

public class KeywordService
{
    private readonly Dictionary<string, double> _keywords;

    public KeywordService()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Config", "keywords.json");

        if (!File.Exists(path))
        {
            _keywords = new Dictionary<string, double>();
            return;
        }

        var json = File.ReadAllText(path);
        _keywords = JsonSerializer.Deserialize<Dictionary<string, double>>(json)
                    ?? new Dictionary<string, double>();
    }

    public double CalculateScore(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var normalized = text.ToLowerInvariant();
        double score = 0;

        foreach (var keyword in _keywords)
        {
            if (normalized.Contains(keyword.Key.ToLowerInvariant()))
            {
                score += keyword.Value;
            }
        }

        return Math.Min(Math.Round(score, 2), 10);
    }
}