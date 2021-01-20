using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApiClient
{
    public class RetryHandler : DelegatingHandler
    {
        private const int MaxRetries = 4;

        public RetryHandler(HttpMessageHandler innerHandler) : base(innerHandler)
        { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = null;
            for (int i = 0; i < MaxRetries; i++)
            {
                response = await base.SendAsync(request, cancellationToken);
                await Task.Delay(200, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return response;
                }

                if ((int)response.StatusCode >= 400 && i + 1 == MaxRetries)
                {
                    throw new ApiException(await response.Content.ReadAsStringAsync())
                        { StatusCode = (int)response.StatusCode };
                }

                if ((int)response.StatusCode == 429)
                {
                    await Task.Delay(700, cancellationToken);
                }
            }

            return response;
        }
    }
}
