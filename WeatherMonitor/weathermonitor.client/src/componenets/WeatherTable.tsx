import React from 'react';
import { WeatherRecord } from '../types/weather';

interface WeatherTableProps {
    records: WeatherRecord[];
}

const WeatherTable: React.FC<WeatherTableProps> = ({ records }) => {
    return (
        <div className= "overflow-x-auto" >
        <table className="min-w-full bg-white border border-gray-200" >
            <thead>
            <tr>
            <th className="px-4 py-2 border-b" > Country </th>
                < th className = "px-4 py-2 border-b" > City </th>
                    < th className = "px-4 py-2 border-b" > Current Temp(°C) </th>
                        < th className = "px-4 py-2 border-b" > Min Temp(°C) </th>
                            < th className = "px-4 py-2 border-b" > Max Temp(°C) </th>
                                < th className = "px-4 py-2 border-b" > Last Updated </th>
                                    </tr>
                                    </thead>
                                    <tbody>
    {
        records.map((record) => (
            <tr key= {`${record.country}-${record.city}`}>
                <td className="px-4 py-2 border-b" > { record.country } </td>
                    < td className = "px-4 py-2 border-b" > { record.city } </td>
                        < td className = "px-4 py-2 border-b" > { record.temperature.toFixed(1) } </td>
                            < td className = "px-4 py-2 border-b" > { record.minTemperature.toFixed(1) } </td>
                                < td className = "px-4 py-2 border-b" > { record.maxTemperature.toFixed(1) } </td>
                                    < td className = "px-4 py-2 border-b" >
                                        { new Date(record.lastUpdated).toLocaleString() }
                                        </td>
                                        </tr>
          ))}
</tbody>
    </table>
    </div>
  );
};

export default WeatherTable;