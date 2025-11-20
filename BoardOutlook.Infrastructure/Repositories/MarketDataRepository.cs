using BoardOutlook.Domain.Entities;
using BoardOutlook.Infrastructure.Common;
using BoardOutlook.Infrastructure.Configuration;
using BoardOutlook.Infrastructure.ExternalApis.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Registry;


namespace BoardOutlook.Infrastructure.Repositories
{
    public class MarketDataRepository : BaseApiRepository<string, object>, IMarketDataRepository
    {        
        private readonly ExternalApiSettings _settings;
        private readonly ILogger<MarketDataRepository> _logger;

        public MarketDataRepository(
            HttpClient httpClient,
            IOptions<ExternalApiSettings> settings,
            ILogger<MarketDataRepository> logger,
            IPolicyRegistry<string> policyRegistry)
            : base(httpClient, policyRegistry)
        {            
            _settings = settings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Fetches all companies for a given exchange from the external API.
        /// Converts API response to domain model only.
        /// </summary>
        /// <param name="exchange">Stock exchange code (e.g., ASX)</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Enumerable of Company domain models</returns>
        public async Task<IEnumerable<Company>> GetCompaniesAsync(string exchange, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(exchange))
            {
                _logger.LogWarning("Exchange code is empty. No companies fetched.");
                return Enumerable.Empty<Company>();
            }

            var url = $"{_settings.BaseUrl}/exchanges/{exchange}/companies?code={_settings.CompaniesAuthCode}";
            _logger.LogDebug("Fetching companies for exchange {Exchange} from {Url}", exchange, url);

            // Call the API and deserialize response into API response objects
            var response = await GetAsync<List<CompanyApiResponse>>(url, ct);

            if (response == null || !response.Any())
            {
                _logger.LogInformation("No companies returned from API for exchange {Exchange}", exchange);
                return Enumerable.Empty<Company>();
            }

            // Map API response to domain model
            return response.Select(c => new Company(
                symbol: c.Symbol,
                name: c.Name,
                sector: c.Sector,
                country: c.Country,
                int.TryParse(c.FullTimeEmployees, out var fullTimeEmployees) ? fullTimeEmployees : 0,
                price: c.Price,
                exchange: c.Exchange,
                exchangeShortName: c.ExchangeShortName,
                type: Company.ParseSecurityType(c.Type)
            ));
        }

        /// <summary>
        /// Fetches all executives for a given company symbol.
        /// Converts API response to domain model only.
        /// </summary>
        /// <param name="companySymbol">Company stock symbol</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Enumerable of Executive domain models</returns>
        public async Task<IEnumerable<Executive>> GetExecutivesAsync(string companySymbol, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(companySymbol))
            {
                _logger.LogWarning("Company symbol is empty. No executives fetched.");
                return Enumerable.Empty<Executive>();
            }

            var url = $"{_settings.BaseUrl}/companies/{companySymbol}/executives?code={_settings.AuthCode}";
            _logger.LogDebug("Fetching executives for company {Symbol} from {Url}", companySymbol, url);

            var response = await GetAsync<List<ExecutiveApiResponse>>(url, ct);

            if (response == null || !response.Any())
            {
                _logger.LogInformation("No executives returned from API for company {Symbol}", companySymbol);
                return Enumerable.Empty<Executive>();
            }

            // Map API response to domain model
            return response.Select(e => new Executive(
                cik: e.Cik,
                symbol: new CompanySymbol(e.Symbol),
                companyName: e.CompanyName ?? "",
                industry: e.IndustryTitle,
                acceptedDate: DateTime.Parse(e.AcceptedDate),
                filingDate: DateTime.Parse(e.FilingDate),
                nameAndPosition: e.NameAndPosition,
                year: e.Year,
                salary: e.Salary,
                bonus: e.Bonus,
                stockAward: e.StockAward,
                incentivePlanCompensation: e.IncentivePlanCompensation,
                allOtherCompensation: e.AllOtherCompensation,
                total: e.Total,
                url: e.Url
            ));
        }

        /// <summary>
        /// Fetches industry benchmark (average executive compensation) for a given industry.
        /// Converts API response to domain model only.
        /// </summary>
        /// <param name="industry">Industry title</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>IndustryBenchmark domain model</returns>
        public async Task<IndustryBenchmark> GetIndustryBenchmarkAsync(string industry, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(industry))
            {
                _logger.LogWarning("Industry title is empty. Returning default benchmark.");
                return new IndustryBenchmark(new Industry(industry), year: DateTime.UtcNow.Year, averageCompensation: 0m);
            }

            var url = $"{_settings.BaseUrl}/industries/{Uri.EscapeDataString(industry)}/benchmark?code={_settings.AuthCode}";
            _logger.LogDebug("Fetching benchmark for industry {Industry} from {Url}", industry, url);

            var response = await GetAsync<BenchmarkApiResponse>(url, ct);

            return new IndustryBenchmark(
                industry: new Industry(industry),
                year: response?.Year ?? DateTime.UtcNow.Year,
                averageCompensation: response?.AverageCompensation ?? 0m
            );
        }


        // Generic helper using base class GetAsync
        private async Task<TApiResponse> GetAsync<TApiResponse>(string url, CancellationToken ct)
        {
            return await base.GetAsync<TApiResponse>(url, ct);
        }
    }

}
