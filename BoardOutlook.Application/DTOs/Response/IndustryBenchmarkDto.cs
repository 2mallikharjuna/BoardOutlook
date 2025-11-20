using System;
using BoardOutlook.Domain.Entities;

namespace BoardOutlook.Application.DTOs.Response
{   
    // Industry Benchmark DTO
    public class IndustryBenchmarkDto
    {
        public string IndustryTitle { get; set; }
        public int Year { get; set; }
        public decimal AverageCompensation { get; set; }


        public IndustryBenchmarkDto()
        {
        }

        public IndustryBenchmarkDto(string industry, decimal comp)
        {
            IndustryTitle = industry;
            AverageCompensation = comp;
        }        

        // IndustryBenchmark → IndustryBenchmarkDto
        public static IndustryBenchmarkDto ToDto(IndustryBenchmark benchmark) =>
            new()
            {
                IndustryTitle = benchmark.Industry.Title,
                Year = benchmark.Year,
                AverageCompensation = benchmark.AverageCompensation
            };
    }
}
