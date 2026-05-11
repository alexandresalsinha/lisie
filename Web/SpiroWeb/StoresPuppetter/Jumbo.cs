using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace StoresPuppetter
{
    public static class Jumbo
    {
        public static async Task<bool> AddProducts(List<Product> products, string pathToDownload)
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

            var options = new LaunchOptions { Headless = true, ExecutablePath = executablePath };

            using (var browser = await Puppeteer.LaunchAsync(options))
            using (var page = await browser.NewPageAsync())
            {
                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
                await page.SetViewportAsync(new ViewPortOptions { Width = 1280, Height = 800 });
                await page.GoToAsync("https://www.jumbo.pt/Frontoffice/MyAccount/Authentication", new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
                });
                //await page.WaitForNavigationAsync();
                //var test = await page.EvaluateExpressionAsync<string>("() => document.querySelector('#cookies-accept').innerHTML");
                var element = await page.QuerySelectorAsync("#cookies-accept");
                //var attrb = await page.QuerySelectorAsync("#cookies-accept").EvaluateFunctionAsync<string>("(elem) => {return elem.style.display}");
                if (element != null)
                {
                    await element.ClickAsync();
                    //await page.WaitForNavigationAsync(new NavigationOptions
                    //{
                    //    WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded }
                    //});
                    //await page.WaitForNavigationAsync();
                }
                await page.ScreenshotAsync(executablePath + "screenshot.png");
                await page.TypeAsync("[name='username']", "alexandresalsinha@gmail.com");
                await page.TypeAsync("[name='password']", "1234qwrefdsa");
                await page.ClickAsync("#jumbo-login > form.jumbo-form.login-form > div.jumbo-submit.jumbo-button.jumbo-button-flex.login-form");

                //await page.WaitForSelectorAsync(".featured-categories", new WaitForSelectorOptions { Visible = true });
                await page.WaitForSelectorAsync(".featured-categories");


                foreach (Product _product in products)
                {
                    await page.GoToAsync("https://www.jumbo.pt" + _product.Url);
                    await page.WaitForSelectorAsync("#conteudo > div.product-detail > div:nth-child(1) > div.col-md-11.col-sm-11.col-xs-12 > div > div.col-md-5.col-sm-5.col-xs-12.text-info.pull-right > div.product-btns-panel > button.btn.btn-link.large.hidden-xs", new WaitForSelectorOptions { Visible=true });
                    await page.ClickAsync("#conteudo > div.product-detail > div:nth-child(1) > div.col-md-11.col-sm-11.col-xs-12 > div > div.col-md-5.col-sm-5.col-xs-12.text-info.pull-right > div.product-btns-panel > button.btn.btn-link.large.hidden-xs");
                    //await page.WaitForNavigationAsync();

                    var element2 = await page.QuerySelectorAsync("#deliveryMethodComfirmation");
                    if (element2 != null)
                    {
                        await element2.ClickAsync();
                    }
                }

            }
            return true;
        }

    }
}
