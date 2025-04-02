using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Server.Controllers;

namespace WeatherMonitor.Tests.API.Controllers
{
    public class WeatherControllerTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherController _controller;

        public WeatherControllerTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task GetAllWeatherRecords_ReturnsOkResultWithRecords()
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

            _mockWeatherService.Setup(service => service.GetAllWeatherRecordsAsync())
                .ReturnsAsync(expectedRecords);

            // Act
            var result = await _controller.GetAllWeatherRecords();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<WeatherRecord>>(okResult.Value);
            Assert.Equal(expectedRecords, returnValue);
        }

        [Fact]
        public async Task GetWeatherByCity_ReturnsOkResultWithRecord_WhenRecordExists()
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

            _mockWeatherService.Setup(service => service.GetWeatherRecordByCityAsync(city, country))
                .ReturnsAsync(expectedRecord);

            // Act
            var result = await _controller.GetWeatherByCity(country, city);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<WeatherRecord>(okResult.Value);
            Assert.Equal(expectedRecord, returnValue);
        }

        [Fact]
        public async Task GetWeatherByCity_ReturnsNotFound_WhenRecordDoesNotExist()
        {
            // Arrange
            string city = "NonExistentCity";
            string country = "NonExistentCountry";

            _mockWeatherService.Setup(service => service.GetWeatherRecordByCityAsync(city, country))
                .ReturnsAsync((WeatherRecord)null);

            // Act
            var result = await _controller.GetWeatherByCity(country, city);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMinMaxTemperatures_ReturnsOkResultWithRecords()
        {
            // Arrange
            string city = "Paris";
            string country = "FR";
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            var expectedRecords = new List<WeatherRecord>
            {
                new WeatherRecord
                {
                    Id = 3,
                    Country = country,
                    City = city,
                    Temperature = 18.0m,
                    MinTemperature = 15.0m,
                    MaxTemperature = 20.0m,
                    RecordedAt = DateTime.UtcNow.AddDays(-6),
                    LastUpdated = DateTime.UtcNow.AddDays(-6)
                },
                new WeatherRecord
                {
                    Id = 4,
                    Country = country,
                    City = city,
                    Temperature = 19.5m,
                    MinTemperature = 16.0m,
                    MaxTemperature = 22.0m,
                    RecordedAt = DateTime.UtcNow.AddDays(-3),
                    LastUpdated = DateTime.UtcNow.AddDays(-3)
                }
            };

            _mockWeatherService.Setup(service => service.GetMinMaxTemperaturesByCityAsync(
                    city, country, startDate, endDate))
                .ReturnsAsync(expectedRecords);

            // Act
            var result = await _controller.GetMinMaxTemperatures(country, city, startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<WeatherRecord>>(okResult.Value);
            Assert.Equal(expectedRecords, returnValue);
        }
    }
}
