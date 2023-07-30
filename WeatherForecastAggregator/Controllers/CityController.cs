using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        [HttpGet("{id}")]
        public string Index(int id)
        {
            City? city = Cities.GetById(id);
            //if (city is null)
            //    return $"there is no city with {id} id";
            string allCitiesStr = JsonConvert.SerializeObject(city);
            return allCitiesStr;
        }
    }
}
