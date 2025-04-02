using Microsoft.AspNetCore.Mvc;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherRecord>>> GetAllWeatherRecords()
        {
            var records = await _weatherService.GetAllWeatherRecordsAsync();
            return Ok(records);
        }

        [HttpGet("{country}/{city}")]
        public async Task<ActionResult<WeatherRecord>> GetWeatherByCity(string country, string city)
        {
            var record = await _weatherService.GetWeatherRecordByCityAsync(city, country);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }

        [HttpGet("minmax")]
        public async Task<ActionResult<IEnumerable<WeatherRecord>>> GetMinMaxTemperatures(
            [FromQuery] string country,
            [FromQuery] string city,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var records = await _weatherService.GetMinMaxTemperaturesByCityAsync(city, country, startDate, endDate);
            return Ok(records);
        }
    }
}
