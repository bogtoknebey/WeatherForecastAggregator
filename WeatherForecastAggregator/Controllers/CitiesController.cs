using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            List<City> allCities = Cities.GetAll();
            string allCitiesStr = JsonConvert.SerializeObject(allCities);
            return allCitiesStr;
        }
    }
}
