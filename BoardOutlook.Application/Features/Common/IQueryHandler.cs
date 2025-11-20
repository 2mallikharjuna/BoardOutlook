using System;

namespace BoardOutlook.Application.Features.Common
{
    public interface IQueryHandler<in TQuery, TReturn> where TQuery : IQuery
    {
        Task<TReturn> ExecuteAsync(TQuery query, CancellationToken ct);
    }
}
