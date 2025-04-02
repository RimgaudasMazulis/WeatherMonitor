using System.Threading.Tasks;

namespace WeatherMonitor.Functions.Services.Interfaces
{
    public interface IWeatherAzureService
    {
        Task FetchAndStoreWeatherDataAsync(string city);
    }
}
