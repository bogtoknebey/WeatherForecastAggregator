namespace WeatherForecastAggregator.Models
{
    public class Forecaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BaseLink { get; set; }

        public Forecaster() { }
        public Forecaster(string name, string baseLink)
        {
            Name = name;
            BaseLink = baseLink;
        }
        public Forecaster(int id, string name, string baseLink)
        {
            Id = id;
            Name = name;
            BaseLink = baseLink;
        }
    }
}
