using Microsoft.EntityFrameworkCore;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Infrastructure.Data;

namespace WeatherMonitor.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly ApplicationDbContext _context;

        public WeatherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WeatherRecord>> GetAllWeatherRecordsAsync()
        {
            return await _context.WeatherRecords.ToListAsync();
        }

        public async Task<IEnumerable<WeatherRecord>> GetWeatherRecordsByCountryAsync(string country)
        {
            return await _context.WeatherRecords
                .Where(w => w.Country == country)
                .ToListAsync();
        }

        public async Task<WeatherRecord> GetWeatherRecordByCityAsync(string city, string country)
        {
            return await _context.WeatherRecords
                .FirstOrDefaultAsync(w => w.City == city && w.Country == country);
        }

        public async Task<WeatherRecord> AddWeatherRecordAsync(WeatherRecord weatherRecord)
        {
            _context.WeatherRecords.Add(weatherRecord);
            await _context.SaveChangesAsync();
            return weatherRecord;
        }

        public async Task<WeatherRecord> UpdateWeatherRecordAsync(WeatherRecord weatherRecord)
        {
            var existingRecord = await _context.WeatherRecords
                .FirstOrDefaultAsync(w => w.City == weatherRecord.City && w.Country == weatherRecord.Country);

            if (existingRecord == null)
            {
                return await AddWeatherRecordAsync(weatherRecord);
            }

            existingRecord.Temperature = weatherRecord.Temperature;

            if (weatherRecord.Temperature < existingRecord.MinTemperature)
            {
                existingRecord.MinTemperature = weatherRecord.Temperature;
            }

            if (weatherRecord.Temperature > existingRecord.MaxTemperature)
            {
                existingRecord.MaxTemperature = weatherRecord.Temperature;
            }

            existingRecord.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingRecord;
        }

        public async Task<IEnumerable<WeatherRecord>> GetMinMaxTemperaturesByCityAsync(
            string city, string country, DateTime startDate, DateTime endDate)
        {
            return await _context.WeatherRecords
                .Where(w => w.City == city &&
                           w.Country == country &&
                           w.RecordedAt >= startDate &&
                           w.RecordedAt <= endDate)
                .ToListAsync();
        }
    }
}
