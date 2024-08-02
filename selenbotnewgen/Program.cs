using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Xml.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async () =>
{
    var scraper = new WebScraper(builder.Configuration);
    await scraper.StartScrapingAsync();
});

app.Run();