using Azure;
using Azure.Data.Tables;

namespace WeatherMonitor.Core.Models.Azure
{
    public class WeatherLog : ITableEntity
    {
        public string PartitionKey { get; set; } = default!;
        public string RowKey { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public bool IsSuccess { get; set; }
        public string City { get; set; } = default!;
        public string? ErrorMessage { get; set; }
        public string BlobId { get; set; } = default!;

        public WeatherLog() { }

        public WeatherLog(string city, bool isSuccess, string? errorMessage = null)
        {
            PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd");
            RowKey = $"{DateTime.UtcNow:HHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            City = city;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            BlobId = RowKey;
        }
    }
}
