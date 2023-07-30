using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.DTO;
using System.Timers;
using WeatherForecastAggregator.Parser;

namespace WeatherForecastAggregator.Monitor
{
    public class Monitor
    {
        public int lagHours { get; }
        public List<City> cities { get; set; }
        public List<Forecaster> forecasters { get; set; }

        public Monitor(List<City> cities, List<Forecaster> forecasters, int lagHours) 
        {
            this.cities = cities;
            this.forecasters = forecasters;
            this.lagHours = lagHours;
        }


        private void AddWeatherDaysHandle(object source, ElapsedEventArgs e)
        {
            Connector conn = new Connector();
            foreach (Forecaster f in forecasters)
            {
                IParser parser = SelectParser.GetParser(f);
                foreach (City c in cities)
                {
                    // WeatherDay weatherDay = await parser.GetWeatherDayAPI(c);
                    WeatherDay weatherDay = Task.Run(() => parser.GetWeatherDayAPI(c)).Result;
                    WeatherDays.Add(weatherDay, conn);
                    Thread.Sleep(1000);
                }
            }
            conn.CloseConnection();
        }


        async public void Acitivate()
        {
            int lag = lagHours * 60 * 60 * 1000;
            // int lag = 300 * 1000;

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(AddWeatherDaysHandle);
            timer.Interval = lag;
            timer.Enabled = true;
        }

        async public void testAdd(City city, Forecaster forecaster)
        {
            IParser parser = SelectParser.GetParser(forecaster);
            WeatherDay weatherDay = await parser.GetWeatherDayAPI(city);
            int i = 0;
            WeatherDays.Add(weatherDay);
        }
    }
}
