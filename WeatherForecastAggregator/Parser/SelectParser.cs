using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Parser
{
    public static class SelectParser
    {
        public static IParser GetParser(string baseLink)
        {
            if (baseLink == "https://sinoptik.ua/")
            {
                return new Parser1();
            }
            if (baseLink == "https://www.gismeteo.ua/")
            {
                return new Parser2();
            }
            if (baseLink == "https://meteo.ua/")
            {
                return new Parser3();
            }
            return new Parser1();
        } 
    }
}
