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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
            .HasIndex(a => a.Url)
            .IsUnique();
    }
}