using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BccCode.Tripletex.Client
{
    public class TripletexRateLimitingHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static int _remainingRequests = 100;
        private static int _requestLimit = 100;
        private static DateTimeOffset _resetTime = DateTimeOffset.MinValue;

        private const int MAX_RETRIES = 10;
        private const int RETRY_DELAY_MILLISECONDS = 500;

        private const int QUOTA_DELAY_MILLISECONDS = 50;

        public TripletexRateLimitingHandler(HttpMessageHandler innerHandler = null!)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await WaitForQuotaAsync(cancellationToken);

            HttpResponseMessage response = default!;
            for (int retry = 0; retry < MAX_RETRIES; retry++)
            {
                response = await base.SendAsync(request, cancellationToken);
                await UpdateRateLimitStateAsync(response);

                if (response.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                {
                    return response;
                }

                var retryAfter = RETRY_DELAY_MILLISECONDS;
                if (response.Headers.TryGetValues("Retry-After", out var values) && int.TryParse(values.FirstOrDefault(), out var retryAfterValue))
                {
                    retryAfter = retryAfterValue * 1000;
                }

                Debug.WriteLine("Too many requests. Retrying after {0} milliseconds", retryAfter);
                await System.Threading.Tasks.Task.Delay(retryAfter * retry, cancellationToken);
            }

            return response; // Return the last response after exhausting retries
        }

        private static async System.Threading.Tasks.Task WaitForQuotaAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await _semaphore.WaitAsync(cancellationToken);
                try
                {
                    if (DateTimeOffset.Now >= _resetTime)
                    {
                        _remainingRequests = _requestLimit; // Reset to the new limit from headers
                    }

                    if (_remainingRequests > 0)
                    {
                        _remainingRequests--;
                        return;
                    }
                }
                finally
                {
                    _semaphore.Release();
                }

                var now = DateTimeOffset.Now;
                if (_resetTime > now)
                {
                    var timeUntilReset = _resetTime - now;

                    Debug.WriteLine("Quota exceeded, waiting until {0}", _resetTime);
                    await System.Threading.Tasks.Task.Delay(timeUntilReset, cancellationToken);
                }
            }
        }

        private static async System.Threading.Tasks.Task UpdateRateLimitStateAsync(HttpResponseMessage response)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (response.Headers.TryGetValues("X-Rate-Limit-Limit", out var limitValues) &&
                    int.TryParse(limitValues.FirstOrDefault(), out var limit))
                {
                    _requestLimit = limit;
                }

                if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var remainingValues) &&
                    int.TryParse(remainingValues.FirstOrDefault(), out var remaining))
                {
                    _remainingRequests = remaining;
                }

                if (response.Headers.TryGetValues("X-Rate-Limit-Reset", out var resetValues) &&
                    int.TryParse(resetValues.FirstOrDefault(), out var resetSeconds))
                {
                    _resetTime = DateTimeOffset.Now.AddSeconds(resetSeconds);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}