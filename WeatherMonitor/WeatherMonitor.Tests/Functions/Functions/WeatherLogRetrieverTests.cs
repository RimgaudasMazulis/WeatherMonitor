using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;
using WeatherMonitor.Functions.Functions;
using Xunit;

namespace WeatherMonitor.Tests.Functions.Functions
{
    public class WeatherLogRetrieverTests
    {
        private readonly Mock<IWeatherLogRepository> _mockLogRepository;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly WeatherLogRetriever _function;

        public WeatherLogRetrieverTests()
        {
            _mockLogRepository = new Mock<IWeatherLogRepository>();
            _mockLogger = new Mock<ILogger>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _function = new WeatherLogRetriever(
                _mockLogRepository.Object,
                _mockLoggerFactory.Object
            );
        }

        [Fact]
        public async Task Run_WithValidDateRange_ReturnsLogs()
        {
            // Arrange
            var fromDate = new DateTime(2025, 4, 1);
            var toDate = new DateTime(2025, 4, 2);
            var expectedLogs = new List<WeatherLog>
            {
                new WeatherLog("London", true) { PartitionKey = "20250401", RowKey = "123456-abcdef" }
            };

            var context = new Mock<FunctionContext>();

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri($"http://localhost/api/GetWeatherLogs?from={fromDate:yyyy-MM-dd}&to={toDate:yyyy-MM-dd}");
            request.Setup(r => r.Url).Returns(uri);

            _mockLogRepository.Setup(repo => repo.GetLogsByTimeRangeAsync(
                It.Is<DateTime>(d => d.Date == fromDate.Date),
                It.Is<DateTime>(d => d.Date == toDate.Date)
            )).ReturnsAsync(expectedLogs);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            var responseBodyStream = new MemoryStream();
            response.Setup(r => r.Body).Returns(responseBodyStream);
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Run_WithInvalidDateRange_ReturnsBadRequest()
        {
            // Arrange
            var context = new Mock<FunctionContext>();

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri("http://localhost/api/GetWeatherLogs");
            request.Setup(r => r.Url).Returns(uri);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.BadRequest);
            var responseBodyStream = new MemoryStream();
            response.Setup(r => r.Body).Returns(responseBodyStream);
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }
    }
}
