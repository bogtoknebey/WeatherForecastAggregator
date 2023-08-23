using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.DTO;
using System.Timers;
using WeatherForecastAggregator.Parser;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WeatherForecastAggregator.Monitor
{
    public class Monitor
    {
        private readonly ILogger<Monitor> _logger;

        private readonly bool _immediateOperation;
        public double lagHours { get; }
        public List<City> cities { get; set; }
        public List<Forecaster> forecasters { get; set; }
        

        public Monitor(List<City> cities, List<Forecaster> forecasters, double lagHours, bool immediateOperation, ILogger<Monitor> logger) 
        {
            this.cities = cities;
            this.forecasters = forecasters;
            this.lagHours = lagHours;
            _immediateOperation = immediateOperation;
            _logger = logger;
        }


        private void AddWeatherDaysHandle(object source, ElapsedEventArgs e)
        {
            _logger.LogInformation("-------Handler Function Start-------");
            Connector conn = new Connector();
            foreach (Forecaster f in forecasters)
            {
                _logger.LogInformation("-----Forecaster iteration-----");
                IParser parser = SelectParser.GetParser(f);
                foreach (City c in cities)
                {
                    // _logger.LogInformation("---City iteration---");
                    try
                    {
                        WeatherDay? weatherDay = Task.Run(() => parser.GetWeatherDayAPI(c)).Result;
                        if (weatherDay is not null)
                        {
                            //Console.ForegroundColor = ConsoleColor.Green;
                            //Console.WriteLine($"{f.Name}, {c.UkrainianTranslit}: That`s one is ok!, 00 temperature: {weatherDay.WeatherHours[0].Temperature}");
                            //Console.ResetColor();
                            _logger.LogInformation($"Add data in DB by: {f.Name}, {c.UkrainianTranslit}. Zero temperature: {weatherDay.WeatherHours[0].Temperature}");
                            WeatherDays.Add(weatherDay, conn);
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            _logger.LogInformation($"NO DATA ABOUT: {f.Name}, {c.UkrainianTranslit}.");
                            //Console.ForegroundColor = ConsoleColor.Red;
                            //Console.WriteLine($"{f.Name}, {c.UkrainianTranslit}: That`s one is NOT ok!");
                            //Console.ResetColor();
                        }
                    } 
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            _logger.LogInformation($"\n\n");
            //Console.WriteLine();
            //Console.WriteLine();
            conn.CloseConnection();
            _logger.LogInformation($"-------Handler Function Finish-------\n\n");
        }


        async public void Acitivate()
        {
            _logger.LogInformation("----------Monitor Activation----------");
            if (_immediateOperation)
            {
                System.Timers.Timer firstTimer = new System.Timers.Timer();
                firstTimer.Elapsed += new ElapsedEventHandler(AddWeatherDaysHandle);
                firstTimer.AutoReset = false;
                firstTimer.Interval = 1000;
                firstTimer.Enabled = true;
                _logger.LogInformation("-------FIRST TIMER HAS STARTED-------");
            }

            // Console.WriteLine("Activate method is starting it`s work...");
            int lag = Convert.ToInt32(lagHours * 60 * 60 * 1000);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += new ElapsedEventHandler(AddWeatherDaysHandle);
            timer.Interval = lag;
            timer.Enabled = true;
            _logger.LogInformation("-------MAIN TIMER HAS STARTED-------");
        }
    }
}
