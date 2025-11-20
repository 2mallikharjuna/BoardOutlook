using System;

namespace BoardOutlook.Infrastructure.ExternalApis.Dtos
{
    internal class ExecutiveApiResponse
    {
        public string Cik { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string IndustryTitle { get; set; } = string.Empty;
        public string AcceptedDate { get; set; } = string.Empty; // string because API sends it as "yyyy-MM-dd HH:mm:ss"
        public string FilingDate { get; set; } = string.Empty; // string
        public string NameAndPosition { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Salary { get; set; }
        public decimal Bonus { get; set; }
        public decimal? StockAward { get; set; }
        public decimal? IncentivePlanCompensation { get; set; }
        public decimal? AllOtherCompensation { get; set; }
        public decimal Total { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}

