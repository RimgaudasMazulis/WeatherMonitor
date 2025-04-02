using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherMonitor.Functions.Functions;
using WeatherMonitor.Functions.Services.Interfaces;
using Xunit;

namespace WeatherMonitor.Tests.Functions.Functions
{
    public class WeatherDataFetcherTests
    {
        private readonly Mock<IWeatherAzureService> _mockWeatherService;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly WeatherDataFetcher _function;

        public WeatherDataFetcherTests()
        {
            _mockWeatherService = new Mock<IWeatherAzureService>();
            _mockLogger = new Mock<ILogger>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _function = new WeatherDataFetcher(
                _mockWeatherService.Object,
                _mockLoggerFactory.Object
            );
        }

        [Fact]
        public async Task Run_WhenTriggered_CallsWeatherService()
        {
            // Arrange
            var timerInfo = new TimerInfo();  // Creating a simple instance since we don't need actual timer data.

            _mockWeatherService.Setup(service => service.FetchAndStoreWeatherDataAsync("London"))
                .Returns(Task.CompletedTask);

            // Act
            await _function.Run(timerInfo);

            // Assert
            _mockWeatherService.Verify(service => service.FetchAndStoreWeatherDataAsync("London"), Times.Once);
            _mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(l => l == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Weather data fetcher function executed at:")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), Times.Once);
        }
    }
}
