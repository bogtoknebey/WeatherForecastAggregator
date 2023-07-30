using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecasterController : ControllerBase
    {
        [HttpGet("{id}")]
        public string Index(int id)
        {
            Forecaster? forecaster = Forecasters.GetById(id);
            string forecasterStr = JsonConvert.SerializeObject(forecaster);
            return forecasterStr;
        }
    }
}
