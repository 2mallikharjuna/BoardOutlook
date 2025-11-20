using System;
using BoardOutlook.Application.DTOs.Response;

namespace BoardOutlook.Application.Interfaces
{
    /// <summary>
    /// API Service to interact with external APIs
    /// </summary>
    public interface IExecutiveCompensationService
    {
        Task<IEnumerable<CompanyDto>> GetCompaniesAsync(string exchange, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExecutiveDto>> GetExecutivesAsync(string companySymbol, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExecutiveCompensationResultDto>> GetHighPaidExecutivesAsync(string exchange, CancellationToken ct);
        Task<IndustryBenchmarkDto> GetIndustryBenchmarkAsync(string industry, CancellationToken cancellationToken = default);
    }
}
