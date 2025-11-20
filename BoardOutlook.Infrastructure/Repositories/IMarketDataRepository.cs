using System;
using BoardOutlook.Domain.Entities;

namespace BoardOutlook.Infrastructure.Repositories
{
    public interface IMarketDataRepository
    {
        Task<IEnumerable<Company>> GetCompaniesAsync(string exchange, CancellationToken ct = default);
        Task<IEnumerable<Executive>> GetExecutivesAsync(string companySymbol, CancellationToken ct = default);
        Task<IndustryBenchmark> GetIndustryBenchmarkAsync(string industry, CancellationToken ct = default);
    }
}
