using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Moq;
using WeatherMonitor.Core.Models.Azure;
using WeatherMonitor.Infrastructure.Repositories.Azure;
using Xunit;

namespace WeatherMonitor.Tests.Infrastructure.Repositories.Azure
{
    public class WeatherLogRepositoryTests
    {
        private readonly Mock<TableClient> _mockTableClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly WeatherLogRepository _repository;

        public WeatherLogRepositoryTests()
        {
            _mockTableClient = new Mock<TableClient>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration
            _mockConfiguration.Setup(c => c["AzureWebJobsStorage"]).Returns("UseDevelopmentStorage=true");

            // Create a factory that allows testing the repository without actual Azure Table Storage
            _repository = new WeatherLogRepositoryForTest(
                _mockConfiguration.Object,
                _mockTableClient.Object
            );
        }

        [Fact]
        public async Task AddLogAsync_CallsTableClientAddEntity()
        {
            // Arrange
            var log = new WeatherLog("London", true);

            // Create a mock Response object
            var mockResponse = new Mock<Response>();

            _mockTableClient.Setup(client => client.AddEntityAsync(
                It.IsAny<WeatherLog>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(mockResponse.Object);

            // Act
            await _repository.AddLogAsync(log);

            // Assert
            _mockTableClient.Verify(client => client.AddEntityAsync(
                It.Is<WeatherLog>(l => l.City == "London" && l.IsSuccess),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetLogByIdAsync_WhenFound_ReturnsLog()
        {
            // Arrange
            string partitionKey = "20250401";
            string rowKey = "123456-abcdef";
            var expectedLog = new WeatherLog("London", true)
            {
                PartitionKey = partitionKey,
                RowKey = rowKey
            };

            _mockTableClient.Setup(client => client.GetEntityAsync<WeatherLog>(
                partitionKey,
                rowKey,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(Response.FromValue(expectedLog, null));

            // Act
            var result = await _repository.GetLogByIdAsync(partitionKey, rowKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("London", result.City);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetLogByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            string partitionKey = "20250401";
            string rowKey = "123456-abcdef";

            _mockTableClient.Setup(client => client.GetEntityAsync<WeatherLog>(
                partitionKey,
                rowKey,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new RequestFailedException(404, "Not found"));

            // Act
            var result = await _repository.GetLogByIdAsync(partitionKey, rowKey);

            // Assert
            Assert.Null(result);
        }

        // This is a testable version of WeatherLogRepository that allows injecting a mock TableClient
        public class WeatherLogRepositoryForTest : WeatherLogRepository
        {
            private readonly TableClient _tableClient;

            public WeatherLogRepositoryForTest(IConfiguration configuration, TableClient tableClient)
                : base(configuration)
            {
                _tableClient = tableClient;
            }

            protected override TableClient GetTableClient()
            {
                return _tableClient;
            }
        }
    }
}
