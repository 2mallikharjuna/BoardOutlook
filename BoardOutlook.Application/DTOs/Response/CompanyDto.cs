using System;
using BoardOutlook.Domain.Entities;

namespace BoardOutlook.Application.DTOs.Response
{
    public class CompanyDto
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string Country { get; set; }
        public int FullTimeEmployees { get; set; }
        public decimal? Price { get; set; }
        public string Exchange { get; set; }
        public string ExchangeShortName { get; set; }
        public string Type { get; set; }

        // Company → CompanyDto
        public static CompanyDto ToDto(Company company) =>
            new()
            {
                Symbol = company.Symbol,
                Name = company.Name,
                Sector = company.Sector,
                Country = company.Country,
                FullTimeEmployees = company.FullTimeEmployees,
                Price = company.Price,
                Exchange = company.Exchange,
                ExchangeShortName = company.ExchangeShortName,
                Type = company.Type.ToString()
            };
    }
}
