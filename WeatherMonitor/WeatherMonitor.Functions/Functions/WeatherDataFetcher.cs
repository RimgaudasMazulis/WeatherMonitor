using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WeatherMonitor.Functions.Services.Interfaces;

namespace WeatherMonitor.Functions.Functions
{
    public class WeatherDataFetcher
    {
        private readonly IWeatherAzureService _weatherService;
        private readonly ILogger _logger;

        public WeatherDataFetcher(IWeatherAzureService weatherService, ILoggerFactory loggerFactory)
        {
            _weatherService = weatherService;
            _logger = loggerFactory.CreateLogger<WeatherDataFetcher>();
        }

        [Function("FetchWeatherData")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo timerInfo)
        {
            _logger.LogInformation($"Weather data fetcher function executed at: {DateTime.UtcNow}");

            await _weatherService.FetchAndStoreWeatherDataAsync("London");
        }
    }
}
