using BoardOutlook.Domain.Entities;
using BoardOutlook.Infrastructure.Repositories;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Benchmarks
{
    public class GetIndustryBenchmarkQueryHandler
        : IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark>
    {
        private readonly IMarketDataRepository _repo;

        public GetIndustryBenchmarkQueryHandler(IMarketDataRepository repo)
        {
            _repo = repo;
        }

        public Task<IndustryBenchmark> ExecuteAsync(GetIndustryBenchmarkQuery query, CancellationToken ct)
        {
            return _repo.GetIndustryBenchmarkAsync(query.Industry, ct);
        }
       
    }
}
