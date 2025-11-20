using Moq;
using Xunit;
using BoardOutlook.Application.Features.Benchmarks;
using BoardOutlook.Application.Features.Common;
using BoardOutlook.Application.Features.Companies;
using BoardOutlook.Application.Features.Executives;
using BoardOutlook.Application.Services;
using BoardOutlook.Domain.Entities;
using Microsoft.Extensions.Logging;



namespace BoardOutlook.Test
{
    public class ExecutiveCompensationServiceTests
    {
        private readonly Mock<IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>>> _mockCompaniesHandler;
        private readonly Mock<IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>>> _mockExecutivesHandler;
        private readonly Mock<IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark>> _mockBenchmarkHandler;
        private readonly Mock<ILogger<ExecutiveCompensationService>> _mockLogger;

        private readonly ExecutiveCompensationService _service;

        public ExecutiveCompensationServiceTests()
        {
            _mockCompaniesHandler = new Mock<IQueryHandler<GetAsxCompanyQuery, IEnumerable<Company>>>();
            _mockExecutivesHandler = new Mock<IQueryHandler<GetExecutivesQuery, IEnumerable<Executive>>>();
            _mockBenchmarkHandler = new Mock<IQueryHandler<GetIndustryBenchmarkQuery, IndustryBenchmark>>();
            _mockLogger = new Mock<ILogger<ExecutiveCompensationService>>();

            _service = new ExecutiveCompensationService(
                _mockCompaniesHandler.Object,
                _mockExecutivesHandler.Object,
                _mockBenchmarkHandler.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetCompaniesAsync_ReturnsCompanyDtos()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company("ABC", "ABC Ltd", "Tech", "AU", 100, 10.5m, "ASX", "ASX", SecurityType.Stock)
            };

            _mockCompaniesHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetAsxCompanyQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            // Act
            var result = (await _service.GetCompaniesAsync("ASX")).ToList();

            // Assert
            
            Assert.AreEqual("ABC", result[0].Symbol);
            Assert.AreEqual("ABC Ltd", result[0].Name);
        }

        [Fact]
        public async Task GetExecutivesAsync_ReturnsExecutiveDtos()
        {
            // Arrange
            var executives = new List<Executive>
            {
                new Executive("0001", new CompanySymbol("ABC"), "ABC Ltd", "Tech", DateTime.UtcNow, DateTime.UtcNow,
                    "John Doe CEO", 2024, 200_000, 50_000, 100_000, 0, 0, 400_000, "http://url.com")
            };

            _mockExecutivesHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetExecutivesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(executives);

            // Act
            var result = (await _service.GetExecutivesAsync("ABC")).ToList();

            // Assert            
            Assert.Equals("John Doe CEO", result[0].Name);
            Assert.AreEqual(400_000, result[0].TotalCompensation);
        }

        [Fact]
        public async Task GetIndustryBenchmarkAsync_ReturnsBenchmarkDto()
        {
            // Arrange
            var benchmark = new IndustryBenchmark(new Industry("Tech"), 2024, 350_000m);

            _mockBenchmarkHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetIndustryBenchmarkQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(benchmark);

            // Act
            var result = await _service.GetIndustryBenchmarkAsync("Tech");

            // Assert
            Assert.IsNotNull(result);
            Assert.Equals("Tech", result.IndustryTitle);
            Assert.Equals(350_000m, result.AverageCompensation);
        }

        [Fact]
        public async Task GetHighPaidExecutivesAsync_ReturnsExecutivesAboveThreshold()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company("ABC", "ABC Ltd", "Tech", "AU", 100, 10.5m, "ASX", "ASX", SecurityType.Stock)
            };

            var executives = new List<Executive>
            {
                new Executive("0001", new CompanySymbol("ABC"), "ABC Ltd", "Tech", DateTime.UtcNow, DateTime.UtcNow,
                    "John Doe CEO", 2024, 200_000, 50_000, 100_000, 0, 0, 400_000, "http://url.com")
            };

            var benchmark = new IndustryBenchmark(new Industry("Tech"), 2024, 350_000m);

            _mockCompaniesHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetAsxCompanyQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companies);

            _mockExecutivesHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetExecutivesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(executives);

            _mockBenchmarkHandler
                .Setup(h => h.ExecuteAsync(It.IsAny<GetIndustryBenchmarkQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(benchmark);

            // Act
            var results = (await _service.GetHighPaidExecutivesAsync("ASX")).ToList();

            // Assert            
            Assert.Equals("John Doe CEO", results[0].NameAndPosition);
            Assert.Equals(400_000, results[0].TotalCompensation);
            Assert.Equals(350_000, results[0].IndustryAverage);
        }

        [Fact]
        public async Task GetHighPaidExecutivesAsync_EmptyExchange_ReturnsEmpty()
        {
            // Act
            var results = (await _service.GetHighPaidExecutivesAsync("")).ToList();

            // Assert
            Assert.AreEqual(results.Count, 0);
        }
    }
}
