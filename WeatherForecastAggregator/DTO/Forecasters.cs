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
    public class Forecasters
    {
        private const string tableName_ = "forecasters";
        public static string tableName { get { return tableName_; } }


        public static bool IsThere(Forecaster f)
        {
            Connector conn = new Connector();
            string where = $"" +
                $"name = '{f.Name}' AND " +
                $"base_link = '{f.BaseLink}';";

            bool res = conn.IsThere(tableName, where);
            conn.CloseConnection();
            return res;
        }
        public static bool AddIfThereNo(Forecaster f)
        {
            if (IsThere(f))
                return false;

            Connector conn = new Connector();
            int ind = conn.GetNewInd(tableName);
            string sql = $"INSERT INTO {tableName} VALUES (" +
                $"{ind}, " +
                $"'{f.Name}', " +
                $"'{f.BaseLink}');";

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
        public static bool Update(Forecaster f, int ind)
        {
            Connector conn = new Connector();
            string sql = $"" +
                $"UPDATE {tableName} " +
                $"SET " +
                    $"Name = '{f.Name}', " +
                    $"BaseLink = '{f.BaseLink}' " +
                $"WHERE Id = {ind};";

            bool res = conn.ExecuteNonQueryRequest(sql);
            conn.CloseConnection();
            return res;
        }


        public static List<Forecaster> GetAll()
        {
            List<Forecaster> forecasters = new List<Forecaster>();
            Connector conn = new Connector();
            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", "forecasters");
                while (rdr.Read())
                {
                    Forecaster forecaster = new Forecaster()
                    {
                        Id = Convert.ToInt32(rdr[0].ToString()),
                        Name = rdr[1].ToString(),
                        BaseLink = rdr[2].ToString()
                    };
                    forecasters.Add(forecaster);
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
            }

            conn.CloseConnection();
            return forecasters;
        }
        public static Forecaster? GetById(int ind)
        {
            Forecaster? forecaster = new Forecaster();
            Connector conn = new Connector();

            try
            {
                MySqlDataReader rdr = conn.ExecuteRequest("*", "forecasters", $"Id = {ind}");
                rdr.Read();
                forecaster = new Forecaster()
                {
                    Id = Convert.ToInt32(rdr[0].ToString()),
                    Name = rdr[1].ToString(),
                    BaseLink = rdr[2].ToString()
                };
                rdr.Close();
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                forecaster = null;
            }

            conn.CloseConnection();
            return forecaster;
        }
    }
}
