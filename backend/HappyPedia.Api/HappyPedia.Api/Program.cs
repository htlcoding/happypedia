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

// CORS für das Vue-Frontend (Vite dev server)
const string FrontendCorsPolicy = "FrontendCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:4173",
                "http://127.0.0.1:4173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(FrontendCorsPolicy);

app.MapControllers();

app.Run();