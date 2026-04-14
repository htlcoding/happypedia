using HappyPedia.Api.Data;
using HappyPedia.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 CORS hinzufügen (WICHTIG für Vue Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddSingleton<KeywordService>();

builder.Services.AddHttpClient<LlmScoringService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddScoped<RssImportService>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 🔥 CORS aktivieren (GANZ wichtig, sonst "Failed to fetch")
app.UseCors("frontend");

// Routing
app.MapControllers();

app.Run();