using System;

namespace BoardOutlook.Application.DTOs.Response
{
    /// <summary>
    /// Represents an executive whose compensation exceeds the industry benchmark by a certain threshold.
    /// </summary>
    public class ExecutiveCompensationResultDto
    {
        /// <summary>
        /// Full name and position of the executive.
        /// </summary>
        public string NameAndPosition { get; init; }

        /// <summary>
        /// Total compensation of the executive.
        /// </summary>
        public decimal TotalCompensation { get; init; }

        /// <summary>
        /// Average compensation in the executive's industry for the corresponding year.
        /// </summary>
        public decimal IndustryAverage { get; init; }

        /// <summary>
        /// Constructor to initialize all required properties.
        /// </summary>
        /// <param name="nameAndPosition">Name and position of the executive</param>
        /// <param name="totalCompensation">Total compensation of the executive</param>
        /// <param name="industryAverage">Industry average compensation</param>
        public ExecutiveCompensationResultDto(string nameAndPosition, decimal totalCompensation, decimal industryAverage)
        {
            NameAndPosition = nameAndPosition ?? throw new ArgumentNullException(nameof(nameAndPosition));
            TotalCompensation = totalCompensation;
            IndustryAverage = industryAverage;
        }
    }
}
