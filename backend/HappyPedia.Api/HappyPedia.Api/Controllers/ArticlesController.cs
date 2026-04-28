using HappyPedia.Api.Data;
using HappyPedia.Api.Models;
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

    [HttpPost("fetch")]
    public async Task<IActionResult> Fetch()
    {
        var count = await _rssImportService.ImportAllFeeds();
        return Ok(new { message = "Artikel erfolgreich gefetcht", imported = count });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Artikel gelöscht", id });
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> Like(int id, [FromHeader(Name = "X-User-Id")] int userId)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        var existingVote = await _db.ArticleVotes
            .FirstOrDefaultAsync(v => v.ArticleId == id && v.UserId == userId);

        if (existingVote == null)
        {
            _db.ArticleVotes.Add(new ArticleVote
            {
                ArticleId = id,
                UserId = userId,
                IsLike = true
            });

            article.Upvotes += 1;
        }
        else if (existingVote.IsLike)
        {
            _db.ArticleVotes.Remove(existingVote);
            article.Upvotes -= 1;
        }
        else
        {
            existingVote.IsLike = true;
            article.Downvotes -= 1;
            article.Upvotes += 1;
        }

        RecalculateScore(article);
        await _db.SaveChangesAsync();

        return Ok(article);
    }

    [HttpPost("{id}/dislike")]
    public async Task<IActionResult> Dislike(int id, [FromHeader(Name = "X-User-Id")] int userId)
    {
        var article = await _db.Articles.FindAsync(id);

        if (article == null)
            return NotFound(new { message = "Artikel nicht gefunden" });

        var existingVote = await _db.ArticleVotes
            .FirstOrDefaultAsync(v => v.ArticleId == id && v.UserId == userId);

        if (existingVote == null)
        {
            _db.ArticleVotes.Add(new ArticleVote
            {
                ArticleId = id,
                UserId = userId,
                IsLike = false
            });

            article.Downvotes += 1;
        }
        else if (!existingVote.IsLike)
        {
            _db.ArticleVotes.Remove(existingVote);
            article.Downvotes -= 1;
        }
        else
        {
            existingVote.IsLike = false;
            article.Upvotes -= 1;
            article.Downvotes += 1;
        }

        RecalculateScore(article);
        await _db.SaveChangesAsync();

        return Ok(article);
    }

    private static void RecalculateScore(Article article)
    {
        article.Upvotes = Math.Max(0, article.Upvotes);
        article.Downvotes = Math.Max(0, article.Downvotes);
        article.Score = article.AiScore + article.KeywordScore + article.Upvotes - article.Downvotes;
    }
}