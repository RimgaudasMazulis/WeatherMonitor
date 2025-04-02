using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync();
        Task<WeatherRecord> GetWeatherRecordByCityAsync(string city, string country);
        Task UpdateWeatherDataAsync();
        Task<IEnumerable<WeatherRecord>> GetMinMaxTemperaturesByCityAsync(string city, string country, DateTime startDate, DateTime endDate);
    }
}
