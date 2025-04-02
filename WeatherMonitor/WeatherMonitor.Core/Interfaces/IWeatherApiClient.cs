using WeatherMonitor.Core.Models;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<WeatherApiResponse> GetWeatherDataAsync(string city);
    }
}
