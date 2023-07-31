using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Parser
{
    public interface IParser
    {
        public const int respondDelay = 5000;
        WeatherDay GetWeatherDay(City city);
        Task<WeatherDay?> GetWeatherDayAPI(City city);
    }
}
