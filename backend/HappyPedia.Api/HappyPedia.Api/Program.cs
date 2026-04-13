using HappyPedia.Api.Data;
using HappyPedia.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<KeywordService>();

builder.Services.AddHttpClient<LlmScoringService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<RssImportService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();