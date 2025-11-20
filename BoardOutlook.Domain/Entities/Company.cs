using System;

namespace BoardOutlook.Domain.Entities
{    
    public class Company
    {
        public string Symbol { get; private set; }
        public string Name { get; private set; }
        public string Sector { get; private set; }
        public string Country { get; private set; }
        public int FullTimeEmployees { get; private set; }
        public decimal? Price { get; private set; }
        public string Exchange { get; private set; }
        public string ExchangeShortName { get; private set; }
        public SecurityType Type { get; private set; }

        public Company(
            string symbol,
            string name,
            string sector,
            string country,
            int fullTimeEmployees,
            decimal? price,
            string exchange,
            string exchangeShortName,
            SecurityType type)
        {
            Symbol = symbol;
            Name = name;
            Sector = sector;
            Country = country;
            FullTimeEmployees = fullTimeEmployees;
            Price = price;
            Exchange = exchange;
            ExchangeShortName = exchangeShortName;
            Type = type;
        }

        /// <summary>
        /// Factory method to map from API string
        /// </summary>
        public static SecurityType ParseSecurityType(string type) =>
            type?.ToLowerInvariant() switch
            {
                "stock" => SecurityType.Stock,
                "bond" => SecurityType.Bond,
                "etf" => SecurityType.Etf,
                _ => SecurityType.Other
            };
    }  

}
