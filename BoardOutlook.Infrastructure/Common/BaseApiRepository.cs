using BoardOutlook.Infrastructure.Core;
using Polly;
using Polly.Registry;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoardOutlook.Infrastructure.Common
{
    public class BaseApiRepository<T, TApiResponse> : IApiRepository<T, TApiResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IAsyncPolicy<HttpResponseMessage> _circuitBreakerPolicy;

        private readonly JsonSerializerOptions _jsonOptions;

        public BaseApiRepository(HttpClient httpClient, IPolicyRegistry<string> policyRegistry)
        {
            _httpClient = httpClient;
            _retryPolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("RetryPolicy");
            _circuitBreakerPolicy = policyRegistry.Get<IAsyncPolicy<HttpResponseMessage>>("CircuitBreakerPolicy");

            // Configure JSON deserialization options
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,        // Matches "sector" with "Sector", "name" with "Name"
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,  // Handles null values gracefully
                NumberHandling = JsonNumberHandling.AllowReadingFromString     // Handles numbers as strings
            };
        }

        private IAsyncPolicy<HttpResponseMessage> WrapPolicies() => Policy.WrapAsync(_retryPolicy, _circuitBreakerPolicy);

        
        public async Task<TApiResponse> GetAsync<TApiResponse>(string url, CancellationToken ct = default)
        {
            // Execute the HTTP call under the policy (returns HttpResponseMessage)
            var response = await WrapPolicies().ExecuteAsync(() =>
                _httpClient.GetAsync(url, ct)
            );

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(ct);

            // Deserialize after the policy execution
            return JsonSerializer.Deserialize<TApiResponse>(content, _jsonOptions)!;
        }

        public async Task PutAsync(T model, CancellationToken ct = default)
        {
            var content = CreateJson(model);

            // Execute HTTP call under the policy
            var response = await WrapPolicies().ExecuteAsync(() =>
                _httpClient.PutAsync("", content, ct)
            );

            response.EnsureSuccessStatusCode();
        }

        public async Task<T> PostAsync(T model, CancellationToken ct = default)
        {
            var content = CreateJson(model);

            // Execute HTTP call under the policy (returns HttpResponseMessage)
            var response = await WrapPolicies().ExecuteAsync(() =>
                _httpClient.PostAsync("", content, ct)
            );

            response.EnsureSuccessStatusCode();

            var resp = await response.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<T>(resp)!;
        }

        private static StringContent CreateJson(object value) =>
            new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        
    }

}
