using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using WeatherMonitor.Core.Interfaces.Azure;

namespace WeatherMonitor.Functions.Functions
{
    public class WeatherPayloadRetriever
    {
        private readonly IWeatherLogRepository _logRepository;
        private readonly IWeatherPayloadRepository _payloadRepository;
        private readonly ILogger _logger;

        public WeatherPayloadRetriever(
            IWeatherLogRepository logRepository,
            IWeatherPayloadRepository payloadRepository,
            ILoggerFactory loggerFactory)
        {
            _logRepository = logRepository;
            _payloadRepository = payloadRepository;
            _logger = loggerFactory.CreateLogger<WeatherPayloadRetriever>();
        }

        [Function("GetWeatherPayload")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("GetWeatherPayload function processed a request");

            // Parse query parameters
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            var partitionKey = queryParams["partitionKey"];
            var rowKey = queryParams["rowKey"];

            if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please provide 'partitionKey' and 'rowKey' parameters");
                return badResponse;
            }

            // Get the log entry to find the blob ID
            var log = await _logRepository.GetLogByIdAsync(partitionKey, rowKey);
            if (log == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Log entry not found");
                return notFoundResponse;
            }

            // Get the payload from blob storage
            var payload = await _payloadRepository.GetPayloadAsync(log.BlobId);
            if (payload == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync("Payload not found in blob storage");
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            await response.WriteStringAsync(payload);

            return response;
        }
    }
}
