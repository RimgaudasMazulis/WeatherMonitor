using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Server.Services
{
    public class WeatherUpdateService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WeatherUpdateService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(1);

        public WeatherUpdateService(
            IServiceProvider serviceProvider,
            ILogger<WeatherUpdateService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Weather Update Service is starting");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Updating weather data at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
                        await weatherService.UpdateWeatherDataAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating weather data");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }
        }
    }
}
