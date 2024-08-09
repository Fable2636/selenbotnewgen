using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PageOpener
{
    private readonly IWebDriver driver;
    private readonly int maxConcurrentTabs;
    private readonly int pageWaitTime;
    private readonly ILogger<PageOpener> _logger;

    public PageOpener(IWebDriver driver, int maxConcurrentTabs, int pageWaitTime, ILogger<PageOpener> logger)
    {
        this.driver = driver;
        this.maxConcurrentTabs = maxConcurrentTabs;
        this.pageWaitTime = pageWaitTime;
        _logger = logger;
    }

    public async Task OpenPagesAsync(List<string> links)
    {
        using (SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrentTabs))
        {
            List<Task> tasks = new List<Task>();

            foreach (var link in links)
            {
                await semaphore.WaitAsync();

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Открываем: {Link}",link);
                        driver.Navigate().GoToUrl(link);
                        await Task.Delay(pageWaitTime);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при открытии страницы: {Link}", link);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}