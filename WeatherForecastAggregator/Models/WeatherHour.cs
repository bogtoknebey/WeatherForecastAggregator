namespace WeatherForecastAggregator.Models
{
    public class WeatherHour
    {
        public int Id { get; set; }
        public int Temperature { get; set; }
        public int Hour { get; set; }

        public WeatherHour() { }
        public WeatherHour(int temperature, int hour)
        {
            Temperature = temperature;
            Hour = hour;
        }
    }
}
