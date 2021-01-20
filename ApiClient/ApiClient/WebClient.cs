using Newtonsoft.Json;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient
{
    public class WebClient : IDisposable
    {
        private readonly TimeSpan _timeout;
        private HttpClient _httpClient;
        private HttpClientHandler _httpClientHandler;
        private readonly string _baseUrl;
        private const string ClientUserAgent = "example-api-client-v1";
        private const string MediaTypeJson = "application/json";
        private readonly string _token;

        public WebClient(string baseUrl, string token, TimeSpan? timeout = null)
        {
            _baseUrl = NormalizeBaseUrl(baseUrl);
            _timeout = timeout ?? TimeSpan.FromSeconds(90);
            _token = token;
            CreateHttpClient();
        }

        public async Task<string> PostAsync(string url, object input)
        {
            try
            {
                using (var requestContent = new StringContent(ConvertToJsonString(input), Encoding.UTF8, MediaTypeJson))
                {
                    using (var response = await _httpClient.PostAsync(url, requestContent))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<TResult> PostAsync<TResult>(string url, object input) where TResult : class, new()
        {
            try
            {
                var strResponse = await PostAsync(url, input);

                return JsonConvert.DeserializeObject<TResult>(strResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<HttpStatusCode> PatchAsync(string url, object input)
        {
            try
            {
                return await PatchAsyncInternal(url, input);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<TResult> GetAsync<TResult>(string url) where TResult : class, new()
        {
            try
            {
                var strResponse = await GetAsync(url);
                return JsonConvert.DeserializeObject<TResult>(strResponse);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GetAsync(string url)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> PutAsync(string url, object input)
        {
            try
            {
                return await PutAsync(url,
                    new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, MediaTypeJson));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> PutAsync(string url, HttpContent content)
        {
            try
            {
                using (var response = await _httpClient.PutAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<HttpStatusCode> PatchAsyncInternal(string url, object content)
        {
            try
            {
                using (var response = await _httpClient.PatchAsJsonAsync(url, content))
                {
                    response.EnsureSuccessStatusCode();
                    return response.StatusCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<HttpStatusCode> DeleteAsync(string url)
        {
            try
            {
                using (var response = await _httpClient.DeleteAsync(url))
                {
                    response.EnsureSuccessStatusCode();
                    return response.StatusCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _httpClientHandler?.Dispose();
            _httpClient?.Dispose();
        }

        private void CreateHttpClient()
        {
            _httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            _httpClient = new HttpClient(new RetryHandler(_httpClientHandler), false)
            {
                Timeout = _timeout,
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", _token) }
            };

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(ClientUserAgent);

            if (!string.IsNullOrWhiteSpace(_baseUrl))
            {
                _httpClient.BaseAddress = new Uri(_baseUrl);
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeJson));
        }

        private static string ConvertToJsonString(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            return JsonConvert.SerializeObject(obj);
        }

        private static string NormalizeBaseUrl(string url)
        {
            return url.EndsWith("/") ? url : url + "/";
        }
    }
}