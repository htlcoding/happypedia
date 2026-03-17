using HappyPedia.Api.Data;
using HappyPedia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyPedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ArticlesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/articles
    [HttpGet]
    public async Task<ActionResult<List<Article>>> GetAll()
    {
        var articles = await _db.Articles
            .OrderByDescending(a => a.Score)
            .ThenByDescending(a => a.PublishedAt)
            .ToListAsync();

        return Ok(articles);
    }

    // GET /api/articles/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Article>> GetById(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null) return NotFound();
        return Ok(article);
    }

    // GET /api/articles/top?count=5
    [HttpGet("top")]
    public async Task<ActionResult<List<Article>>> GetTop([FromQuery] int count = 5)
    {
        var articles = await _db.Articles
            .OrderByDescending(a => a.Score)
            .Take(count)
            .ToListAsync();

        return Ok(articles);
    }

    // POST /api/articles
    [HttpPost]
    public async Task<ActionResult<Article>> Create(Article article)
    {
        _db.Articles.Add(article);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = article.Id }, article);
    }

    // DELETE /api/articles/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var article = await _db.Articles.FindAsync(id);
        if (article == null) return NotFound();

        _db.Articles.Remove(article);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST /api/articles/seed – Testdaten einfügen
    [HttpPost("seed")]
    public async Task<ActionResult> Seed()
    {
        if (await _db.Articles.AnyAsync())
            return BadRequest("Datenbank enthält bereits Artikel. Lösche sie zuerst.");

        var seedArticles = new List<Article>
        {
            new()
            {
                Title = "Wiener Startup entwickelt App gegen Lebensmittelverschwendung",
                Url = "https://example.com/wiener-startup-food",
                Source = "derstandard.at",
                PublishedAt = DateTime.UtcNow.AddDays(-1),
                Score = 92
            },
            new()
            {
                Title = "Neue Studie: Ehrenamt stärkt mentale Gesundheit nachweislich",
                Url = "https://example.com/ehrenamt-studie",
                Source = "zeit.de",
                PublishedAt = DateTime.UtcNow.AddDays(-2),
                Score = 88
            },
            new()
            {
                Title = "Solarenergie-Rekord in Deutschland: 60% des Stroms aus Erneuerbaren",
                Url = "https://example.com/solar-rekord",
                Source = "tagesschau.de",
                PublishedAt = DateTime.UtcNow.AddDays(-1),
                Score = 95
            },
            new()
            {
                Title = "Schweizer Forscher finden neuen Ansatz gegen Antibiotikaresistenz",
                Url = "https://example.com/antibiotika-forschung",
                Source = "nzz.ch",
                PublishedAt = DateTime.UtcNow.AddDays(-3),
                Score = 85
            },
            new()
            {
                Title = "Community Gardens in Graz: Nachbarschaft wächst zusammen",
                Url = "https://example.com/community-gardens-graz",
                Source = "kleinezeitung.at",
                PublishedAt = DateTime.UtcNow.AddHours(-12),
                Score = 78
            },
            new()
            {
                Title = "Berliner Schüler gewinnen internationalen Robotik-Wettbewerb",
                Url = "https://example.com/robotik-berlin",
                Source = "spiegel.de",
                PublishedAt = DateTime.UtcNow.AddDays(-2),
                Score = 82
            },
            new()
            {
                Title = "Neues Bildungsprogramm macht Coding für alle Altersgruppen zugänglich",
                Url = "https://example.com/coding-bildung",
                Source = "golem.de",
                PublishedAt = DateTime.UtcNow.AddDays(-4),
                Score = 74
            },
            new()
            {
                Title = "Österreich investiert Milliarden in Bahnausbau für klimafreundliches Reisen",
                Url = "https://example.com/bahn-ausbau-at",
                Source = "orf.at",
                PublishedAt = DateTime.UtcNow.AddHours(-6),
                Score = 90
            },
        };

        _db.Articles.AddRange(seedArticles);
        await _db.SaveChangesAsync();

        return Ok($"{seedArticles.Count} Testartikel eingefügt.");
    }
}