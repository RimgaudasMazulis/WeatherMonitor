import React, { useEffect, useState } from 'react';
import { WeatherRecord } from './types/weather';
import { weatherService } from './services/api';
import './App.css';
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
        return <div className="loading">Loading weather data...</div>;
    }

    if (error) {
        return <div className="error">{error}</div>;
    }

    return (
        <div className="app-container">
            <h1 className="app-title">Weather Monitoring Dashboard</h1>
            <div className="section">
                <h2 className="section-title">Current Weather Data</h2>
                <WeatherTable records={records} />
            </div>
            <div className="section">
                <h2 className="section-title">Temperature Comparison</h2>
                <div className="chart-container">
                    <TemperatureChart records={records} />
                </div>
            </div>
            <div className="last-updated">
                Last updated: {new Date().toLocaleString()}
            </div>
        </div>
    );
};

export default App;
