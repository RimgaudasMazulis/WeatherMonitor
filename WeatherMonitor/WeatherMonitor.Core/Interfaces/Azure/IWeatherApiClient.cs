﻿using WeatherMonitor.Core.Models;

namespace WeatherMonitor.Core.Interfaces.Azure
{
    public interface IWeatherApiClient
    {
        Task<WeatherApiResponse> GetWeatherDataAsync(string city, string country);
        Task<string> GetWeatherDataAsStringAsync(string city);
    }
}
