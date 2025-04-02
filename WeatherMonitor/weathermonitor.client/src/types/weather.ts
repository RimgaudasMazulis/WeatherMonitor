export interface WeatherRecord {
    id: number;
    country: string;
    city: string;
    temperature: number;
    minTemperature: number;
    maxTemperature: number;
    recordedAt: string;
    lastUpdated: string;
}