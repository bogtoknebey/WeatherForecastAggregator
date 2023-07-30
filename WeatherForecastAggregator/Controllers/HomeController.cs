using Microsoft.AspNetCore.Mvc;
using System.Text;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.View;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherForecastAggregator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        // GET: api/<HomeController>
        [HttpGet]
        async public Task<string> Index()
        {
            List<City> allCities = Cities.GetAll();
            List<Forecaster> allForcasters = Forecasters.GetAll();
            List<City> citiesWeb = new List<City>() { allCities[0], allCities[1] };
            List<Forecaster> forecastersWeb = new List<Forecaster>() { allForcasters[0], allForcasters[2] };

            StringBuilder stringBuilder = new();
            stringBuilder.Append(await GeneralView.GetHtmlAPI(citiesWeb, forecastersWeb));

            return stringBuilder.ToString();
        }
    }
}
