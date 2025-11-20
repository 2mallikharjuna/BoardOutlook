using System;

namespace BoardOutlook.Infrastructure.ExternalApis.Dtos
{
    public class BenchmarkApiResponse
    {
        public string IndustryTitle { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal AverageCompensation { get; set; }
    }
}
