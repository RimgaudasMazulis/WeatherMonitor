import { WeatherRecord } from '../types/weather';

const API_BASE_URL = 'https://localhost:7042/api';

export const weatherService = {
    async getAllWeatherRecords(): Promise<WeatherRecord[]> {
        const response = await fetch(`${API_BASE_URL}/weather`);
        if (!response.ok) {
            throw new Error('Failed to fetch weather records');
        }
        return response.json();
    },

    async getWeatherByCity(country: string, city: string): Promise<WeatherRecord> {
        const response = await fetch(`${API_BASE_URL}/weather/${country}/${city}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch weather for ${city}, ${country}`);
        }
        return response.json();
    },

    async getMinMaxTemperatures(
        country: string,
        city: string,
        startDate: string,
        endDate: string
    ): Promise<WeatherRecord[]> {
        const response = await fetch(
            `${API_BASE_URL}/weather/minmax?country=${country}&city=${city}&startDate=${startDate}&endDate=${endDate}`
        );
        if (!response.ok) {
            throw new Error(`Failed to fetch min/max temperatures for ${city}, ${country}`);
        }
        return response.json();
    }
};