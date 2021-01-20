using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApiClient
{
    public class BaseClient
    {
        private readonly string _apiKey;
        private string _bearerToken;
        private readonly string _secret;

        private readonly string _baseUrl;


        /// <summary>
        /// Initialize a new instance of the Client class
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="apiKey"></param>
        /// <param name="secret"></param>
        public BaseClient(string baseUrl, string apiKey, string secret)
        {
            _apiKey = apiKey;
            _secret = secret;
            _baseUrl = baseUrl;
        }

        private string BearerToken => _bearerToken ?? (_bearerToken = CreateJwt());

        private string CreateJwt()
        {
            var helper = new TokenHelper();

            return helper.GenerateToken(_apiKey, _secret);
        }

        public async Task<T> GetAsync<T>(string url) where T : class, new()
        {
            T response = default(T);
            using (var client = new WebClient(_baseUrl, BearerToken))
            {
                response = await client.GetAsync<T>(url);
            }

            return response;
        }

        public async Task<T> PostAsync<T, TK>(string url, TK payload)
            where T : class, new()
            where TK : class, new()
        {
            T response = default(T);
            using (var client = new WebClient(_baseUrl, BearerToken))
            {
                response = await client.PostAsync<T>(url, payload);
            }

            return response;
        }

        public async Task<HttpStatusCode> PatchAsync<T>(string url, T payload)
            where T : class, new()
        {
            HttpStatusCode response = HttpStatusCode.BadRequest;
            using (var client = new WebClient(_baseUrl, BearerToken))
            {
                response = await client.PatchAsync(url, payload);
            }

            return response;
        }

        public async Task<HttpStatusCode> DeleteAsync(string url)
        {
            HttpStatusCode response;
            using (var client = new WebClient(_baseUrl, BearerToken))
            {
                response = await client.DeleteAsync(url);
            }

            return response;
        }
    }
}
