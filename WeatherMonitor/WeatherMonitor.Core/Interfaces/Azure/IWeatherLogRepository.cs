using WeatherMonitor.Core.Models.Azure;

namespace WeatherMonitor.Core.Interfaces.Azure
{
    public interface IWeatherLogRepository
    {
        Task AddLogAsync(WeatherLog log);
        Task<IEnumerable<WeatherLog>> GetLogsByTimeRangeAsync(DateTime startTime, DateTime endTime);
        Task<WeatherLog?> GetLogByIdAsync(string partitionKey, string rowKey);
    }
}
