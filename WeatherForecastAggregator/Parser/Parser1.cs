﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using WeatherForecastAggregator.Models;
using System.IO;
using HtmlAgilityPack;
using WeatherForecastAggregator.Data;

namespace WeatherForecastAggregator.Parser
{
    public class Parser1 : IParser
    {
        public string BaseLink = BaseLinks.Link1;


        public WeatherDay GetWeatherDay(City city)
        {
            // https://sinoptik.ua/
            // https://sinoptik.ua/погода-харьков

            string link = BaseLink + "погода-" + city.Russian.ToLower();

            WeatherDay weatherDay = new WeatherDay() { City = city, Day = DateTime.Now };

            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + '\\';
            IWebDriver driver = new ChromeDriver(pathToFile);
            driver.Navigate().GoToUrl(link);

            
            string xStr = "";
            int temperature, hour;
            int tdCount = 8;
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "//tr[@class=\"temperature\"]/td[" + i.ToString() + "]";
                temperature = GetTemperature(driver.FindElement(By.XPath(xStr)).Text);

                xStr = "//tr[@class=\"gray time\"]/td[" + i.ToString() + "]";
                hour = GetHour(driver.FindElement(By.XPath(xStr)).Text);

                WeatherHour weatherHour = new WeatherHour { Temperature = temperature, Hour = hour };
                weatherDay.WeatherHours.Add(weatherHour);
            }

            driver.Close();
            driver.Quit();
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


        async public Task<WeatherDay> GetWeatherDayAPI(City city)
        {
            string html = await GetWeatherDayHtml(city);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            WeatherDay weatherDay = new WeatherDay() { City=city, Day=DateTime.Now };

            string xStr = "";
            int temperature, hour;
            int tdCount = 8;
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "//tr[@class=\"temperature\"]/td[" + i.ToString() + "]";
                temperature = GetTemperature(htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText);

                xStr = "//tr[@class=\"gray time\"]/td[" + i.ToString() + "]";
                hour = GetHour(htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText);

                WeatherHour weatherHour = new WeatherHour { Temperature = temperature, Hour = hour };
                weatherDay.WeatherHours.Add(weatherHour);
            }

            return weatherDay;
        }


        async public Task<string> GetWeatherDayHtml(City city)
        {
            // https://sinoptik.ua/
            // https://sinoptik.ua/погода-харьков

            string link = BaseLink + "погода-" + city.Russian.ToLower();
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(link);
            return await response.Content.ReadAsStringAsync();
        }
    }
}