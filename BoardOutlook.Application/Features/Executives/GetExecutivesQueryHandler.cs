using System;
using BoardOutlook.Domain.Entities;
using BoardOutlook.Infrastructure.Repositories;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Executives
{
    public class GetExecutivesQueryHandler
    : IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>>
    {
        private readonly IMarketDataRepository _repo;

        public GetExecutivesQueryHandler(IMarketDataRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Executive>> ExecuteAsync(GetExecutivesQuery request, CancellationToken ct)
        {
            return _repo.GetExecutivesAsync(request.CompanySymbol, ct);
        }
    }
}
