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

    async getWeatherByCity(city: string): Promise<WeatherRecord> {
        const response = await fetch(`${API_BASE_URL}/weather/${city}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch weather for ${city}`);
        }
        return response.json();
    }
};