using MySqlConnector;
using MySqlConnector.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastAggregator.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherForecastAggregator.DTO
{
    public class WeatherDays
    {
        private const string tableName_ = "weather_days";
        public static string tableName { get { return tableName_; } }

        public static bool Add(WeatherDay wd)
        {
            Connector conn = new Connector();
            int ind = conn.GetNewInd(tableName);

            Cities.AddIfThereNo(wd.City);
            Forecasters.AddIfThereNo(wd.Forecaster);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"{wd.City.Id}, " +
                $"{wd.Forecaster.Id}, " +
                $"'{Format.DBFormat(wd.Date)}', " +
                $"'{Format.DBFormat(wd.ForecastDate)}'); ";
            bool res = conn.ExecuteNonQueryRequest(sql);

            if (res)
                foreach (var wh in wd.WeatherHours)
                    WeatherHours.Add(wh, ind, conn);

            conn.CloseConnection();
            return res;
        }
        public static bool Add(WeatherDay wd, Connector conn)
        {
            int ind = conn.GetNewInd(tableName);

            Cities.AddIfThereNo(wd.City);
            Forecasters.AddIfThereNo(wd.Forecaster);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"{wd.City.Id}, " +
                $"{wd.Forecaster.Id}, " +
                $"'{Format.DBFormat(wd.Date)}', " +
                $"'{Format.DBFormat(wd.ForecastDate)}'); ";
            bool res = conn.ExecuteNonQueryRequest(sql);

            if (res)
                foreach (var wh in wd.WeatherHours)
                    WeatherHours.Add(wh, ind);
            return res;
        }
        public static bool Delete(int ind)
        {
            Connector conn = new Connector();
            string sql = $"DELETE FROM {tableName} WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }
        public static bool Update(WeatherDay wd, int ind)
        {
            Connector conn = new Connector();
            string sql = $"" +
                $"UPDATE {tableName} " +
                $"SET " +
                $"city_id = {wd.City.Id}, " +
                $"forecaster_id = {wd.Forecaster.Id}, " +
                $"_date = '{wd.Date}', " +
                $"forecast_date = '{wd.ForecastDate}' " +
                $"WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }


        public static List<WeatherDay> GetAll()
        {
            List<WeatherDay> wds = new List<WeatherDay>();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName);

                while (rdr.Read())
                {
                    int ind = Convert.ToInt32(rdr[0].ToString());
                    List<WeatherHour> whs = WeatherHours.GetByForeignKey(ind);
                    City? city = Cities.GetById(Convert.ToInt32(rdr[1].ToString()));
                    Forecaster? forecaster = Forecasters.GetById(Convert.ToInt32(rdr[2].ToString()));
                    DateOnly date = Format.CodeDateOnlyFormat(rdr[3].ToString());
                    DateTime forecastDate = Format.CodeDateTimeFormat(rdr[4].ToString());

                    WeatherDay wd = new WeatherDay()
                    {
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        WeatherHours = whs,
                        City = city,
                        Forecaster = forecaster,
                        Date = date,
                        ForecastDate = forecastDate
                    };
                    wds.Add(wd);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return wds;
        }
        public static WeatherDay? GetById(int ind)
        {
            WeatherDay? wd = new WeatherDay();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, $"Id = {ind}");
                rdr.Read();

                List<WeatherHour> whs = WeatherHours.GetByForeignKey(ind);
                City? city = Cities.GetById(Convert.ToInt32(rdr[1].ToString()));
                Forecaster? forecaster = Forecasters.GetById(Convert.ToInt32(rdr[2].ToString()));
                DateOnly date = Format.CodeDateOnlyFormat(rdr[3].ToString());
                DateTime forecastDate = Format.CodeDateTimeFormat(rdr[4].ToString());

                wd = new WeatherDay()
                {
                    Id = Convert.ToInt32(rdr[0].ToString()),
                    WeatherHours = whs,
                    City = city,
                    Forecaster = forecaster,
                    Date = date,
                    ForecastDate = forecastDate
                };
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                wd = null;
            }

            conn.CloseConnection();
            return wd;
        }


        public static List<WeatherDay> GetWeatherDays(int forecasterId, int cityId, DateOnly wdDate)
        {
            List<WeatherDay> wds = new List<WeatherDay>();
            Connector conn = new Connector();
            string dateStr = Format.DBFormat(wdDate);
            try
            {
                string condition = $"city_id = {cityId} AND forecaster_id = {forecasterId} AND _date = '{dateStr}'";
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, condition);

                while (rdr.Read())
                {
                    int ind = Convert.ToInt32(rdr[0].ToString());
                    List<WeatherHour> whs = WeatherHours.GetByForeignKey(ind);
                    City? city = Cities.GetById(Convert.ToInt32(rdr[1].ToString()));
                    Forecaster? forecaster = Forecasters.GetById(Convert.ToInt32(rdr[2].ToString()));
                    DateOnly date = Format.CodeDateOnlyFormat(rdr[3].ToString());
                    DateTime forecastDate = Format.CodeDateTimeFormat(rdr[4].ToString());

                    WeatherDay wd = new WeatherDay()
                    {
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        WeatherHours = whs,
                        City = city,
                        Forecaster = forecaster,
                        Date = date,
                        ForecastDate = forecastDate
                    };
                    wds.Add(wd);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return wds;
        }
        public static Dictionary<DateTime, List<WeatherHour>> GetWeatherHours(int forecasterId, int cityId, DateOnly wdDate)
        {
            Dictionary<DateTime, List<WeatherHour>> res = new Dictionary<DateTime, List<WeatherHour>>();
            Connector conn = new Connector();
            string dateStr = Format.DBFormat(wdDate);

            try
            {
                string condition = $"city_id = {cityId} AND forecaster_id = {forecasterId} AND _date = '{dateStr}'";
                string orderBy = "forecast_date";
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, condition, orderBy);

                while (rdr.Read())
                {
                    int ind = Convert.ToInt32(rdr[0].ToString());
                    List<WeatherHour> whs = WeatherHours.GetByForeignKey(ind);
                    DateTime forecastDate = Format.CodeDateTimeFormat(rdr[4].ToString());

                    res.Add(forecastDate, whs);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return res;
        }
    }
}
