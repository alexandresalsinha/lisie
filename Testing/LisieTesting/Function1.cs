using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace LisieTesting
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public async static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            HttpClient Client = new HttpClient();
            HttpResponseMessage _response = await Client.GetAsync("https://www.google.com");
            string _response2 = await _response.Content.ReadAsStringAsync();
            log.Info($"C# Timer trigger function executed at: {DateTime.Now} with response {_response2}");
        }
    }
}
