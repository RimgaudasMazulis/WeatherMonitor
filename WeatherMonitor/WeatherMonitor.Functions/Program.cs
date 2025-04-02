using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Functions.Services;
using WeatherMonitor.Functions.Services.Interfaces;
using WeatherMonitor.Infrastructure.Repositories.Azure;

namespace WeatherMonitor.Functions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<IWeatherLogRepository, WeatherLogRepository>();
                    services.AddSingleton<IWeatherPayloadRepository, WeatherPayloadRepository>();
                    services.AddSingleton<IWeatherAzureService, WeatherAzureService>();
                })
                .Build();

            host.Run();
        }
    }
}
