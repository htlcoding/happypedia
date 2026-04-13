using HappyPedia.Api.Data;
using HappyPedia.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;

namespace HappyPedia.Api.Services;

public class RssImportService
{
    private readonly AppDbContext _db;
    private readonly KeywordService _keywordService;
    private readonly LlmScoringService _llmScoringService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RssImportService> _logger;

    public RssImportService(
        AppDbContext db,
        KeywordService keywordService,
        LlmScoringService llmScoringService,
        IConfiguration configuration,
        ILogger<RssImportService> logger)
    {
        _db = db;
        _keywordService = keywordService;
        _llmScoringService = llmScoringService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> ImportAllFeeds()
    {
        var maxItemsPerFeed = _configuration.GetValue<int?>("ImportSettings:MaxItemsPerFeed") ?? 2;

        var feeds = await _db.RssFeeds
            .Where(f => f.IsActive && !string.IsNullOrWhiteSpace(f.Url))
            .ToListAsync();

        var existingUrls = (await _db.Articles
            .Select(a => a.Url)
            .ToListAsync())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var imported = 0;

        foreach (var feed in feeds)
        {
            try
            {
                using var reader = XmlReader.Create(feed.Url, new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore
                });

                var rss = SyndicationFeed.Load(reader);
                if (rss == null)
                    continue;

                foreach (var item in rss.Items.Take(maxItemsPerFeed))
                {
                    try
                    {
                        var url = item.Links.FirstOrDefault()?.Uri?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(url))
                            continue;

                        if (existingUrls.Contains(url))
                            continue;

                        var title = item.Title?.Text?.Trim() ?? "Ohne Titel";
                        var description = item.Summary?.Text?.Trim() ?? "";
                        var publishedAt = item.PublishDate != DateTimeOffset.MinValue
                            ? item.PublishDate.UtcDateTime
                            : DateTime.UtcNow;

                        var rawKeywordScore = _keywordService.CalculateScore($"{title} {description}");
                        var keywordPoints = Math.Clamp(Math.Round(rawKeywordScore, 2), 0, 10);

                        double aiPoints;
                        try
                        {
                            aiPoints = await _llmScoringService.CalculateScoreAsync(title, description, feed.Name);
                        }
                        catch
                        {
                            aiPoints = 1;
                        }

                        aiPoints = Math.Clamp(Math.Round(aiPoints, 2), 1, 10);

                        var finalScore = aiPoints + keywordPoints;

                        var article = new Article
                        {
                            Title = title,
                            Url = url,
                            Source = feed.Name,
                            Description = description,
                            PublishedAt = publishedAt,
                            KeywordScore = keywordPoints,
                            AiScore = aiPoints,
                            Upvotes = 0,
                            Downvotes = 0,
                            Score = finalScore,
                            ImageUrl = null
                        };

                        _db.Articles.Add(article);
                        existingUrls.Add(url);
                        imported++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Artikel aus Feed {FeedName} konnte nicht importiert werden", feed.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Feed fehlgeschlagen: {FeedName} ({FeedUrl})", feed.Name, feed.Url);
                feed.IsActive = false;
            }
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "SaveChanges hatte einen Duplicate-Konflikt");
        }

        return imported;
    }
}