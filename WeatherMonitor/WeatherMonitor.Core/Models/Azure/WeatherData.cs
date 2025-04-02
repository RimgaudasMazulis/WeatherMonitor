namespace WeatherMonitor.Core.Models.Azure
{
    public class WeatherData
    {
        public string LogId { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public string RawData { get; set; } = default!;
    }
}
