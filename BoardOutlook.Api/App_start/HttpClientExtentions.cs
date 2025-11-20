using BoardOutlook.Infrastructure.Configuration;
using BoardOutlook.Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using System.Net;

namespace BoardOutlook.Api.App_start
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// configure API settings        
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigurePollySettings(this IServiceCollection services)
        {
            var policyRegistry = new PolicyRegistry();


            // Fallback Policy — Handle 404 gracefully          
            var fallback404 = Policy<HttpResponseMessage>
                .HandleResult(r => r.StatusCode == HttpStatusCode.NotFound)
                .FallbackAsync(
                    fallbackValue: new HttpResponseMessage(HttpStatusCode.NotFound),
                    onFallbackAsync: async (outcome, context) =>
                    {
                        Console.WriteLine("404 encountered → Ignored and continuing.");
                    });


            // 2️⃣ Retry Policy — Only transient failures         
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()                     // HttpRequestException, 5xx, 408
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests) // 429
                                                                               // DO NOT add 404 here
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (outcome, ts, attempt, ctx) =>
                    {
                        Console.WriteLine($"Retry {attempt} due to {outcome.Result?.StatusCode}");
                    });


            // Circuit Breaker — Only on transient failures            
            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests) // 429
                                                                               // DO NOT include 404 here
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30));

            // Combine Policies            
            var combinedPolicy = fallback404
                .WrapAsync(retryPolicy)
                .WrapAsync(circuitBreakerPolicy);

            policyRegistry.Add("DefaultPolicy", combinedPolicy);
            policyRegistry.Add("Fallback404", fallback404);
            policyRegistry.Add("RetryPolicy", retryPolicy);
            policyRegistry.Add("CircuitBreakerPolicy", circuitBreakerPolicy);

            services.AddSingleton<IPolicyRegistry<string>>(policyRegistry);

            return services;
        }
    }

}
