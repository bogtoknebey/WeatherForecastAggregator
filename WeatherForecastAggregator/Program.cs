using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.IO;
using System.Text;
using WeatherForecastAggregator.Analytics;
using WeatherForecastAggregator.Bot;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Monitor;
using WeatherForecastAggregator.Parser;
using WeatherForecastAggregator.View;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NLog.Extensions.Logging;
using NLog.Web;
using Microsoft.Extensions.Hosting;


// Data and Settings
List<City> allCities = Cities.GetAll();
List<Forecaster> allForecasters = Forecasters.GetAll();

List<City> citiesBot = new List<City>() { allCities[0], allCities[1], allCities[2], allCities[3], allCities[4] };
List<City> citiesWeb = new List<City>() { allCities[0], allCities[1] };
List<City> citiesMonitoring = allCities;
List<City> testCitiesMonitoring = allCities;

List<Forecaster> forecastersBot = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };
List<Forecaster> forecastersWeb = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };
List<Forecaster> forecastersMonitoring = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };

//Bot
Bot bot = new Bot(citiesBot, forecastersBot);
bot.Run();

// Web
var builder = WebApplication.CreateBuilder(args);

// Add NLog configuration
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddNLog();

builder.Services.AddControllers();

// Monitoring
double testLagHours = 5.0 / 60.0;
double prodLagHours = 3;

var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<WeatherForecastAggregator.Monitor.Monitor>();
bool immediateOperation = true;

WeatherForecastAggregator.Monitor.Monitor monitor = new WeatherForecastAggregator.Monitor.Monitor(
    citiesMonitoring,
    forecastersMonitoring,
    prodLagHours,
    immediateOperation,
    logger
);
monitor.Acitivate();

// App settings
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
Console.ReadLine();


