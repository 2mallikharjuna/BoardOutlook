using BoardOutlook.Domain.Entities;
using Polly.Caching;
using System;

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

        /// <summary>
        /// Determines if the executive's total compensation exceeds a given threshold percentage over the industry average.
        /// </summary>
        /// <param name="averageCompensation">Industry average compensation.</param>
        /// <param name="thresholdPercentage">Threshold percentage (e.g., 10 for 10%).</param>
        /// <returns>True if total compensation is above the threshold, otherwise false.</returns>
        public bool IsCompensationAboveThreshold(decimal averageCompensation, decimal thresholdPercentage)
        {
            if (averageCompensation <= 0 || thresholdPercentage < 0) return false;

            var threshold = averageCompensation * (1 + thresholdPercentage / 100);
            return TotalCompensation >= threshold;
        }

        // Executive → ExecutiveDto
        public static ExecutiveDto ToDto(Executive executive) =>
            new()
            {
                CompanySymbol = executive.Symbol.Value,
                Name = executive.NameAndPosition,
                Position = executive.NameAndPosition, // Split or parse name/position if needed
                Salary = executive.Salary,
                Bonus = executive.Bonus,
                StockAward = executive.StockAward,
                IncentivePlanCompensation = executive.IncentivePlanCompensation,
                AllOtherCompensation = executive.AllOtherCompensation,
                TotalCompensation = executive.Total
            };
    }
}
