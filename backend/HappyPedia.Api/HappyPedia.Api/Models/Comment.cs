namespace HappyPedia.Api.Models;

public class Comment
{
    public int Id { get; set; }

    public int ArticleId { get; set; }
    public int UserId { get; set; }

    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Article? Article { get; set; }
    public AppUser? User { get; set; }
}