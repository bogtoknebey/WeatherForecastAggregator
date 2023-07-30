using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastersController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            List<Forecaster> allForcasters = Forecasters.GetAll();
            string allForcastersStr = JsonConvert.SerializeObject(allForcasters);
            return allForcastersStr;
        }
    }
}
