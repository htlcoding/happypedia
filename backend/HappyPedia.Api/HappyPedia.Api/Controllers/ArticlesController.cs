using HappyPedia.Api.Data;
using HappyPedia.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyPedia.Api.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly RssImportService _rssImportService;

    public ArticlesController(AppDbContext db, RssImportService rssImportService)
    {
        _db = db;
        _rssImportService = rssImportService;
    }

    // GET /api/articles
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var articles = await _db.Articles
            .OrderByDescending(a => a.Score)
            .ThenByDescending(a => a.Upvotes)
            .ThenByDescending(a => a.PublishedAt)
            .ToListAsync();

        return Ok(articles);
    }

    // POST /api/articles/fetch
    [HttpPost("fetch")]
    public async Task<IActionResult> Fetch()
    {
        var count = await _rssImportService.ImportAllFeeds();

        return Ok(new
        {
            message = "Artikel erfolgreich gefetcht",
            imported = count
        });
    }

    // DELETE /api/articles/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Artikel gelöscht",
            id
        });
    }

    // POST /api/articles/{id}/like
    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(int id)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        article.Upvotes += 1;
        article.Score = article.AiScore + article.KeywordScore + article.Upvotes - article.Downvotes;

        await _db.SaveChangesAsync();

        return Ok(article);
    }

    // POST /api/articles/{id}/dislike
    [HttpPost("{id}/dislike")]
    public async Task<IActionResult> Dislike(int id)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        article.Downvotes += 1;
        article.Score = article.AiScore + article.KeywordScore + article.Upvotes - article.Downvotes;

        await _db.SaveChangesAsync();

        return Ok(article);
    }
}