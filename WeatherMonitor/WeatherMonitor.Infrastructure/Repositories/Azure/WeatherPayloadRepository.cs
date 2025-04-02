using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Text;
using WeatherMonitor.Core.Interfaces.Azure;

namespace WeatherMonitor.Infrastructure.Repositories.Azure
{
    public class WeatherPayloadRepository : IWeatherPayloadRepository
    {
        private readonly IConfiguration _configuration;
        private BlobContainerClient? _containerClient;

        public WeatherPayloadRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected virtual BlobContainerClient GetBlobContainerClient()
        {
            if (_containerClient == null)
            {
                var connectionString = _configuration["AzureWebJobsStorage"];
                var blobServiceClient = new BlobServiceClient(connectionString);
                _containerClient = blobServiceClient.GetBlobContainerClient("weather-payloads");
                _containerClient.CreateIfNotExists();
            }
            return _containerClient;
        }

        public async Task StorePayloadAsync(string blobId, string payload)
        {
            var blobClient = GetBlobContainerClient().GetBlobClient(blobId);
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        public async Task<string?> GetPayloadAsync(string blobId)
        {
            var blobClient = GetBlobContainerClient().GetBlobClient(blobId);

            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var response = await blobClient.DownloadAsync();
            using var streamReader = new StreamReader(response.Value.Content);
            return await streamReader.ReadToEndAsync();
        }
    }
}
