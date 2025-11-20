using System;
using System.Text.Json.Serialization;

namespace BoardOutlook.Infrastructure.ExternalApis.Dtos
{
    internal class CompanyApiResponse
    {
        public string Sector { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FullTimeEmployees { get; set; } = ""; 
        public string Symbol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Exchange { get; set; } = string.Empty;
        public string ExchangeShortName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "stock"
    }
}
