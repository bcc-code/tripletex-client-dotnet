using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BccCode.Tripletex.Client
{
    public partial class TripletexClient
    {
        IHttpClientFactory _clientFactory;
        TripletexClientOptions _options;
        string? _sessionToken = null;
        public TripletexClient(TripletexClientOptions options, IHttpClientFactory clientFactory) : this()
        {
            _clientFactory = clientFactory;
            _options = options;
            BaseUrl = options.ApiBasePath ?? BaseUrl;
        }

        public TripletexClient(TripletexClientOptions options) : this()
        {
            _clientFactory = new ClientFactory();
            _options = options;
            BaseUrl = options.ApiBasePath ?? BaseUrl;
        }

        internal class ClientFactory : IHttpClientFactory
        {
            public HttpClient CreateClient(string name) => new HttpClient();
        }


        public async Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellation)
        {
            _sessionToken = await EnsureAccessTokenAsync(cancellation);
            return _clientFactory.CreateClient();
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            var bearerToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"0:{_sessionToken}"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", bearerToken);
        }
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
        }
        partial void ProcessResponse(HttpClient client, HttpResponseMessage response)
        {

        }


        private static ConcurrentDictionary<string, (string? token, DateTime expires)> _tokens = new ConcurrentDictionary<string, (string? token, DateTime expires)>(); 
        private static SemaphoreSlim _tokenLock = new SemaphoreSlim(1);
        protected virtual async Task<string> EnsureAccessTokenAsync(System.Threading.CancellationToken cancellationToken)
        {
            var key = $"token_{_options.EmployeeToken}_{_options.ConsumerToken}";
            var token = default((string? value, DateTime expires));
            if (!_tokens.TryGetValue(key, out token) || token.expires <= DateTime.Today)
            {
                try
                {
                    await _tokenLock.WaitAsync(cancellationToken);
                    if (!_tokens.TryGetValue(key, out token) || token.expires <= DateTime.Today)
                    {
                        token = await GetSessionTokenAsync(cancellationToken).ConfigureAwait(false);
                        _tokens[key] = token;
                    }                    

                } 
                finally
                {
                    _tokenLock.Release();
                }
            }
            return token.value ?? "";
        }

        protected async Task<(string? value, DateTime expires)> GetSessionTokenAsync(CancellationToken cancellation)
        {
            var expires = DateTimeOffset.Now.AddDays(1).Date;
            var token = default(ResponseWrapperSessionToken); 
            var result = await _clientFactory.CreateClient().PutAsync($"{_options.ApiBasePath}/token/session/:create?consumerToken={_options.ConsumerToken}&employeeToken={_options.EmployeeToken}&expirationDate={expires.ToString("yyyy-MM-dd")}", new StringContent(""), cancellation);
            if (result.IsSuccessStatusCode)
            {
                var strResponse = await result.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<ResponseWrapperSessionToken>(strResponse);
            }
            else
            {
                throw new Exception($"Failed to retreive session token. Status code: {result.StatusCode}");
            }
            return (token?.Value?.Token?.ToString(), expires);
        }

       
    }
}
