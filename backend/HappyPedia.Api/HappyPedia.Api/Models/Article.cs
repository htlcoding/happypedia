namespace HappyPedia.Api.Models;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public string Source { get; set; } = "";
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime PublishedAt { get; set; }

    public double KeywordScore { get; set; }   // 0-10
    public double AiScore { get; set; }        // 1-10
    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public double Score { get; set; }
}