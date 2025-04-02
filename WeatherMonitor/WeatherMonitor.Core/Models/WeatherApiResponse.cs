namespace WeatherMonitor.Core.Models
{
    public class WeatherApiResponse
    {
        public string City { get; set; }
        public string Country { get; set; }
        public decimal Temperature { get; set; }
        public decimal MinTemperature { get; set; }
        public decimal MaxTemperature { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
