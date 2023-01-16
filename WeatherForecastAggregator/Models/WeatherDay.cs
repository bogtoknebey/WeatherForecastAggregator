namespace WeatherForecastAggregator.Models
{
    public class WeatherDay
    {
        public List<WeatherHour> WeatherHours { get; set; } = new();
        public City City { get; set; }
        public DateTime Day { get; set; }

    }
}
