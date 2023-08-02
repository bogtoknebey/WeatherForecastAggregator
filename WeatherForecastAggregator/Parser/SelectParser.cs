using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Parser
{
    public static class SelectParser
    {
        public static IParser GetParser(Forecaster f)
        {
            if (f.Name == "sinoptik")
            {
                return new Parser1();
            }
            if (f.Name == "meteofor")
            {
                return new Parser2();
            }
            if (f.Name == "meteo")
            {
                return new Parser3();
            }
            return new Parser1();
        } 
    }
}
