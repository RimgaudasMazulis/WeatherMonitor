namespace WeatherMonitor.Core.Entities
{
    public class WeatherRecord
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public decimal Temperature { get; set; }
        public decimal MinTemperature { get; set; }
        public decimal MaxTemperature { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
