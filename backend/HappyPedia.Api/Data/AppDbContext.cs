using Microsoft.EntityFrameworkCore;
using HappyPedia.Api.Models;

namespace HappyPedia.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Article> Articles => Set<Article>();
}