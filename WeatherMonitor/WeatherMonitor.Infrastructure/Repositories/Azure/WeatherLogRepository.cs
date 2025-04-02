using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;

namespace WeatherMonitor.Infrastructure.Repositories.Azure
{
    public class WeatherLogRepository : IWeatherLogRepository
    {
        private readonly TableClient _tableClient;

        public WeatherLogRepository(IConfiguration configuration)
        {
            var connectionString = configuration["AzureWebJobsStorage"];
            var tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = tableServiceClient.GetTableClient("WeatherLogs");
            _tableClient.CreateIfNotExists();
        }

        public async Task AddLogAsync(WeatherLog log)
        {
            await _tableClient.AddEntityAsync(log);
        }

        public async Task<IEnumerable<WeatherLog>> GetLogsByTimeRangeAsync(DateTime startTime, DateTime endTime)
        {
            startTime = startTime.ToUniversalTime();
            endTime = endTime.ToUniversalTime();

            var partitionKeys = new List<string>();
            for (var day = startTime.Date; day <= endTime.Date; day = day.AddDays(1))
            {
                partitionKeys.Add(day.ToString("yyyyMMdd"));
            }

            var results = new List<WeatherLog>();
            foreach (var partitionKey in partitionKeys)
            {
                var query = _tableClient.QueryAsync<WeatherLog>(filter: $"PartitionKey eq '{partitionKey}'");

                await foreach (var log in query)
                {
                    if (log.Timestamp >= startTime && log.Timestamp <= endTime)
                    {
                        results.Add(log);
                    }
                }
            }

            return results.OrderByDescending(l => l.Timestamp);
        }

        public async Task<WeatherLog?> GetLogByIdAsync(string partitionKey, string rowKey)
        {
            try
            {
                var response = await _tableClient.GetEntityAsync<WeatherLog>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}
