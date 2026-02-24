namespace HappyPedia.Api.Models;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public string Source { get; set; } = "";
    public DateTime PublishedAt { get; set; }
    public int Score { get; set; }
}