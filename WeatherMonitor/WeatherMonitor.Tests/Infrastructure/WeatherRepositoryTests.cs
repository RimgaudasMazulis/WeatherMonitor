using Microsoft.EntityFrameworkCore;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Infrastructure.Data;
using WeatherMonitor.Infrastructure.Repositories;

namespace WeatherMonitor.Tests.Infrastructure
{
    public class WeatherRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public WeatherRepositoryTests()
        {
            // Use in-memory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private async Task SeedDatabaseAsync(ApplicationDbContext context)
        {
            await context.WeatherRecords.AddRangeAsync(new[]
            {
                new WeatherRecord
                {
                    Country = "US",
                    City = "New York",
                    Temperature = 20.5m,
                    MinTemperature = 18.0m,
                    MaxTemperature = 22.0m,
                    RecordedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdated = DateTime.UtcNow.AddDays(-1)
                },
                new WeatherRecord
                {
                    Country = "UK",
                    City = "London",
                    Temperature = 15.0m,
                    MinTemperature = 12.0m,
                    MaxTemperature = 17.0m,
                    RecordedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdated = DateTime.UtcNow.AddDays(-1)
                },
                new WeatherRecord
                {
                    Country = "FR",
                    City = "Paris",
                    Temperature = 18.0m,
                    MinTemperature = 15.0m,
                    MaxTemperature = 20.0m,
                    RecordedAt = DateTime.UtcNow.AddDays(-1),
                    LastUpdated = DateTime.UtcNow.AddDays(-1)
                }
            });

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllWeatherRecordsAsync_ShouldReturnAllRecords()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            await SeedDatabaseAsync(context);
            var repository = new WeatherRepository(context);

            // Act
            var records = await repository.GetAllWeatherRecordsAsync();

            // Assert
            Assert.Equal(3, records.Count());
        }

        [Fact]
        public async Task GetWeatherRecordsByCountryAsync_ShouldReturnRecordsForCountry()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            await SeedDatabaseAsync(context);
            var repository = new WeatherRepository(context);

            // Act
            var records = await repository.GetWeatherRecordsByCountryAsync("US");

            // Assert
            Assert.Single(records);
            Assert.Equal("New York", records.First().City);
        }

        [Fact]
        public async Task GetWeatherRecordByCityAsync_ShouldReturnCorrectRecord()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            await SeedDatabaseAsync(context);
            var repository = new WeatherRepository(context);

            // Act
            var record = await repository.GetWeatherRecordByCityAsync("London");

            // Assert
            Assert.NotNull(record);
            Assert.Equal("London", record.City);
            Assert.Equal("UK", record.Country);
        }

        [Fact]
        public async Task UpdateWeatherRecordAsync_ShouldUpdateExistingRecord()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            await SeedDatabaseAsync(context);
            var repository = new WeatherRepository(context);

            var updatedRecord = new WeatherRecord
            {
                Country = "UK",
                City = "London",
                Temperature = 18.0m,
                MinTemperature = 12.0m,
                MaxTemperature = 18.0m,
                RecordedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            // Act
            var result = await repository.UpdateWeatherRecordAsync(updatedRecord);

            // Assert
            Assert.Equal(18.0m, result.Temperature);

            // Verify the record was updated in the database
            var record = await context.WeatherRecords
                .FirstOrDefaultAsync(w => w.City == "London" && w.Country == "UK");
            Assert.NotNull(record);
            Assert.Equal(18.0m, record.Temperature);
            Assert.Equal(18.0m, record.MaxTemperature);
        }

        [Fact]
        public async Task UpdateWeatherRecordAsync_ShouldCreateNewRecordIfNotExists()
        {
            // Arrange
            using var context = new ApplicationDbContext(_dbContextOptions);
            await SeedDatabaseAsync(context);
            var repository = new WeatherRepository(context);

            var newRecord = new WeatherRecord
            {
                Country = "DE",
                City = "Berlin",
                Temperature = 14.0m,
                MinTemperature = 10.0m,
                MaxTemperature = 16.0m,
                RecordedAt = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            // Act
            var result = await repository.UpdateWeatherRecordAsync(newRecord);

            // Assert
            Assert.Equal("Berlin", result.City);

            // Verify the record was added to the database
            var records = await context.WeatherRecords.ToListAsync();
            Assert.Equal(4, records.Count);
            Assert.Contains(records, r => r.City == "Berlin" && r.Country == "DE");
        }
    }
}
