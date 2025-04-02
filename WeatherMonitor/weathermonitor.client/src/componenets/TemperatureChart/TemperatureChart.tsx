import React from 'react';
import { Bar } from 'react-chartjs-2';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
} from 'chart.js';
import { WeatherRecord } from '../../types/weather';

ChartJS.register(
    CategoryScale,
    LinearScale,
    BarElement,
    Title,
    Tooltip,
    Legend
);

interface TemperatureChartProps {
    records: WeatherRecord[];
}

const TemperatureChart: React.FC<TemperatureChartProps> = ({ records }) => {
    const chartData = {
        labels: records.map(record => `${record.city}, ${record.country}`),
        datasets: [
            {
                label: 'Current Temp (°C)',
                data: records.map(record => record.temperature),
                backgroundColor: 'rgba(53, 162, 235, 0.5)',
            },
            {
                label: 'Min Temp (°C)',
                data: records.map(record => record.minTemperature),
                backgroundColor: 'rgba(75, 192, 192, 0.5)',
            },
            {
                label: 'Max Temp (°C)',
                data: records.map(record => record.maxTemperature),
                backgroundColor: 'rgba(255, 99, 132, 0.5)',
            },
        ],
    };

    const options = {
        responsive: true,
        plugins: {
            legend: {
                position: 'top' as const,
            },
            title: {
                display: true,
                text: 'Temperature Comparison by City',
            },
        },
    };

    return <Bar data={chartData} options={options} />;
};

export default TemperatureChart;