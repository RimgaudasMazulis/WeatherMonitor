using Microsoft.Extensions.Logging;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Services.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _repository;
        private readonly IWeatherApiClient _weatherApiClient;
        private readonly ILogger<WeatherService> _logger;
        private readonly List<string> _monitoredLocations;

        public WeatherService(
            IWeatherRepository repository,
            IWeatherApiClient weatherApiClient,
            ILogger<WeatherService> logger)
        {
            _repository = repository;
            _weatherApiClient = weatherApiClient;
            _logger = logger;

            _monitoredLocations = new List<string>
            {
                "New York",
                "Los Angeles",
                "London",
                "Manchester",
                "Paris",
                "Lyon"
            };
        }

        public async Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync()
        {
            return await _repository.GetAllWeatherRecordsAsync();
        }

        public async Task<WeatherRecord> GetWeatherRecordByCityAsync(string city)
        {
            return await _repository.GetWeatherRecordByCityAsync(city);
        }

        public async Task UpdateWeatherDataAsync()
        {
            foreach (var city in _monitoredLocations)
            {
                try
                {
                    var weatherData = await _weatherApiClient.GetWeatherDataAsync(city);

                    var weatherRecord = new WeatherRecord
                    {
                        City = city,
                        Country = weatherData.Country,
                        Temperature = weatherData.Temperature,
                        MinTemperature = weatherData.MinTemperature,
                        MaxTemperature = weatherData.MaxTemperature,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _repository.UpdateWeatherRecordAsync(weatherRecord);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Failed to fetch newest weather information for city {city}", e);
                }
            }
        }
    }
}
