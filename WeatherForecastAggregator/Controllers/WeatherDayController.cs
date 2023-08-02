using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Parser;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherDayController : ControllerBase
    {
        [HttpGet("{forecasterId}/{cityId}")]
        async public Task<string> Index(int forecasterId, int cityId)
        {
            // TODO Delete next line
            
            // if (forecasterId == 1) return JsonConvert.SerializeObject("");

            Forecaster? forecaster = Forecasters.GetById(forecasterId);
            City? city = Cities.GetById(cityId);
            if (forecaster is null || city is null)
                return JsonConvert.SerializeObject("");

            IParser parser = SelectParser.GetParser(forecaster);
            WeatherDay? weatherDay = await parser.GetWeatherDayAPI(city);
            if (weatherDay is null)
                return JsonConvert.SerializeObject("");

            return JsonConvert.SerializeObject(weatherDay);
        }
    }
}
