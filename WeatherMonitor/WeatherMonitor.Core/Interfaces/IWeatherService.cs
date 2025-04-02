using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync();
        Task<WeatherRecord> GetWeatherRecordByCityAsync(string city);
        Task UpdateWeatherDataAsync();
    }
}
