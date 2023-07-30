namespace WeatherForecastAggregator.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Russian { get; set; }
        public string Ukrainian { get; set; }
        public string RussianTranslit { get; set; }
        public string UkrainianTranslit { get; set; }
        public double latitude { get; set;}
        public double longitude { get; set; }

        public City() { }
        public City(string russian, string ukrainian, string russianTranslit, string ukrainianTranslit, double latitude, double longitude)
        {
            Russian = russian;
            Ukrainian = ukrainian;
            RussianTranslit = russianTranslit;
            UkrainianTranslit = ukrainianTranslit;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
