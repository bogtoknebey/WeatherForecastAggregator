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

// Data and Settings
List<City> allCities = Cities.GetAll();
List<Forecaster> allForecasters = Forecasters.GetAll();


List<City> citiesBot = new List<City>() { allCities[0], allCities[1], allCities[2], allCities[3], allCities[4] };
List<City> citiesWeb = new List<City>() { allCities[0], allCities[1] };
List<City> citiesMonitoring = allCities;

List<Forecaster> forecastersBot = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };
List<Forecaster> forecastersWeb = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };
List<Forecaster> forecastersMonitoring = new List<Forecaster>() { allForecasters[0], allForecasters[1], allForecasters[2] };

// Monitoring
WeatherForecastAggregator.Monitor.Monitor monitor = new WeatherForecastAggregator.Monitor.Monitor(
    citiesMonitoring,
    forecastersMonitoring,
    3
);
monitor.Acitivate();

//Bot
Bot bot = new Bot(citiesBot, forecastersBot);
bot.Run();

// Web
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.UseCors(policy => policy.AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials());
// app.UseCors(options => options.AllowAnyOrigin());
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//string s = await GeneralView.GetHtmlAPI(citiesWeb, forecastersWeb);
//app.Run(async (context) =>
//{
//    StringBuilder stringBuilder = new();
//    stringBuilder.Append(await GeneralView.GetHtmlAPI(citiesWeb, forecastersWeb));

//    var response = context.Response;
//    response.Headers.ContentLanguage = "ru-RU";
//    response.Headers.ContentType = "text/html; charset=utf-8";
//    await response.WriteAsync(stringBuilder.ToString());
//});


app.Run();
Console.ReadLine();
