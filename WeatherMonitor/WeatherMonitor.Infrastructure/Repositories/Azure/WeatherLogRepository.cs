using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;

namespace WeatherMonitor.Infrastructure.Repositories.Azure
{
    public class WeatherLogRepository : IWeatherLogRepository
    {
        private readonly IConfiguration _configuration;
        private TableClient? _tableClient;

        public WeatherLogRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected virtual TableClient GetTableClient()
        {
            if (_tableClient == null)
            {
                var connectionString = _configuration["AzureWebJobsStorage"];
                var tableServiceClient = new TableServiceClient(connectionString);
                _tableClient = tableServiceClient.GetTableClient("WeatherLogs");
                _tableClient.CreateIfNotExists();
            }
            return _tableClient;
        }

        public async Task AddLogAsync(WeatherLog log)
        {
            await GetTableClient().AddEntityAsync(log);
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
                var query = GetTableClient().QueryAsync<WeatherLog>(filter: $"PartitionKey eq '{partitionKey}'");

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
                var response = await GetTableClient().GetEntityAsync<WeatherLog>(partitionKey, rowKey);
                return response.Value;
            }
            catch (RequestFailedException)
            {
                return null;
            }
        }
    }
}
