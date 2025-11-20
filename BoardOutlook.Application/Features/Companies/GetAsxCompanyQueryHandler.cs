using System;
using BoardOutlook.Domain.Entities;
using BoardOutlook.Infrastructure.Repositories;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Companies
{
    public class GetAsxCompanyQueryHandler : IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>>
    {
        readonly IMarketDataRepository _repo;

        public GetAsxCompanyQueryHandler(IMarketDataRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Company>> ExecuteAsync(GetAsxCompanyQuery query, CancellationToken ct)
        {
            return await _repo.GetCompaniesAsync(query.Company, ct);
        }
    }

}
