# WeatherMonitor

Make sure you are using correct ApplicationUrl between Frontend and backend, for development purposes I was using "https" profile.

Don't forget to create database before starting up solution. (Use command Update-Database in nuget package manager console to create DB and apply migrations)

Prerequisites: 
    - Need to have Azure Function Tools installed (https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp)
    - Azurite (npm install -g azurite)

Functions:
    - FetchWeatherData - runs every minute to fetch weather data from https://api.openweathermap.org
    - GetWeatherLogs - example HTTP call: http://localhost:7004/api/GetWeatherLogs?from=2025-03-01&to=2025-04-03
    - GetWeatherPayload - example HTTP call: http://localhost:7004/api/GetWeatherPayload?partitionKey=20250402&rowKey=094804-eace8786

Potential improvements: 
    Use Automapper to avoid mapping each entity to a similar one
    Better exception handling solution wide
    Creating a class for returning configuration values all at once
    More Unit Tests
    Tests could be split into different projects to mimic each project in the solution
    Could add cancelation tokens on all requests that are going into the database
    Notracking on db entities
    Move cities into configuration and read it from there

Task1:
    Run WeatherMonitor.Functions project
Task2:
    Setup multiple startup projects in Visual studio: weathermonitor.client for react and WeatherMonitor.Server for backend, run them both