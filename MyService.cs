using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IHttpClientFactory_ConsoleDemo
{
    // Class for a typed instance
    public class MyService : IMyService
    {
        private readonly IHttpClientFactory _clientFactory;

        public MyService(IHttpClientFactory clientFactory)
        {
            // Use injected instance of httpclientfactory
            _clientFactory = clientFactory;
        }

        public async Task<string> GetPageInformation()
        {
            var client = _clientFactory.CreateClient();

            // Get some HTML content from google.com
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://google.com");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine("Not found");
                }
                return $"StatusCode: {response.StatusCode}";
            }
        }
    }
}