using Moq;
using Polly.Registry;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using BoardOutlook.Infrastructure.Configuration;
using BoardOutlook.Infrastructure.Repositories;
using BoardOutlook.Infrastructure.ExternalApis.Dtos;


namespace BoardOutlook.Test
{
    [TestClass]
    public class MarketDataRepositoryTests
    {
        private readonly Mock<IOptions<ExternalApiSettings>> _mockSettings;
        private readonly Mock<ILogger<MarketDataRepository>> _mockLogger;
        private readonly Mock<IPolicyRegistry<string>> _mockPolicyRegistry;

        public MarketDataRepositoryTests()
        {
            _mockSettings = new Mock<IOptions<ExternalApiSettings>>();
            _mockLogger = new Mock<ILogger<MarketDataRepository>>();
            _mockPolicyRegistry = new Mock<IPolicyRegistry<string>>();

            _mockSettings.Setup(s => s.Value).Returns(new ExternalApiSettings
            {
                BaseUrl = "https://fakeapi.com",
                CompaniesAuthCode = "companiescode",
                AuthCode = "authcode"
            });
        }

        private MarketDataRepository CreateRepository(Mock<MarketDataRepository> repoMock = null)
        {
            if (repoMock != null) return repoMock.Object;

            return new MarketDataRepository(
                httpClient: new HttpClient(),
                settings: _mockSettings.Object,
                logger: _mockLogger.Object,
                policyRegistry: _mockPolicyRegistry.Object
            );
        }

        [Fact]
        public async Task GetCompaniesAsync_ReturnsMappedCompanies()
        {
            // Arrange
            var fakeApiResponse = new List<CompanyApiResponse>
            {
                new CompanyApiResponse
                {
                    Symbol = "ABC",
                    Name = "ABC Ltd",
                    Sector = "Tech",
                    Country = "AU",
                    FullTimeEmployees = "100",
                    Price = 10.5m,
                    Exchange = "ASX",
                    ExchangeShortName = "ASX",
                    Type = "stock"
                }
            };

            var repoMock = new Mock<MarketDataRepository>(
                new HttpClient(),
                _mockSettings.Object,
                _mockLogger.Object,
                _mockPolicyRegistry.Object
            )
            { CallBase = true };

            repoMock
                .Setup(r => r.GetAsync<List<CompanyApiResponse>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeApiResponse);

            var repo = CreateRepository(repoMock);

            // Act
            var companies = (await repo.GetCompaniesAsync("ASX")).ToList();

            // Assert
            Assert.AreEqual(companies.Count, 0);
            Assert.Equals("ABC", companies[0].Symbol);
            Assert.Equals("ABC Ltd", companies[0].Name);
            Assert.Equals(100, companies[0].FullTimeEmployees);
        }

        [Fact]
        public async Task GetExecutivesAsync_ReturnsMappedExecutives()
        {
            // Arrange
            var fakeApiResponse = new List<ExecutiveApiResponse>
            {
                new ExecutiveApiResponse
                {
                    Cik = "000123",
                    Symbol = "ABC",
                    CompanyName = "ABC Ltd",
                    IndustryTitle = "Tech",
                    AcceptedDate = "2025-01-01 12:00:00",
                    FilingDate = "2025-01-01",
                    NameAndPosition = "John Doe CEO",
                    Year = 2024,
                    Salary = 200000,
                    Bonus = 50000,
                    StockAward = 100000,
                    IncentivePlanCompensation = null,
                    AllOtherCompensation = null,
                    Total = 400000,
                    Url = "http://fakeurl.com"
                }
            };

            var repoMock = new Mock<MarketDataRepository>(
                new HttpClient(),
                _mockSettings.Object,
                _mockLogger.Object,
                _mockPolicyRegistry.Object
            )
            { CallBase = true };

            repoMock
                .Setup(r => r.GetAsync<List<ExecutiveApiResponse>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeApiResponse);

            var repo = CreateRepository(repoMock);

            // Act
            var executives = (await repo.GetExecutivesAsync("ABC")).ToList();

            // Assert
            Assert.AreEqual(executives.Count, 0);
            Assert.Equals("John Doe CEO", executives[0].NameAndPosition);
            Assert.Equals(400000, executives[0].Total);
            Assert.Equals("Tech", executives[0].IndustryTitle);
        }

        [Fact]
        public async Task GetIndustryBenchmarkAsync_ReturnsMappedBenchmark()
        {
            // Arrange
            var fakeApiResponse = new BenchmarkApiResponse
            {
                Year = 2024,
                AverageCompensation = 350000m
            };

            var repoMock = new Mock<MarketDataRepository>(
                new HttpClient(),
                _mockSettings.Object,
                _mockLogger.Object,
                _mockPolicyRegistry.Object
            )
            { CallBase = true };

            repoMock
                .Setup(r => r.GetAsync<BenchmarkApiResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeApiResponse);

            var repo = CreateRepository(repoMock);

            // Act
            var benchmark = await repo.GetIndustryBenchmarkAsync("Tech");

            // Assert
            Assert.Equals(2024, benchmark.Year);
            Assert.Equals(350000m, benchmark.AverageCompensation);
            Assert.Equals("Tech", benchmark.Industry.Title);
        }
    }
}
