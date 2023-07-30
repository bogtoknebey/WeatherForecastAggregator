using System.Text;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Parser;

namespace WeatherForecastAggregator.View
{
    public class WeatherDayView
    {
        public static string GetHtml(WeatherDay weatherDay)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append("<div>" + weatherDay.Date.ToString() + "</div>");
            stringBuilder.Append("<div>" + weatherDay.City.Russian + "</div>");
            stringBuilder.Append("<table bgcolor=\"#00FF00\">");
            stringBuilder.Append($"<tr bgcolor=\"#ffffff\"><td>Hour</td><td>Temperature</td></tr>");

            foreach (var weatherHour in weatherDay.WeatherHours)
            {
                stringBuilder.Append($"<tr bgcolor=\"#ffffff\"><td>{weatherHour.Hour}</td><td>{weatherHour.Temperature}</td></tr>");
            }
            stringBuilder.Append("</table>");
            stringBuilder.Append("<br><br>");

            return stringBuilder.ToString();
        }
    }
}
