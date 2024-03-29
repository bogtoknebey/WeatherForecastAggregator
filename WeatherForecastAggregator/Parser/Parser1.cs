﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using WeatherForecastAggregator.Models;
using System.IO;
using HtmlAgilityPack;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.DTO;

namespace WeatherForecastAggregator.Parser
{
    public class Parser1 : IParser
    {
        public const int respondDelay = 10000;
        public static Forecaster Forecaster { get; set; } = new Forecaster(0, "sinoptik", "https://sinoptik.ua");

        public WeatherDay GetWeatherDay(City city)
        {
            // https://sinoptik.ua/
            // https://sinoptik.ua/погода-харьков

            string link = Forecaster.BaseLink + "/погода-" + city.Russian.ToLower();

            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + '\\';
            IWebDriver driver = new ChromeDriver(pathToFile);
            driver.Navigate().GoToUrl(link);

            string xStr = "";
            int temperature, hour;
            int tdCount = 8;
            List<WeatherHour> weatherHours = new List<WeatherHour>();
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "//tr[@class=\"temperature\"]/td[" + i.ToString() + "]";
                temperature = GetTemperature(driver.FindElement(By.XPath(xStr)).Text);

                xStr = "//tr[@class=\"gray time\"]/td[" + i.ToString() + "]";
                hour = GetHour(driver.FindElement(By.XPath(xStr)).Text);

                WeatherHour weatherHour = new WeatherHour(temperature, hour);
                weatherHours.Add(weatherHour);
            }

            driver.Close();
            driver.Quit();

            WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);
            return weatherDay;
        }


        private int GetHour(string s)
        {
            // 2:00 
            Regex regex = new Regex(@"^\d+");
            MatchCollection matches = regex.Matches(s);

            if (matches.Count == 1)
            {
                foreach (Match match in matches)
                {
                    return Convert.ToInt32(match.Value);
                }
            }

            return -1;
        }


        private int GetTemperature(string s)
        {
            // +5°
            // -5°
            Regex regex = new Regex(@"[-]?\d+");
            MatchCollection matches = regex.Matches(s);

            if (matches.Count == 1)
            {
                foreach (Match match in matches)
                {
                    return Convert.ToInt32(match.Value);
                }
            }

            return -1;
        }


        async public Task<WeatherDay?> GetWeatherDayAPI(City city)
        {
            var task = GetWeatherDayHtml(city);
            if (await Task.WhenAny(task, Task.Delay(respondDelay)) == task)
            {
                // The task completed within {respondDelay} seconds
                string? html = await task;
                if (html is null)
                    return null;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                string xStr = "";
                int temperature, hour;
                int tdCount = 8;
                List<WeatherHour> weatherHours = new List<WeatherHour>();
                for (int i = 1; i <= tdCount; i++)
                {
                    // get temperature
                    xStr = "//tr[@class=\"temperature\"]/td[" + i.ToString() + "]";
                    var selectedNode = htmlDoc.DocumentNode.SelectSingleNode(xStr);
                    if (selectedNode is not null)
                    {
                        try
                        {
                            temperature = GetTemperature(selectedNode.InnerText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("There no such element");
                        return null;
                    }

                    // get hour
                    xStr = "//tr[@class=\"gray time\"]/td[" + i.ToString() + "]";
                    selectedNode = htmlDoc.DocumentNode.SelectSingleNode(xStr);
                    if (selectedNode is not null)
                    {
                        try
                        {
                            hour = GetHour(selectedNode.InnerText);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("There no such element");
                        return null;
                    }

                    WeatherHour weatherHour = new WeatherHour { Temperature = temperature, Hour = hour };
                    weatherHours.Add(weatherHour);
                }

                WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);

                return weatherDay;
            }
            else
            {
                // The task did not complete within {respondDelay} seconds
                return null;
            }
        }


        async public Task<string?> GetWeatherDayHtml(City city)
        {
            try 
            {
                // https://sinoptik.ua/
                // https://sinoptik.ua/погода-харьков

                string link = Forecaster.BaseLink + "/погода-" + city.Russian.ToLower();
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(link);
                return await response.Content.ReadAsStringAsync();
            } 
            catch (Exception ex) 
            {
                return null;
            }
        }
    }
}
