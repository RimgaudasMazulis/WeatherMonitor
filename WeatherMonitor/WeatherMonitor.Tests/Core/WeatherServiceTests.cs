using Microsoft.Extensions.Logging;
using Moq;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models;
using WeatherMonitor.Core.Services;
using Xunit;


namespace WeatherMonitor.Tests.Core
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherRepository> _mockRepository;
        private readonly Mock<IWeatherApiClient> _mockApiClient;
        private readonly Mock<ILogger<WeatherService>> _logger;
        private readonly WeatherService _service;

        public WeatherServiceTests()
        {
            _mockRepository = new Mock<IWeatherRepository>();
            _mockApiClient = new Mock<IWeatherApiClient>();
            _logger = new Mock<ILogger<WeatherService>>();
            _service = new WeatherService(_mockRepository.Object, _mockApiClient.Object, _logger.Object);
        }

        [Fact]
        public async Task GetAllWeatherRecordsAsync_ShouldReturnAllRecords()
        {
            // Arrange
            var expectedRecords = new List<WeatherRecord>
            {
                new WeatherRecord
                {
                    Id = 1,
                    Country = "US",
                    City = "New York",
                    Temperature = 20.5m,
                    MinTemperature = 18.0m,
                    MaxTemperature = 22.0m,
                    RecordedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                new WeatherRecord
                {
                    Id = 2,
                    Country = "UK",
                    City = "London",
                    Temperature = 15.0m,
                    MinTemperature = 12.0m,
                    MaxTemperature = 17.0m,
                    RecordedAt = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(repo => repo.GetAllWeatherRecordsAsync())
                .ReturnsAsync(expectedRecords);

            // Act
            var result = await _service.GetAllWeatherRecordsAsync();

            // Assert
            Assert.Equal(expectedRecords, result);
            _mockRepository.Verify(repo => repo.GetAllWeatherRecordsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetWeatherRecordByCityAsync_ShouldReturnCorrectRecord()
        {
            // Arrange
            string city = "Paris";
            string country = "FR";
            var expectedRecord = new WeatherRecord
            {
                Id = 3,
                Country = country,
                City = city,
                Temperature = 18.0m,
                MinTemperature = 15.0m,
                MaxTemperature = 20.0m,
                RecordedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            _mockRepository.Setup(repo => repo.GetWeatherRecordByCityAsync(city))
                .ReturnsAsync(expectedRecord);

            // Act
            var result = await _service.GetWeatherRecordByCityAsync(city);

            // Assert
            Assert.Equal(expectedRecord, result);
            _mockRepository.Verify(repo => repo.GetWeatherRecordByCityAsync(city), Times.Once);
        }

        [Fact]
        public async Task UpdateWeatherDataAsync_ShouldUpdateAllMonitoredLocations()
        {
            // Arrange
            _mockApiClient.Setup(client => client.GetWeatherDataAsync(It.IsAny<string>()))
                .ReturnsAsync(new WeatherApiResponse
                {
                    City = "TestCity",
                    Country = "TestCountry",
                    Temperature = 25.0m,
                    MinTemperature = 20.0m,
                    MaxTemperature = 30.0m,
                    LastUpdated = DateTime.UtcNow
                });

            _mockRepository.Setup(repo => repo.UpdateWeatherRecordAsync(It.IsAny<WeatherRecord>()))
                .ReturnsAsync((WeatherRecord record) => record);

            // Act
            await _service.UpdateWeatherDataAsync();

            _mockApiClient.Verify(client => client.GetWeatherDataAsync(It.IsAny<string>()),
                Times.AtLeast(1));
            _mockRepository.Verify(repo => repo.UpdateWeatherRecordAsync(It.IsAny<WeatherRecord>()),
                Times.AtLeast(1));
        }
    }
}
