using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class PageOpener
{
    private readonly IWebDriver driver;
    private readonly int maxConcurrentTabs;
    private readonly int pageWaitTime;

    public PageOpener(IWebDriver driver, int maxConcurrentTabs, int pageWaitTime)
    {
        this.driver = driver;
        this.maxConcurrentTabs = maxConcurrentTabs;
        this.pageWaitTime = pageWaitTime;
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
                        Console.WriteLine($"Открываем: {link}");
                        driver.Navigate().GoToUrl(link);
                        await Task.Delay(pageWaitTime);
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