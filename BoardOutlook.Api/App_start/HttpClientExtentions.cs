using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.Options;
using BoardOutlook.Infrastructure.Repositories;
using BoardOutlook.Infrastructure.Configuration;

namespace BoardOutlook.Api.App_start
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<IMarketDataRepository, MarketDataRepository>()
                .ConfigureHttpClient((sp, client) =>
                {
                    var settings = sp.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                    client.BaseAddress = new Uri(settings.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                })
                // Retry policy
                .AddPolicyHandler((sp, request) =>
                {
                    var settings = sp.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                    var polly = settings.HttpOptions;

                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .WaitAndRetryAsync(
                            retryCount: polly.RetryCount,
                            sleepDurationProvider: retryAttempt =>
                                TimeSpan.FromSeconds(Math.Pow(polly.RetryBaseDelaySeconds, retryAttempt))
                        );
                })
                // Circuit breaker policy
                .AddPolicyHandler((sp, request) =>
                {
                    var settings = sp.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                    var polly = settings.HttpOptions;

                    return HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(
                            handledEventsAllowedBeforeBreaking: polly.CircuitBreakerFailures,
                            durationOfBreak: TimeSpan.FromSeconds(polly.CircuitBreakerDurationSeconds)
                        );
                });

            return services;
        }
    }

}
