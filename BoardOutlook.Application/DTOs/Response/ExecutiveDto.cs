using BoardOutlook.Domain.Entities;

namespace BoardOutlook.Application.DTOs.Response
{
    // Executive DTO
    public class ExecutiveDto
    {
        public string CompanySymbol { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public decimal Bonus { get; set; }
        public decimal? StockAward { get; set; }
        public decimal? IncentivePlanCompensation { get; set; }
        public decimal? AllOtherCompensation { get; set; }
        public decimal TotalCompensation { get; set; }

        

        // Executive → ExecutiveDto
        public static ExecutiveDto ToDto(Executive executive) =>
            new()
            {
                CompanySymbol = executive.Symbol.Value,
                Name = executive.NameAndPosition,
                Position = executive.NameAndPosition, // Split or parse name/position if needed
                Salary = executive.Salary ?? 0,
                Bonus = executive.Bonus ?? 0,
                StockAward = executive.StockAward,
                IncentivePlanCompensation = executive.IncentivePlanCompensation,
                AllOtherCompensation = executive.AllOtherCompensation,
                TotalCompensation = executive.Total
            };
    }
}
