using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Primitives;
using System.Diagnostics;
using System.IO;
using System.Text;
using WeatherForecastAggregator.Bot;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Parser;
using WeatherForecastAggregator.View;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Data and Settings
List<City> citiesWBot = Cities.GetCities();
List<City> citiesWeb = Cities.GetCities(2);

List<string> baseLinksBot = new List<string> { BaseLinks.Link1, BaseLinks.Link2, BaseLinks.Link3 };
List<string> baseLinksWeb = new List<string> { BaseLinks.Link1, BaseLinks.Link2, BaseLinks.Link3 };



// Bot
Bot bot = new Bot(citiesWBot, baseLinksBot);
bot.Run();

// Web
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run(async (context) =>
{
    
    StringBuilder stringBuilder = new();
    stringBuilder.Append(await GeneralView.GetHtmlAPI(citiesWeb, baseLinksWeb));


    var response = context.Response;
    response.Headers.ContentLanguage = "ru-RU";
    response.Headers.ContentType = "text/html; charset=utf-8";
    await response.WriteAsync(stringBuilder.ToString());
});

app.Run();
Console.ReadLine();
