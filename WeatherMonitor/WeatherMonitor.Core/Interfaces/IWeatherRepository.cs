using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync();
        Task<IEnumerable<WeatherRecord>> GetWeatherRecordsByCountryAsync(string country);
        Task<WeatherRecord> GetWeatherRecordByCityAsync(string city, string country);
        Task<WeatherRecord> AddWeatherRecordAsync(WeatherRecord weatherRecord);
        Task<WeatherRecord> UpdateWeatherRecordAsync(WeatherRecord weatherRecord);
        Task<IEnumerable<WeatherRecord>> GetMinMaxTemperaturesByCityAsync(string city, string country, DateTime startDate, DateTime endDate);
    }
}
