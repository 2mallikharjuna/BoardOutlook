using System;
using BoardOutlook.Application.Features.Common;

namespace BoardOutlook.Application.Features.Benchmarks
{
    public class GetIndustryBenchmarkQuery : IQuery
    {
        public string Industry { get; }
        public GetIndustryBenchmarkQuery(string industry)
        {
            Industry = industry;
        }
    }
}
