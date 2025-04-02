import React from 'react';
import './WeatherTable.css'; // Importing the CSS file
import { WeatherRecord } from '../../types/weather';

interface WeatherTableProps {
    records: WeatherRecord[];
}

const WeatherTable: React.FC<WeatherTableProps> = ({ records }) => {
    return (
        <div className="table-container">
            <table className="weather-table">
                <thead>
                    <tr>
                        <th>Country</th>
                        <th>City</th>
                        <th>Current Temp (°C)</th>
                        <th>Min Temp (°C)</th>
                        <th>Max Temp (°C)</th>
                        <th>Last Updated</th>
                    </tr>
                </thead>
                <tbody>
                    {records.map((record) => (
                        <tr key={`${record.country}-${record.city}`}>
                            <td>{record.country}</td>
                            <td>{record.city}</td>
                            <td>{record.temperature.toFixed(1)}</td>
                            <td>{record.minTemperature.toFixed(1)}</td>
                            <td>{record.maxTemperature.toFixed(1)}</td>
                            <td>{new Date(record.lastUpdated).toLocaleString()}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default WeatherTable;
