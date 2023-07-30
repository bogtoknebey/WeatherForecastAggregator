using System.Collections.Generic;
using WeatherForecastAggregator.DTO;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Analytics
{
    public static class Analyst
    {
        private static KeyValuePair<DateTime, Dictionary<int, int>> Converting(KeyValuePair<DateTime, List<WeatherHour>> dataItem)
        {
            DateTime resKey = dataItem.Key;
            Dictionary<int, int> resValue = new Dictionary<int, int>();

            foreach (var wh in dataItem.Value)
            {
                resValue.Add(wh.Hour, wh.Temperature);
            }

            return new KeyValuePair<DateTime, Dictionary<int, int>>(resKey, resValue);
        }
        private static Dictionary<DateTime, Dictionary<int, int>> Converting(Dictionary<DateTime, List<WeatherHour>> data)
        {
            Dictionary<DateTime, Dictionary<int, int>> res = new Dictionary<DateTime, Dictionary<int, int>>();

            foreach (var dataItem in data)
            {
                DateTime pairKey = dataItem.Key;
                Dictionary<int, int> pairValue = new Dictionary<int, int>();

                foreach (var wh in dataItem.Value)
                {
                    pairValue.Add(wh.Hour, wh.Temperature);
                }
                res.Add(pairKey, pairValue);
            }

            return res;
        }


        public static Dictionary<DateTime, Dictionary<int, int>> GetDeviations(Forecaster forecaster, City city, DateOnly date)
        {
            // this result Value (Dictionary<int, int>>) means
            // Key - hour(0,3,6...)
            // Value - Diviation (26-25, 24-24, 22-22...)
            Dictionary<DateTime, Dictionary<int, int>> diviations = new Dictionary<DateTime, Dictionary<int, int>>();
            Dictionary<DateTime, Dictionary<int, int>> data = Converting(WeatherDays.GetWeatherHours(forecaster.Id, city.Id, date));

            // !!!!
            //Dictionary<DateTime, Dictionary<int, int>> errorResponse = new Dictionary<DateTime, Dictionary<int, int>>();
            //errorResponse.Add(DateTime.Now, new Dictionary<int, int>());
            //Dictionary<DateTime, Dictionary<int, int>> errorResponse2 = new Dictionary<DateTime, Dictionary<int, int>>();

            if (data.Count < 1)
                return diviations;

            KeyValuePair<DateTime, Dictionary<int, int>> mostRealDatum = data.Last();
            foreach (var pair in data)
            {
                DateTime deviationTime = pair.Key;
                Dictionary<int, int> deviationValues = new Dictionary<int, int>();
                foreach (var datum in pair.Value)
                {
                    int currentHour = datum.Key;
                    int realTemperature = mostRealDatum.Value[currentHour];

                    int difference = Math.Abs(realTemperature - datum.Value);
                    deviationValues.Add(currentHour, difference);
                }

                diviations.Add(deviationTime, deviationValues);
            }

            return diviations;
        }

        public static Dictionary<int, int> GetBiggestDeviation(Forecaster forecaster, City city, DateOnly date)
        {
            Dictionary<DateTime, Dictionary<int, int>> diviations = GetDeviations(forecaster, city, date);
            Dictionary<int, int> biggestDiviations = new Dictionary<int, int>();
            if (diviations.Count < 1)
                return biggestDiviations;
            biggestDiviations = diviations.Values.First();

            int deviationsMaxSumm = 0;

            foreach(var deviation in diviations)
            {
                foreach (var deviationPair in deviation.Value)
                {
                    int currentHour = deviationPair.Key;
                    int currentDeviation = deviationPair.Value;

                    if (biggestDiviations[currentHour] < currentDeviation)
                        biggestDiviations[currentHour] = currentDeviation;
                }
            }
            return biggestDiviations;
        }
    }
}
