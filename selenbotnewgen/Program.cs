using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Используем Serilog как логгер

// Добавление необходимых служб
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", async (ILogger<WebScraper> logger, ILogger <PageOpener> pageOpenerLogger, IConfiguration configuration) =>
{
    var scraper = new WebScraper(logger, builder.Configuration);
    await scraper.StartScrapingAsync(pageOpenerLogger);
});

app.Run();