using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using WeatherMonitor.Infrastructure.Repositories.Azure;
using Xunit;

namespace WeatherMonitor.Tests.Infrastructure.Repositories.Azure
{
    public class WeatherPayloadRepositoryTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<BlobClient> _mockBlobClient;
        private readonly Mock<BlobContainerClient> _mockContainerClient;
        private readonly WeatherPayloadRepository _repository;

        public WeatherPayloadRepositoryTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockBlobClient = new Mock<BlobClient>(MockBehavior.Loose, new Uri("https://test.com"), null);
            _mockContainerClient = new Mock<BlobContainerClient>(MockBehavior.Loose, new Uri("https://test.com"), null);

            // Setup configuration
            _mockConfiguration.Setup(c => c["AzureWebJobsStorage"]).Returns("UseDevelopmentStorage=true");

            // Create a factory that allows testing the repository without actual Azure Blob Storage
            _repository = new WeatherPayloadRepositoryForTest(
                _mockConfiguration.Object,
                _mockContainerClient.Object
            );
        }

        [Fact]
        public async Task StorePayloadAsync_UploadsToBlob()
        {
            // Arrange
            string blobId = "test-blob-id";
            string payload = "{\"weather\":[{\"description\":\"clear sky\"}]}";

            _mockContainerClient.Setup(client => client.GetBlobClient(blobId))
                .Returns(_mockBlobClient.Object);

            var mockResponseValue = new Mock<Response<BlobContentInfo>>();
            _mockBlobClient.Setup(client => client.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.FromResult(mockResponseValue.Object));

            // Act
            await _repository.StorePayloadAsync(blobId, payload);

            // Assert
            _mockContainerClient.Verify(client => client.GetBlobClient(blobId), Times.Once);
            _mockBlobClient.Verify(client => client.UploadAsync(
                It.IsAny<Stream>(),
                true,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetPayloadAsync_WhenNotExists_ReturnsNull()
        {
            // Arrange
            string blobId = "test-blob-id";

            _mockContainerClient.Setup(client => client.GetBlobClient(blobId))
                .Returns(_mockBlobClient.Object);

            _mockBlobClient.Setup(client => client.ExistsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Response.FromValue(false, new Mock<Response>().Object));

            // Act
            var result = await _repository.GetPayloadAsync(blobId);

            // Assert
            Assert.Null(result);
        }

        // This is a testable version of WeatherPayloadRepository that allows injecting a mock BlobContainerClient
        public class WeatherPayloadRepositoryForTest : WeatherPayloadRepository
        {
            private readonly BlobContainerClient _containerClient;

            public WeatherPayloadRepositoryForTest(IConfiguration configuration, BlobContainerClient containerClient)
                : base(configuration)
            {
                _containerClient = containerClient;
            }

            protected override BlobContainerClient GetBlobContainerClient()
            {
                return _containerClient;
            }
        }
    }
}