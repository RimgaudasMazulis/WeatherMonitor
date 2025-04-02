using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherMonitor.Core.Interfaces.Azure;

namespace WeatherMonitor.Functions.Functions
{
    public class WeatherLogRetriever
    {
        private readonly IWeatherLogRepository _logRepository;
        private readonly ILogger _logger;

        public WeatherLogRetriever(IWeatherLogRepository logRepository, ILoggerFactory loggerFactory)
        {
            _logRepository = logRepository;
            _logger = loggerFactory.CreateLogger<WeatherLogRetriever>();
        }

        [Function("GetWeatherLogs")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("GetWeatherLogs function processed a request");

            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            if (!DateTime.TryParse(queryParams["from"], out var fromDate) ||
                !DateTime.TryParse(queryParams["to"], out var toDate))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Please provide valid 'from' and 'to' date parameters");
                return badResponse;
            }

            var logs = await _logRepository.GetLogsByTimeRangeAsync(fromDate, toDate);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            await response.WriteStringAsync(JsonSerializer.Serialize(logs, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            }));

            return response;
        }
    }
}
