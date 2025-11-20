using BoardOutlook.Api.Middleware;
using BoardOutlook.Application.Features.Benchmarks;
using BoardOutlook.Application.Features.Common;
using BoardOutlook.Application.Features.Companies;
using BoardOutlook.Application.Features.Executives;
using BoardOutlook.Application.Interfaces;
using BoardOutlook.Application.Services;
using BoardOutlook.Domain.Entities;
using BoardOutlook.Infrastructure.Configuration;
using BoardOutlook.Infrastructure.Repositories;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;

namespace BoardOutlook.Api.App_start
{
    /// <summary>
    /// Dependency injector of all services
    /// </summary>
    public static class DependencyInjector
    {
        /// <summary>
        /// Extension method to add the dependencies
        /// </summary>
        /// <param name="services">Base services</param>
        /// <param name="configuration">Base configuration</param>
        /// <returns></returns>
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<IConfiguration>(configuration);
            services.AddSingleton(configuration);
            services.AddExceptionHandler<ApplicationExceptionHandler>();
            services.InjectDependencies(configuration);
            services.AddHealthChecks();
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
            }));

            return services;
        }

        /// <summary>
        /// Create a dependency injection
        /// </summary>
        /// <param name="services">Base services</param>
        /// <param name="configuration"></param>
        public static void InjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IExecutiveCompensationService, ExecutiveCompensationService>();

            services.AddScoped<IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>>, GetAsxCompanyQueryHandler>();
            services.AddScoped<IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>>, GetExecutivesQueryHandler>();
            services.AddScoped<IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark>, GetIndustryBenchmarkQueryHandler>();

            services.AddScoped<IMarketDataRepository, MarketDataRepository>();

        }

        /// <summary>
        /// Applying the api middlewares
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder AppApiMiddlewares(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestLoggingMiddleware>();
            
            return builder;
        }

        ///configure the API settings
        public static IServiceCollection ConfigureApiSettings(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure External API Settings
            services.Configure<ExternalApiSettings>(
                configuration.GetSection(ExternalApiSettings.SectionName));
            return services;
        }

        ///configure the API settings
        public static IServiceCollection ConfigurePollySettings(this IServiceCollection services)
        {
            var policyRegistry = new PolicyRegistry();

            // Example Retry Policy
            policyRegistry.Add("RetryPolicy", HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            // Example Circuit Breaker Policy
            policyRegistry.Add("CircuitBreakerPolicy", HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddSingleton<IPolicyRegistry<string>>(policyRegistry);
            return services;
        }


    }
}

