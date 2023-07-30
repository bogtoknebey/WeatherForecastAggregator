namespace WeatherForecastAggregator.Models
{
    public class WeatherDay
    {
        public int Id { get; set; }
        public List<WeatherHour> WeatherHours { get; set; } = new();
        public City City { get; set; }
        public Forecaster Forecaster { get; set; }
        public DateOnly Date { get; set; }
        public DateTime ForecastDate { get; set; }

        public WeatherDay() { }
        public WeatherDay(List<WeatherHour> weatherHours, City city, Forecaster forecaster, DateOnly date, DateTime forecastDate)
        {
            WeatherHours = weatherHours;
            City = city;
            Forecaster = forecaster;
            Date = date;
            ForecastDate = forecastDate;
        }
    }
}
