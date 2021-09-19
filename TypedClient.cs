using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IHttpClientFactory_ConsoleDemo
{
    // Class for a typed instance
    public class TypedClient : ITypedClient
    {
        private HttpClient _client;

        public TypedClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            _client.Timeout = new TimeSpan(0, 0, 30);
            _client.DefaultRequestHeaders.Clear();
        }

        public HttpClient Client => _client;

        public async Task<TestModel> GetSomething(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(
                   HttpMethod.Get,
                   "todos/1");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _client.SendAsync(request,
               HttpCompletionOption.ResponseHeadersRead,
               cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                response.EnsureSuccessStatusCode();

                // Read from stream and deserialize json
                using (var streamReader = new StreamReader(stream))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var jsonSerializer = new JsonSerializer();
                        return jsonSerializer.Deserialize<TestModel>(jsonTextReader);
                    }
                }
            }
        }
    }

    public class TestModel {
        [JsonProperty("userId")]
        public string UserId {get; set;}

        [JsonProperty("id")]
        public string Id {get; set;}

        [JsonProperty("title")]
        public string Title {get; set;}

        [JsonProperty("completed")]
        public string Completed {get; set;}
    }
}