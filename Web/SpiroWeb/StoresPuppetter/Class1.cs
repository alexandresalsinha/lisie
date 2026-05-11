using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace StoresPuppetter
{
    public static class Proccess
    {
        public static async Task<string> GetGoogleHrefs(string pathToDownload)
        {
           
            string response = string.Empty;
            response += "Downloading chromium\n";
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
               Path = pathToDownload
            });

            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
            response += "Navigating to google.com\n";


            var executablePath = browserFetcher.GetExecutablePath(BrowserFetcher.DefaultRevision);

            var options = new LaunchOptions { Headless = false, ExecutablePath = executablePath };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync("http://www.google.com");
                var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
                var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
                foreach (string url in urls)
                {
                    response += $"Url: {url}";
                }
                return response;
            }
        }
    }
}
