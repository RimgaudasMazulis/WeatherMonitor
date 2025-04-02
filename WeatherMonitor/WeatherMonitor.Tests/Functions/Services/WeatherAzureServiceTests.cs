using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;
using WeatherMonitor.Functions.Services;
using Xunit;

namespace WeatherMonitor.Tests.Functions.Services
{
    public class WeatherAzureServiceTests
    {
        private readonly Mock<IWeatherLogRepository> _mockLogRepository;
        private readonly Mock<IWeatherPayloadRepository> _mockPayloadRepository;
        private readonly Mock<ILogger<WeatherAzureService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly HttpClient _httpClient;
        private readonly WeatherAzureService _service;

        public WeatherAzureServiceTests()
        {
            _mockLogRepository = new Mock<IWeatherLogRepository>();
            _mockPayloadRepository = new Mock<IWeatherPayloadRepository>();
            _mockLogger = new Mock<ILogger<WeatherAzureService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration to return API key
            _mockConfiguration.Setup(c => c["OpenWeatherMapApiKey"]).Returns("test-api-key");
            _mockConfiguration.Setup(c => c["OpenWeatherMapBaseUrl"]).Returns("https://api.openweathermap.org");

            // Setup mock HTTP handler
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"weather\": [{\"description\": \"clear sky\"}]}")
                });

            _httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _service = new WeatherAzureService(
                _httpClient,
                _mockLogRepository.Object,
                _mockPayloadRepository.Object,
                _mockConfiguration.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task FetchAndStoreWeatherDataAsync_Success_StoresDataAndUpdatesLog()
        {
            // Arrange
            string city = "London";
            _mockPayloadRepository.Setup(repo => repo.StorePayloadAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            _mockLogRepository.Setup(repo => repo.AddLogAsync(It.IsAny<WeatherLog>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.FetchAndStoreWeatherDataAsync(city);

            // Assert
            _mockPayloadRepository.Verify(repo => repo.StorePayloadAsync(
                It.IsAny<string>(),
                It.Is<string>(content => content.Contains("clear sky"))
            ), Times.Once);

            _mockLogRepository.Verify(repo => repo.AddLogAsync(
                It.Is<WeatherLog>(log =>
                    log.City == city &&
                    log.IsSuccess == true &&
                    !string.IsNullOrEmpty(log.BlobId))
            ), Times.Once);
        }

        [Fact]
        public async Task FetchAndStoreWeatherDataAsync_ApiFailure_LogsError()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var service = new WeatherAzureService(
                httpClient,
                _mockLogRepository.Object,
                _mockPayloadRepository.Object,
                _mockConfiguration.Object,
                _mockLogger.Object
            );

            string city = "London";
            _mockLogRepository.Setup(repo => repo.AddLogAsync(It.IsAny<WeatherLog>()))
                .Returns(Task.CompletedTask);

            // Act
            await service.FetchAndStoreWeatherDataAsync(city);

            // Assert
            _mockPayloadRepository.Verify(repo => repo.StorePayloadAsync(
                It.IsAny<string>(),
                It.IsAny<string>()
            ), Times.Never);

            _mockLogRepository.Verify(repo => repo.AddLogAsync(
                It.Is<WeatherLog>(log =>
                    log.City == city &&
                    log.IsSuccess == false &&
                    !string.IsNullOrEmpty(log.ErrorMessage))
            ), Times.Once);
        }

        //[Fact]
        //public async Task FetchAndStoreWeatherDataAsync_NullApiKey_ThrowsArgumentNullException()
        //{
        //    // Arrange
        //    var mockConfig = new Mock<IConfiguration>();
        //    mockConfig.Setup(c => c["OpenWeatherMapApiKey"]).Returns((string)null);

        //    // Act & Assert
        //    var service = new WeatherAzureService(
        //        _httpClient,
        //        _mockLogRepository.Object,
        //        _mockPayloadRepository.Object,
        //        mockConfig.Object,
        //        _mockLogger.Object
        //    );

        //    await Assert.ThrowsAsync<ArgumentNullException>(() =>
        //        service.FetchAndStoreWeatherDataAsync("London"));
        //}
    }
}
