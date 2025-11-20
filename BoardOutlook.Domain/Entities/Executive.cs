using System;

namespace BoardOutlook.Domain.Entities
{
    // Executive domain model
    public class Executive
    {
        public string Cik { get; private set; }
        public CompanySymbol Symbol { get; private set; }
        public string CompanyName { get; private set; }
        public Industry Industry { get; private set; }
        public DateTime AcceptedDate { get; private set; }
        public DateTime FilingDate { get; private set; }
        public string NameAndPosition { get; private set; }
        public int Year { get; private set; }
        public decimal Salary { get; private set; }
        public decimal Bonus { get; private set; }
        public decimal? StockAward { get; private set; }
        public decimal? IncentivePlanCompensation { get; private set; }
        public decimal? AllOtherCompensation { get; private set; }
        public decimal Total { get; private set; }
        public string Url { get; private set; }

        public Executive(
            string cik,
            CompanySymbol symbol,
            string companyName,
            Industry industry,
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
            Industry = industry ?? throw new ArgumentNullException(nameof(industry));
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

        
    }
}
