using HappyPedia.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HappyPedia.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<RssFeed> RssFeeds => Set<RssFeed>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<ArticleVote> ArticleVotes => Set<ArticleVote>();
    public DbSet<Comment> Comments => Set<Comment>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
    .HasOne(c => c.Article)
    .WithMany()
    .HasForeignKey(c => c.ArticleId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Article>()
            .HasIndex(a => a.Url)
            .IsUnique();

        modelBuilder.Entity<ArticleVote>()
            .HasIndex(v => new { v.ArticleId, v.UserId })
            .IsUnique();

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Article)
            .WithMany()
            .HasForeignKey(c => c.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}