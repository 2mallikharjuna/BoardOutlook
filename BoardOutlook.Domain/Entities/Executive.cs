using System;

namespace BoardOutlook.Domain.Entities
{
    // Executive domain model
    public class Executive
    {
        // Company & Filing Metadata
        public string Cik { get; set; }                  // SEC Central Index Key
        public CompanySymbol Symbol { get; set; }        // Stock ticker symbol
        public string CompanyName { get; set; }          // Full company name
        public string IndustryTitle { get; set; }        // Industry classification

        // Filing Dates
        public DateTime AcceptedDate { get; set; }       // Filing accepted date/time
        public DateTime FilingDate { get; set; }         // Filing date

        // Executive Info
        public string NameAndPosition { get; set; }      // Combined name + title
        public int Year { get; set; }                    // Reporting year

        // Compensation Components
        public decimal? Salary { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? StockAward { get; set; }
        public decimal? IncentivePlanCompensation { get; set; }
        public decimal? AllOtherCompensation { get; set; }
        public decimal Total { get; set; }               // Total compensation

        // Source Link
        public string Url { get; set; }                  // Link to original SEC filing

        public Executive(
            string cik,
            CompanySymbol symbol,
            string companyName,
            string industry,
            DateTime acceptedDate,
            DateTime filingDate,
            string nameAndPosition,
            int year,
            decimal salary,
            decimal bonus,
            decimal? stockAward,
            decimal? incentivePlanCompensation,
            decimal? allOtherCompensation,
            decimal total,
            string url)
        {
            Cik = cik ?? throw new ArgumentNullException(nameof(cik));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
            CompanyName = companyName ?? throw new ArgumentNullException(nameof(companyName));
            IndustryTitle = industry ?? throw new ArgumentNullException(nameof(industry));
            AcceptedDate = acceptedDate;
            FilingDate = filingDate;
            NameAndPosition = nameAndPosition ?? throw new ArgumentNullException(nameof(nameAndPosition));
            Year = year;
            Salary = salary;
            Bonus = bonus;
            StockAward = stockAward;
            IncentivePlanCompensation = incentivePlanCompensation;
            AllOtherCompensation = allOtherCompensation;
            Total = total;
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

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
            return Total >= threshold;
        }


    }
}
