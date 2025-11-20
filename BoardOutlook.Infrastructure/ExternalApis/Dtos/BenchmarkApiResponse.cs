using System;

namespace BoardOutlook.Infrastructure.ExternalApis.Dtos
{
    internal class BenchmarkApiResponse
    {
        public string IndustryTitle { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal AverageCompensation { get; set; }
    }
}
