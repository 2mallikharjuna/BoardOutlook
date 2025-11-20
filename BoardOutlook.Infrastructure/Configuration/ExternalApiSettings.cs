using System;


namespace BoardOutlook.Infrastructure.Configuration
{
    public class ExternalApiSettings
    {
        public const string SectionName = "ExternalApi";

        public string BaseUrl { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public string CompaniesAuthCode { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;

        public HttpSettings HttpOptions { get; set; } = new HttpSettings();
    }
}
