namespace WeatherMonitor.Core.Interfaces.Azure
{
    public interface IWeatherPayloadRepository
    {
        Task StorePayloadAsync(string blobId, string payload);
        Task<string?> GetPayloadAsync(string blobId);
    }
}
