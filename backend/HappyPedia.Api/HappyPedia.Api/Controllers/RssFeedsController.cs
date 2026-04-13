using HappyPedia.Api.Data;
using HappyPedia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyPedia.Api.Controllers;

[ApiController]
[Route("api/rssfeeds")]
public class RssFeedsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RssFeedsController(AppDbContext db)
    {
        _db = db;
    }

    // RSS-Feeds anzeigen
    // GET /api/rssfeeds
    [HttpGet]
    public async Task<IActionResult> GetFeeds()
    {
        var feeds = await _db.RssFeeds
            .OrderBy(f => f.Name)
            .ToListAsync();

        return Ok(feeds);
    }

    // Einzelnen RSS-Feed hochladen
    // POST /api/rssfeeds
    [HttpPost]
    public async Task<IActionResult> AddFeed([FromBody] RssFeed feed)
    {
        if (feed == null)
            return BadRequest(new { message = "Feed fehlt" });

        if (string.IsNullOrWhiteSpace(feed.Name) || string.IsNullOrWhiteSpace(feed.Url))
            return BadRequest(new { message = "Name und Url sind erforderlich" });

        var exists = await _db.RssFeeds
            .AnyAsync(f => f.Url.ToLower() == feed.Url.ToLower());

        if (exists)
            return BadRequest(new { message = "Feed existiert bereits" });

        _db.RssFeeds.Add(feed);
        await _db.SaveChangesAsync();

        return Ok(feed);
    }

    // Mehrere RSS-Feeds auf einmal hochladen
    // POST /api/rssfeeds/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> Bulk([FromBody] BulkRssFeedRequest request)
    {
        if (request == null || request.Feeds == null || request.Feeds.Count == 0)
            return BadRequest(new { message = "Keine Feeds übergeben" });

        var existingUrls = await _db.RssFeeds
            .Select(f => f.Url.ToLower())
            .ToListAsync();

        var feedsToAdd = request.Feeds
            .Where(f => !string.IsNullOrWhiteSpace(f.Name) && !string.IsNullOrWhiteSpace(f.Url))
            .Where(f => !existingUrls.Contains(f.Url.ToLower()))
            .ToList();

        if (feedsToAdd.Count == 0)
            return Ok(new
            {
                message = "Keine neuen Feeds gespeichert",
                count = 0
            });

        _db.RssFeeds.AddRange(feedsToAdd);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Feeds gespeichert",
            count = feedsToAdd.Count
        });
    }
}