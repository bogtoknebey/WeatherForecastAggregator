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
    public class Connector
    {
        public MySqlConnection connection { get; set; }

        public Connector()
        {
            SetConnection();
            OpenConnection();
        }

        public bool IsThere(string table, string whereCondition)
        {
            string sql = $"Select Id From {table}";
            if (whereCondition != "")
                sql += $" Where {whereCondition}";
            int? res = ExecuteScalarRequest(sql);
            return res is not null;
        }
        public int GetNewInd(string table)
        {
            string sql = $"SELECT MAX(Id) FROM {table}";
            return ExecuteScalarRequest(sql) + 1 ?? 0;
        }

        public bool ExecuteNonQueryRequest(string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public int? ExecuteScalarRequest(string sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                object result = cmd.ExecuteScalar();
                if (result is not null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public MySqlDataReader? ExecuteRequest(string sql) 
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public MySqlDataReader? ExecuteRequest(string fields, string table, string whereCondition = "", string orderBy = "")
        {
            try
            {
                string sql = $"SELECT {fields} FROM {table}";
                if (whereCondition != "")
                    sql += $" Where {whereCondition}";
                if (orderBy != "")
                    sql += $" Order By {orderBy}";

                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                return rdr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        private void SetConnection()
        {
            // scp.realhost.com.ua
            // 91.238.103.67
            // string connStr = "server=localhost;user=root;database=wfa;port=3306;password=root";
            string connStr = "Server=scp.realhost.com.ua;Database=wfa;Uid=wfauser;Pwd=fabkguqyrwhvnpcds2to";
            connection = new MySqlConnection(connStr);
        }
        private void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void CloseConnection()
        {
           if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}
