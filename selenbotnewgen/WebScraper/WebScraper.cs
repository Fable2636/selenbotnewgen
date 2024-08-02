using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenQA.Selenium.Interactions;


public class WebScraper
{
    private readonly string sitemapUrl;
    private readonly int maxConcurrentTabs;
    private readonly int pageWaitTime;
    private readonly int repeatInterval;
    private readonly ChromeOptions chromeOptions;

    public WebScraper(IConfiguration configuration)
    {
        sitemapUrl = configuration["SITEMAP_URL"] ?? "https://cska-tickets.com/sitemap.xml";
        maxConcurrentTabs = int.TryParse(configuration["MAX_CONCURRENT_TABS"], out var tabs) ? tabs : 3;
        pageWaitTime = int.TryParse(configuration["PAGE_WAIT_TIME"], out var waitTime) ? waitTime : 5000;
        repeatInterval = int.TryParse(configuration["REPEAT_INTERVAL"], out var interval) ? interval : 10;

        chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArgument("--disable-gpu");
        chromeOptions.AddArgument("--disable-software-rasterizer");
    }

    public async Task StartScrapingAsync()
    {
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {
            while (true)
            {
                var links = await new SitemapFetcher().FetchLinksAsync(sitemapUrl);
                var pageOpener = new PageOpener(driver, maxConcurrentTabs, pageWaitTime);
                await pageOpener.OpenPagesAsync(links);

                await Task.Delay(TimeSpan.FromMinutes(repeatInterval));
            }
        }
    }
}