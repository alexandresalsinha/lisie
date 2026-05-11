using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LisieStores.Extensibility
{
    public class Market
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreUrl { get; set; }
        public string StoreSearchUrl { get; set; }
        public string StoreColor { get; set; }

        public Type ClassType { get; set; }
        public async Task<string> FetchUrl(String url)
        {
            var client = new HttpClient();

            //Fake a Browser Request
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            try
            {
                string response = await client.GetStringAsync(url);

                return response;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> FetchUrlMoreBypasses(String url)
        {
            var client = new HttpClient();

            //Fake a Browser Request
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.117 Safari/537.36");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Referrer Policy", "strict-origin-when-cross-origin");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
            //client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en;q=0.9,en-US;q=0.8");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("Pragma", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko; Google Page Speed Insights) Chrome/27.0.1453 Safari/537.36");
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");
            try
            {
                string response = await client.GetStringAsync(url);

                return response;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<string> FetchUrlMoreBypassesWithCookies(string url, string baseUrl, string cookies)
        {
            try
            {

                using (var httpClient = new HttpClient(new HttpClientHandler { UseCookies = true }))
                {
                    var uri = new Uri(baseUrl);
                    httpClient.BaseAddress = uri;
                    httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/apng,*/*;q=0.8");
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                    //httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en;q=0.9,en-US;q=0.8");
                    httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                    httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko; Google Page Speed Insights) Chrome/27.0.1453 Safari/537.36");
                    httpClient.DefaultRequestHeaders.Add("Cookie", cookies);

                    string response = await httpClient.GetStringAsync(url);
                    return response;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
