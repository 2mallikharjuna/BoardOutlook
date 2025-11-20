using System;

namespace BoardOutlook.Domain.Entities
{
    public class IndustryBenchmark
    {
        // Industry name / title
        public Industry Industry { get; private set; }

        // Fiscal year of the benchmark
        public int Year { get; private set; }

        // Average compensation in this industry
        public decimal AverageCompensation { get; private set; }

        // Constructor to initialize all required properties
        public IndustryBenchmark(Industry industry, int year, decimal averageCompensation)
        {
            Industry = industry ?? throw new ArgumentNullException(nameof(industry));
            Year = year;
            AverageCompensation = averageCompensation;
        }
    }
}
