using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;
using WeatherMonitor.Functions.Services.Interfaces;

namespace WeatherMonitor.Functions.Services
{
    public class WeatherAzureService : IWeatherAzureService
    {
        private readonly HttpClient _httpClient;
        private readonly IWeatherLogRepository _logRepository;
        private readonly IWeatherPayloadRepository _payloadRepository;
        private readonly ILogger<WeatherAzureService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public WeatherAzureService(
            HttpClient httpClient,
            IWeatherLogRepository logRepository,
            IWeatherPayloadRepository payloadRepository,
            IConfiguration configuration,
            ILogger<WeatherAzureService> logger)
        {
            _httpClient = httpClient;
            _logRepository = logRepository;
            _payloadRepository = payloadRepository;
            _logger = logger;
            _apiKey = configuration["OpenWeatherMapApiKey"] ?? throw new ArgumentNullException("OpenWeatherMapApiKey configuration is missing");
            _baseUrl = configuration["OpenWeatherMapBaseUrl"] ?? throw new ArgumentNullException("OpenWeatherMapBaseUrl configuration is missing");
        }

        public async Task FetchAndStoreWeatherDataAsync(string city)
        {
            var log = new WeatherLog(city, false);

            try
            {
                _logger.LogInformation($"Fetching weather data for {city}");
                var url = $"{_baseUrl}/weather?q={city}&appid={_apiKey}";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                await _payloadRepository.StorePayloadAsync(log.BlobId, content);

                log.IsSuccess = true;

                _logger.LogInformation($"Successfully fetched and stored weather data for {city}");
            }
            catch (Exception ex)
            {
                log.ErrorMessage = ex.Message;
                _logger.LogError(ex, $"Error fetching weather data for {city}");
            }
            finally
            {
                await _logRepository.AddLogAsync(log);
            }
        }
    }
}
