using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IHttpClientFactory_ConsoleDemo.Handlers
{
    public class RetryDelegatingHandler : DelegatingHandler
    {
        private readonly int _maxRetries = 3;

        public RetryDelegatingHandler(int maximumAmountOfRetries)
           : base()
        {
            _maxRetries = maximumAmountOfRetries;
        }
        public RetryDelegatingHandler(HttpMessageHandler innerHandler,
          int maximumAmountOfRetries)
            : base(innerHandler)
        {
            _maxRetries = maximumAmountOfRetries;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < _maxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            }
            return response;
        }
    }
}
