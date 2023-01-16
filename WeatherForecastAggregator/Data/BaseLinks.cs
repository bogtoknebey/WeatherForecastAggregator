using System.Text;
using WeatherForecastAggregator.Models;
using static System.Net.WebRequestMethods;

namespace WeatherForecastAggregator.Data
{
    public class BaseLinks
    {
        public static string Link1 { get; } = "https://sinoptik.ua/";
        public static string Link2 { get; } = "https://www.gismeteo.ua/";
        public static string Link3 { get; } = "https://meteo.ua/";
    }
}
