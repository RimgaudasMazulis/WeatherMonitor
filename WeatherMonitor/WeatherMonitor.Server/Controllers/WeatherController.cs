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

        [HttpGet("{city}")]
        public async Task<ActionResult<WeatherRecord>> GetWeatherByCity(string city)
        {
            var record = await _weatherService.GetWeatherRecordByCityAsync(city);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }
    }
}
