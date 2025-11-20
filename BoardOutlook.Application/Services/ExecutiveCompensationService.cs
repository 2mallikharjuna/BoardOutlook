using BoardOutlook.Application.DTOs.Response;
using BoardOutlook.Application.Features.Benchmarks;
using BoardOutlook.Application.Features.Common;
using BoardOutlook.Application.Features.Companies;
using BoardOutlook.Application.Features.Executives;
using BoardOutlook.Application.Interfaces;
using BoardOutlook.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace BoardOutlook.Application.Services
{
    public class ExecutiveCompensationService : IExecutiveCompensationService
    {
        private readonly IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>> _companiesHandler;
        private readonly IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>> _executivesHandler;
        private readonly IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark> _benchmarkHandler;
        private readonly ILogger<ExecutiveCompensationService> _logger;

        public ExecutiveCompensationService(
            IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>> companiesHandler,
            IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>> executivesHandler,
            IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark> benchmarkHandler,
            ILogger<ExecutiveCompensationService> logger)
        {
            _companiesHandler = companiesHandler;
            _executivesHandler = executivesHandler;
            _benchmarkHandler = benchmarkHandler;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyDto>> GetCompaniesAsync(string exchange, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Service: Fetching companies for {Exchange}", exchange);

                if (string.IsNullOrWhiteSpace(exchange))
                {
                    _logger.LogWarning("Exchange is empty. No companies fetched.");
                    return Enumerable.Empty<CompanyDto>();
                }

                var companies = await _companiesHandler.ExecuteAsync(
                    new GetAsxCompanyQuery(exchange), ct);

                // Map Domain -> Application DTO
                return companies.Select(e => CompanyDto.ToDto(e));                
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Fetching companies cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching companies for exchange {Exchange}", exchange);
                return Enumerable.Empty<CompanyDto>();
            }
        }

        public async Task<IEnumerable<ExecutiveDto>> GetExecutivesAsync(string companySymbol, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Service: Fetching executives for {Symbol}", companySymbol);

                if (string.IsNullOrWhiteSpace(companySymbol))
                {
                    _logger.LogWarning("Company symbol is empty. No executives fetched.");
                    return Enumerable.Empty<ExecutiveDto>();
                }

                var executives = await _executivesHandler.ExecuteAsync(
                    new GetExecutivesQuery(companySymbol), ct);
                
                // Map Domain -> Application DTO
                return executives.Select(e => ExecutiveDto.ToDto(e));

            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Fetching executives cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching executives for company {Symbol}", companySymbol);
                return Enumerable.Empty<ExecutiveDto>();
            }
        }

        public async Task<IndustryBenchmarkDto> GetIndustryBenchmarkAsync(string industry, CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("Service: Fetching benchmark for {Industry}", industry);

                if (string.IsNullOrWhiteSpace(industry))
                {
                    _logger.LogWarning("Industry is empty. Benchmark unavailable.");
                    return null;
                }

                var benchmark =  await _benchmarkHandler.ExecuteAsync(
                    new GetIndustryBenchmarkQuery(industry), ct);

                if (benchmark == null)
                {
                    _logger.LogWarning("No benchmark found for industry {Industry}", industry);
                    return new IndustryBenchmarkDto(industry, 0m); // default value
                }

                // Map Domain -> Application DTO
                return IndustryBenchmarkDto.ToDto(benchmark);                
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Fetching benchmark cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching benchmark for {Industry}", industry);
                return null;
            }
        }


        /// <summary>
        /// GetHighPaidExecutivesAsync
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
                
        public async Task<IEnumerable<ExecutiveCompensationResultDto>> GetHighPaidExecutivesAsync(string exchange, CancellationToken ct = default)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(exchange))
                {
                    _logger.LogWarning("Exchange cannot be null or empty.");
                    return Enumerable.Empty<ExecutiveCompensationResultDto>();
                }

                // STEP 1 — FETCH COMPANIES FOR THE EXCHANGE
                var companies = await _companiesHandler.ExecuteAsync(new GetAsxCompanyQuery(exchange), ct);
                if (!companies.Any())
                {
                    _logger.LogInformation("No companies found for exchange {Exchange}", exchange);
                    return Enumerable.Empty<ExecutiveCompensationResultDto>();
                }

                // STEP 2 — PROCESS COMPANIES IN PARALLEL with SemaphoreSlim
                var results = await ProcessCompaniesAsync(companies, ct, maxDegreeOfParallelism: 5);

                return results;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetHighPaidExecutivesAsync was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetHighPaidExecutivesAsync.");
                return Enumerable.Empty<ExecutiveCompensationResultDto>();
            }
        }

        public async Task<List<ExecutiveCompensationResultDto>> ProcessCompaniesAsync(
             IEnumerable<Company> companies,
             CancellationToken ct,
             int maxDegreeOfParallelism = 20) // adjust based on system/API limits
        {
            var results = new ConcurrentBag<ExecutiveCompensationResultDto>();
            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            var tasks = companies.Select(async company =>
            {
                await semaphore.WaitAsync(ct); // limit concurrency
                try
                {
                    ct.ThrowIfCancellationRequested();

                    // STEP 2 — FETCH EXECUTIVES FOR EACH COMPANY
                    var executives = await _executivesHandler.ExecuteAsync(new GetExecutivesQuery(company.Symbol), ct);
                    if (!executives.Any())
                        return;
                    
                    foreach (var exec in executives)
                    {
                        _logger.LogDebug("Processing Executive: {Name} of Company: {Company}", exec.NameAndPosition, company.Name);

                        // STEP 3 — FETCH INDUSTRY BENCHMARK
                        var benchmark = await GetIndustryBenchmarkAsync(exec.IndustryTitle, ct);
                        if (benchmark == null || benchmark.AverageCompensation <= 0)
                            return;

                        var averageComp = benchmark.AverageCompensation;
                        // STEP 4 — FILTER EXECUTIVES ABOVE THRESHOLD (10% above industry average)
                        if (exec.IsCompensationAboveThreshold(averageComp, 10m))
                        {
                            results.Add(new ExecutiveCompensationResultDto(
                                nameAndPosition: $"{exec.NameAndPosition}",
                                totalCompensation: exec.Total,
                                industryAverage: averageComp
                            ));
                        }                        
                    }
                    
                }
                finally
                {
                    semaphore.Release(); // release slot
                }
            });

            await Task.WhenAll(tasks);

            return results.ToList();
        }


    }
}
