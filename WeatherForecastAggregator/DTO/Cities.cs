using MySqlConnector;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.DTO
{
    public static class Cities
    {
        private const string tableName_ = "cities";
        public static string tableName { get { return tableName_; } }


        public static bool IsThere(City c)
        {
            Connector conn = new Connector();
            string where = $"" +
                $"latitude = {c.latitude.ToString(CultureInfo.InvariantCulture)} AND " +
                $"longitude = {c.longitude.ToString(CultureInfo.InvariantCulture)};";

            bool res = conn.IsThere(tableName, where);
            conn.CloseConnection();
            return res;
        }
        public static bool AddIfThereNo(City c)
        {
            if (IsThere(c))
                return false;

            Connector conn = new Connector();
            int ind = conn.GetNewInd(tableName);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"'{c.Russian}', " +
                $"'{c.Ukrainian}', " +
                $"'{c.RussianTranslit}', " +
                $"'{c.UkrainianTranslit}', " +
                $"{c.latitude.ToString(CultureInfo.InvariantCulture)}, " +
                $"{c.longitude.ToString(CultureInfo.InvariantCulture)});";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
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
        public static bool Update(City c, int ind)
        {
            Connector conn = new Connector();
            string sql = $"" +
                $"UPDATE {tableName} " +
                $"SET " +
                $"Russian = '{c.Russian}', " +
                $"Ukrainian = '{c.Ukrainian}', " +
                $"RussianTranslit = '{c.RussianTranslit}', " +
                $"UkrainianTranslit = '{c.UkrainianTranslit}', " +
                $"latitude = {c.latitude.ToString(CultureInfo.InvariantCulture)}, " +
                $"longitude = {c.longitude.ToString(CultureInfo.InvariantCulture)} " +
                $"WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }


        public static List<City> GetAll()
        {
            List<City> cities = new List<City>();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName);

                while (rdr.Read())
                {
                    City city = new City() { 
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        Russian = rdr[1].ToString(),
                        Ukrainian = rdr[2].ToString(),
                        RussianTranslit = rdr[3].ToString(),
                        UkrainianTranslit = rdr[4].ToString(),
                        latitude = Convert.ToDouble(rdr[5].ToString()),
                        longitude = Convert.ToDouble(rdr[6].ToString())
                    };
                    cities.Add(city);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return cities;
        }
        public static City? GetById(int ind)
        {
            City? city = new City();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, $"Id = {ind}");
                rdr.Read();
                city = new City()
                {
                    Id = Convert.ToInt32(rdr[0].ToString()),
                    Russian = rdr[1].ToString(),
                    Ukrainian = rdr[2].ToString(),
                    RussianTranslit = rdr[3].ToString(),
                    UkrainianTranslit = rdr[4].ToString(),
                    latitude = Convert.ToDouble(rdr[5].ToString()),
                    longitude = Convert.ToDouble(rdr[6].ToString())
                };
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                city = null;
            }

            conn.CloseConnection();
            return city;
        }
    }
}
