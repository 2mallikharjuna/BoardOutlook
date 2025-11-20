using System;

namespace BoardOutlook.Domain.ValueObjects
{
    public record ExecutiveCompensationResult(
    string NameAndPosition,
    decimal Compensation,
    decimal AverageIndustryCompensation);
}
