using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Parser
{
    public interface IParser
    {
        WeatherDay GetWeatherDay(City city);
        Task<WeatherDay> GetWeatherDayAPI(City city);
    }
}
