using HappyPedia.Api.Data;
using HappyPedia.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HappyPedia.Api.Controllers;

[ApiController]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;

    public CommentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("api/articles/{articleId}/comments")]
    public async Task<IActionResult> GetComments(int articleId)
    {
        var comments = await _db.Comments
            .Where(c => c.ArticleId == articleId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.ArticleId,
                c.UserId,
                Username = c.User != null ? c.User.Username : "User",
                c.Text,
                c.CreatedAt
            })
            .ToListAsync();

        return Ok(comments);
    }

    [HttpPost("api/articles/{articleId}/comments")]
    public async Task<IActionResult> AddComment(
        int articleId,
        [FromHeader(Name = "X-User-Id")] int userId,
        AddCommentRequest request)
    {
        var articleExists = await _db.Articles.AnyAsync(a => a.Id == articleId);

        if (!articleExists)
            return NotFound(new { message = "Artikel nicht gefunden" });

        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);

        if (!userExists)
            return Unauthorized(new { message = "Bitte anmelden" });

        if (string.IsNullOrWhiteSpace(request.Text))
            return BadRequest(new { message = "Kommentar darf nicht leer sein" });

        var comment = new Comment
        {
            ArticleId = articleId,
            UserId = userId,
            Text = request.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);

        return Ok(new
        {
            comment.Id,
            comment.ArticleId,
            comment.UserId,
            Username = user?.Username ?? "User",
            comment.Text,
            comment.CreatedAt
        });
    }

    [HttpDelete("api/comments/{id}")]
    public async Task<IActionResult> DeleteComment(
        int id,
        [FromHeader(Name = "X-User-Id")] int userId)
    {
        var comment = await _db.Comments.FindAsync(id);

        if (comment == null)
            return NotFound(new { message = "Kommentar nicht gefunden" });

        if (comment.UserId != userId)
            return Unauthorized(new { message = "Du darfst diesen Kommentar nicht löschen" });

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Kommentar gelöscht", id });
    }
}

public record AddCommentRequest(string Text);