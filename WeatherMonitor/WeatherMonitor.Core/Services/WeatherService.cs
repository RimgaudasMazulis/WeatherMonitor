using Microsoft.Extensions.Logging;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Core.Interfaces.Azure;

namespace WeatherMonitor.Core.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _repository;
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly List<(string Country, string City)> _monitoredLocations;

        public WeatherService(
            IWeatherRepository repository,
            IWeatherApiClient weatherApiClient,
            ILogger<WeatherService> logger)
        {
            _repository = repository;
            _weatherApiClient = weatherApiClient;
            _logger = logger;

            _monitoredLocations = new List<(string Country, string City)>
            {
                ("US", "New York"),
                ("US", "Los Angeles"),
                ("UK", "London"),
                ("UK", "Manchester"),
                ("FR", "Paris"),
                ("FR", "Lyon")
            };
        }

        public async Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync()
        {
            return await _repository.GetAllWeatherRecordsAsync();
        }

        public async Task<WeatherRecord> GetWeatherRecordByCityAsync(string city, string country)
        {
            return await _repository.GetWeatherRecordByCityAsync(city, country);
        }

        public async Task UpdateWeatherDataAsync()
        {
            foreach (var (country, city) in _monitoredLocations)
            {
                try
                {
                    var weatherData = await _weatherApiClient.GetWeatherDataAsync(city, country);

                    var weatherRecord = new WeatherRecord
                    {
                        City = city,
                        Country = country,
                        Temperature = weatherData.Temperature,
                        MinTemperature = weatherData.MinTemperature,
                        MaxTemperature = weatherData.MaxTemperature,
                        RecordedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _repository.UpdateWeatherRecordAsync(weatherRecord);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to fetch newest weather information for country: {country}, city {city}", e);
                }
            }
        }

        public async Task<IEnumerable<WeatherRecord>> GetMinMaxTemperaturesByCityAsync(
            string city, string country, DateTime startDate, DateTime endDate)
        {
            return await _repository.GetMinMaxTemperaturesByCityAsync(city, country, startDate, endDate);
        }
    }
}
