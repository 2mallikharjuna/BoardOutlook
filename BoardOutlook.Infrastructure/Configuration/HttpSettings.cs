using System;

namespace BoardOutlook.Infrastructure.Configuration
{
    public class HttpSettings
    {
        public int RetryCount { get; set; }
        public int RetryBaseDelaySeconds { get; set; }
        public int CircuitBreakerFailures { get; set; }
        public int CircuitBreakerDurationSeconds { get; set; }
    }
}
