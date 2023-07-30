using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.DTO
{
    public class WeatherHours
    {
        private const string tableName_ = "weather_hours";
        public static string tableName { get { return tableName_; } }

        public static bool Add(WeatherHour wh, int WD_ind)
        {
            Connector conn = new Connector();
            int ind = conn.GetNewInd(tableName);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"{WD_ind}, " +
                $"{wh.Temperature}, " +
                $"{wh.Hour});";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }
        public static bool Add(WeatherHour wh, int WD_ind, Connector conn)
        {
            int ind = conn.GetNewInd(tableName);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"{WD_ind}, " +
                $"{wh.Temperature}, " +
                $"{wh.Hour});";
            return conn.ExecuteNonQueryRequest(sql);
        }


        public static bool Delete(int ind)
        {
            Connector conn = new Connector();
            string sql = $"DELETE FROM {tableName} WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }
        public static bool Update(WeatherHour wh, int ind)
        {
            Connector conn = new Connector();
            string sql = $"" +
                $"UPDATE {tableName} " +
                $"SET " +
                $"Temperature = {wh.Temperature}, " +
                $"Hour = {wh.Hour} " +
                $"WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }

        public static List<WeatherHour> GetAll()
        {
            List<WeatherHour> whs = new List<WeatherHour>();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName);

                while (rdr.Read())
                {
                    WeatherHour wh = new WeatherHour()
                    {
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        Temperature = Convert.ToInt32(rdr[2].ToString()),
                        Hour = Convert.ToInt32(rdr[3].ToString())
                    };
                    whs.Add(wh);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return whs;
        }
        public static WeatherHour? GetById(int ind)
        {
            WeatherHour? wh = new WeatherHour();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, $"Id = {ind}");
                rdr.Read();
                wh = new WeatherHour()
                {
                    Id = Convert.ToInt32(rdr[0].ToString()),
                    Temperature = Convert.ToInt32(rdr[2].ToString()),
                    Hour = Convert.ToInt32(rdr[3].ToString())
                };
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                wh = null;
            }

            conn.CloseConnection();
            return wh;
        }
        public static List<WeatherHour> GetByForeignKey(int foreignInd)
        {
            List<WeatherHour> whs = new List<WeatherHour>();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", tableName, $"weather_day_id = {foreignInd}");
                while (rdr.Read())
                {
                    WeatherHour wh = new WeatherHour()
                    {
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        Temperature = Convert.ToInt32(rdr[2].ToString()),
                        Hour = Convert.ToInt32(rdr[3].ToString())
                    };
                    whs.Add(wh);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return whs;
            }

            conn.CloseConnection();
            return whs;
        }
    }
}
