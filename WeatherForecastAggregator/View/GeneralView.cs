using System.Text;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Parser;

namespace WeatherForecastAggregator.View
{
    public class GeneralView
    {

        public static string GetHtml(List<City> cities, List<Forecaster> forecasters)
        {
            StringBuilder stringBuilder = new();
            foreach (var city in cities)
            {
                foreach (var f in forecasters)
                {
                    stringBuilder.Append("<div>" + f.BaseLink + "</div>");
                    IParser parser = SelectParser.GetParser(f);
                    WeatherDay weatherDay = parser.GetWeatherDay(city);
                    stringBuilder.Append(WeatherDayView.GetHtml(weatherDay));
                }
            }
            return stringBuilder.ToString();
        }


        async public static Task<string> GetHtmlAPI(List<City> cities, List<Forecaster> forecasters)
        {
            StringBuilder stringBuilder = new("<div>API</div>");
            foreach (var city in cities)
            {
                foreach (var f in forecasters)
                {
                    stringBuilder.Append("<div>" + f.BaseLink + "</div>");
                    IParser parser = SelectParser.GetParser(f);
                    WeatherDay weatherDay = await parser.GetWeatherDayAPI(city);
                    stringBuilder.Append(WeatherDayView.GetHtml(weatherDay));
                }
            }
            return stringBuilder.ToString();
        }
    }
}
