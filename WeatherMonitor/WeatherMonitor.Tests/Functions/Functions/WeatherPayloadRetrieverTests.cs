using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using WeatherMonitor.Core.Interfaces.Azure;
using WeatherMonitor.Core.Models.Azure;
using WeatherMonitor.Functions.Functions;
using Xunit;

namespace WeatherMonitor.Tests.Functions.Functions
{
    public class WeatherPayloadRetrieverTests
    {
        private readonly Mock<IWeatherLogRepository> _mockLogRepository;
        private readonly Mock<IWeatherPayloadRepository> _mockPayloadRepository;
        private readonly Mock<ILogger> _mockLogger;
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly WeatherPayloadRetriever _function;

        public WeatherPayloadRetrieverTests()
        {
            _mockLogRepository = new Mock<IWeatherLogRepository>();
            _mockPayloadRepository = new Mock<IWeatherPayloadRepository>();
            _mockLogger = new Mock<ILogger>();
            _mockLoggerFactory = new Mock<ILoggerFactory>();

            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(_mockLogger.Object);

            _function = new WeatherPayloadRetriever(
                _mockLogRepository.Object,
                _mockPayloadRepository.Object,
                _mockLoggerFactory.Object
            );
        }

        [Fact]
        public async Task Run_WithValidIds_ReturnsPayload()
        {
            // Arrange
            var context = new Mock<FunctionContext>();

            string partitionKey = "20250401";
            string rowKey = "123456-abcdef";
            string blobId = "123456-abcdef";
            string expectedPayload = "{\"weather\":[{\"description\":\"clear sky\"}]}";

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri($"http://localhost/api/GetWeatherPayload?partitionKey={partitionKey}&rowKey={rowKey}");
            request.Setup(r => r.Url).Returns(uri);

            var log = new WeatherLog("London", true)
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                BlobId = blobId
            };

            _mockLogRepository.Setup(repo => repo.GetLogByIdAsync(partitionKey, rowKey))
                .ReturnsAsync(log);

            _mockPayloadRepository.Setup(repo => repo.GetPayloadAsync(blobId))
                .ReturnsAsync(expectedPayload);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.OK);
            response.SetupProperty(r => r.Headers, new HttpHeadersCollection());
            response.Setup(r => r.Body).Returns(new MemoryStream());
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task Run_WithMissingQueryParams_ReturnsBadRequest()
        {
            // Arrange
            var context = new Mock<FunctionContext>();

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri("http://localhost/api/GetWeatherPayload");
            request.Setup(r => r.Url).Returns(uri);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.BadRequest);
            response.Setup(r => r.Body).Returns(new MemoryStream());
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task Run_WithNonExistentLog_ReturnsNotFound()
        {
            // Arrange
            var context = new Mock<FunctionContext>();

            string partitionKey = "20250401";
            string rowKey = "123456-abcdef";

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri($"http://localhost/api/GetWeatherPayload?partitionKey={partitionKey}&rowKey={rowKey}");
            request.Setup(r => r.Url).Returns(uri);

            _mockLogRepository.Setup(repo => repo.GetLogByIdAsync(partitionKey, rowKey))
                .ReturnsAsync((WeatherLog)null);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.NotFound);
            response.Setup(r => r.Body).Returns(new MemoryStream());
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }

        [Fact]
        public async Task Run_WithMissingPayload_ReturnsNotFound()
        {
            // Arrange
            var context = new Mock<FunctionContext>();

            string partitionKey = "20250401";
            string rowKey = "123456-abcdef";
            string blobId = "123456-abcdef";

            var request = new Mock<HttpRequestData>(context.Object);
            var response = new Mock<HttpResponseData>(context.Object);

            var uri = new Uri($"http://localhost/api/GetWeatherPayload?partitionKey={partitionKey}&rowKey={rowKey}");
            request.Setup(r => r.Url).Returns(uri);

            var log = new WeatherLog("London", true)
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                BlobId = blobId
            };

            _mockLogRepository.Setup(repo => repo.GetLogByIdAsync(partitionKey, rowKey))
                .ReturnsAsync(log);

            _mockPayloadRepository.Setup(repo => repo.GetPayloadAsync(blobId))
                .ReturnsAsync((string)null);

            response.SetupProperty(r => r.StatusCode, HttpStatusCode.NotFound);
            response.Setup(r => r.Body).Returns(new MemoryStream());
            request.Setup(r => r.CreateResponse()).Returns(response.Object);

            // Act
            var result = await _function.Run(request.Object);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
        }
    }
}
