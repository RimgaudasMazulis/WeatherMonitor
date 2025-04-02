import React, { useEffect, useState } from 'react';
import { WeatherRecord } from './types/weather';
import { weatherService } from './services/api';
import WeatherTable from './componenets/WeatherTable/WeatherTable';
import TemperatureChart from './componenets/TemperatureChart/TemperatureChart';

const App: React.FC = () => {
    const [records, setRecords] = useState<WeatherRecord[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchWeatherData = async () => {
        try {
            const data = await weatherService.getAllWeatherRecords();
            setRecords(data);
            setLoading(false);
        } catch (err) {
            setError('Failed to fetch weather data');
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchWeatherData();

        // Set up polling every minute
        const intervalId = setInterval(() => {
            fetchWeatherData();
        }, 60000);

        return () => clearInterval(intervalId);
    }, []);

    if (loading) {
        return <div className="text-center p-4">Loading weather data...</div>;
    }

    if (error) {
        return <div className="text-center p-4 text-red-500">{error}</div>;
    }

    return (
        <div className="container mx-auto p-4">
            <h1 className="text-2xl font-bold mb-4">Weather Monitoring Dashboard</h1>
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-2">Current Weather Data</h2>
                <WeatherTable records={records} />
            </div>
            <div className="mb-8">
                <h2 className="text-xl font-semibold mb-2">Temperature Comparison</h2>
                <div className="h-96">
                    <TemperatureChart records={records} />
                </div>
            </div>
            <div className="text-sm text-gray-500">
                Last updated: {new Date().toLocaleString()}
            </div>
        </div>
    );
};

export default App;