using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherForecastAggregator.Analytics;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Parser;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        [HttpGet("{forecasterId}/{cityId}/{daysAgo}")]
        async public Task<string> Index(int forecasterId, int cityId, int daysAgo)
        {
            // TODO Delete next line
            // if (forecasterId == 1) return JsonConvert.SerializeObject("");

            Forecaster? forecaster = Forecasters.GetById(forecasterId);
            City? city = Cities.GetById(cityId);
            if (forecaster is null || city is null)
                return JsonConvert.SerializeObject("");

            DateOnly day = DateOnly.FromDateTime(DateTime.Now.AddDays(-daysAgo));

            // return JsonConvert.SerializeObject(WeatherDays.GetWeatherHours(forecaster.Id, city.Id, day));

            Dictionary<DateTime, Dictionary<int, int>> diviations = Analyst.GetDeviations(forecaster, city, day);
            Dictionary<int, int> biggestDiviation = Analyst.GetBiggestDeviation(forecaster, city, day);

            object[] res = { diviations, biggestDiviation };
            return JsonConvert.SerializeObject(res);
        }
    }
}
