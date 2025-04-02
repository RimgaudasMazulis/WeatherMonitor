using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<WeatherApiResponse> GetWeatherDataAsync(string city)
        {
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/weather?q={city}&units=metric&appid={_apiKey}");

            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            var weatherData = await JsonSerializer.DeserializeAsync<OpenWeatherResponse>(responseStream);

            return new WeatherApiResponse
            {
                City = city,
                Country = weatherData?.Sys.Country ?? string.Empty,
                Temperature = (decimal)weatherData.Main.Temperature,
                MinTemperature = (decimal)weatherData.Main.MinTemperature,
                MaxTemperature = (decimal)weatherData.Main.MaxTemperature,
                LastUpdated = DateTime.UtcNow
            };
        }

        private class OpenWeatherResponse
        {
            [JsonPropertyName("main")]
            public MainData Main { get; set; }

            [JsonPropertyName("sys")]
            public SysData Sys { get; set; }

            public class MainData
            {
                [JsonPropertyName("temp")]
                public float Temperature { get; set; }

                [JsonPropertyName("temp_min")]
                public float MinTemperature { get; set; }

                [JsonPropertyName("temp_max")]
                public float MaxTemperature { get; set; }
            }

            public class SysData
            {
                [JsonPropertyName("country")]
                public string Country { get; set; }
            }
        }
    }
}
