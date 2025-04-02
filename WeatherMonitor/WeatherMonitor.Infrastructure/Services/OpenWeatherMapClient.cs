using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models;

namespace WeatherMonitor.Infrastructure.Services
{
    public class OpenWeatherMapClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenWeatherMapClient> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public OpenWeatherMapClient(HttpClient httpClient, IConfiguration configuration, ILogger<OpenWeatherMapClient> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["WeatherApi:OpenWeatherMap:ApiKey"];
            _baseUrl = configuration["WeatherApi:OpenWeatherMap:BaseUrl"];
            _logger = logger;
        }

        public async Task<WeatherApiResponse> GetWeatherDataAsync(string city, string country)
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/weather?q={city},{country}&units=metric&appid={_apiKey}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var weatherData = await JsonSerializer.DeserializeAsync<OpenWeatherResponse>(responseStream);

            return new WeatherApiResponse
            {
                City = city,
                Country = country,
                Temperature = (decimal)weatherData.main.temp,
                MinTemperature = (decimal)weatherData.main.temp_min,
                MaxTemperature = (decimal)weatherData.main.temp_max,
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<string> GetWeatherDataAsStringAsync(string city)
        {
            _logger.LogInformation($"Fetching weather data for {city}");
            var url = $"{_baseUrl}/weather?q={city}&appid={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private class OpenWeatherResponse
        {
            public MainData main { get; set; }

            public class MainData
            {
                public float temp { get; set; }
                public float temp_min { get; set; }
                public float temp_max { get; set; }
            }
        }
    }
}
