using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WeatherForecastAggregator.Models;
using System.Text.RegularExpressions;
using WeatherForecastAggregator.Data;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace WeatherForecastAggregator.Parser
{
    public class Parser3 : IParser
    {
        public static Forecaster Forecaster { get; set; } = new Forecaster(2, "meteo", "https://meteo.ua");


        public WeatherDay GetWeatherDay(City city)
        {
            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + '\\';
            IWebDriver driver = new ChromeDriver(pathToFile);
            driver.Navigate().GoToUrl(Forecaster.BaseLink);
            SelectLenguage(driver, 3);

            string searchInputStr = "//*[@id=\"search-main-field\"]";
            IWebElement searchInput = driver.FindElement(By.XPath(searchInputStr));
            searchInput.SendKeys(city.Ukrainian);
            Thread.Sleep(2000);
            

            string hrefStr = "//*[@id=\"eac-container-search-main-field\"]/ul/li[1]/div/a[1]";
            IWebElement link = driver.FindElement(By.XPath("//a[1]"));
            bool isActive = true;
            DateTime start = DateTime.Now;
            TimeSpan round = new TimeSpan(10000000 * 3);
            while (isActive && DateTime.Now - start < round)
            {
                isActive = false;
                try
                {
                    link = driver.FindElement(By.XPath(hrefStr));
                }
                catch
                {
                    isActive = true;
                }
            }
            link.Click();
            // SelectLenguage(driver);
            CloseAdvertising(driver, 15);

            // <div class="weather-detail__main-time">00:00</div>
            // <div class="weather-detail__main-degree">+1 °</ div >
            string xStr = "";
            int temperature, hour;
            
            int trCount = 2;
            int tdCount = 4;
            List<WeatherHour> weatherHours = new List<WeatherHour>();
            for (int i = 1; i <= trCount; i++)
            {
                for (int j = 1; j <= tdCount; j++)
                {
                    xStr = "/html/body/div[2]/div/div[2]/div/div[2]/div[1]/div[3]/div/div[2]/div[1]/div[1]/div[1]/div[2]/div[" + i.ToString() + "]/div[" + j.ToString() + "]/div/div[2]";
                    temperature = GetTemperature(driver.FindElement(By.XPath(xStr)).Text);
                    xStr = "/html/body/div[2]/div/div[2]/div/div[2]/div[1]/div[3]/div/div[2]/div[1]/div[1]/div[1]/div[2]/div[" + i.ToString() + "]/div[" + j.ToString() + "]/div/div[1]";
                    hour = GetHour(driver.FindElement(By.XPath(xStr)).Text);

                    WeatherHour weatherHour = new WeatherHour(temperature, hour);
                    weatherHours.Add(weatherHour);
                }
            }

            driver.Close();
            driver.Quit();

            WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);
            return weatherDay;
        }
        
        
        private int GetHour(string s)
        {
            // 03:00
            s = s.Substring(0, s.Length - 3);
            if (s[0] == '0')
            {
                s = s.Substring(1, s.Length - 1);
            }
            return Convert.ToInt32(s);
        }


        private int GetTemperature(string s)
        {
            // -5 °
            // +1 °
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


        private IWebElement? TryFindElement(IWebDriver driver, string xStr, int timeDelay)
        {
            bool isActive = true;
            DateTime start = DateTime.Now;
            TimeSpan round = new TimeSpan(10000000 * timeDelay);
            while (isActive && DateTime.Now - start < round)
            {
                try
                {
                    return driver.FindElement(By.XPath(xStr));
                }
                catch { }
            }
            return null;
        }


        private void SelectLenguage(IWebDriver driver, int timeDelay) 
        {
            string xStr = "//*[@id=\"choiceLanguage\"]/div/div/div[2]/div/button[2]";
            var element = TryFindElement(driver, xStr, timeDelay);
            element?.Click();
        }


        private void CloseAdvertising(IWebDriver driver, int timeDelay)
        {
            string xStr = "//*[@id=\"aswift_4\"]";
            var element = TryFindElement(driver, xStr, timeDelay);
            if (element != null)
            {
                driver.SwitchTo().Frame(element);
            }

            xStr = "//*[@id=\"ad_iframe\"]";
            element = TryFindElement(driver, xStr, timeDelay);
            if (element != null)
            {
                driver.SwitchTo().Frame(element);
            }
            xStr = "//*[@id=\"dismiss-button\"]";
            element = TryFindElement(driver, xStr, timeDelay);
            element?.Click();

            driver.SwitchTo().DefaultContent();
        }


        async public Task<WeatherDay> GetWeatherDayAPI(City city)
        {
            string html = await GetWeatherDayHtml(city);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            string xStr = "";
            int temperature, hour;
            int trCount = 2;
            int tdCount = 4;
            List<WeatherHour> weatherHours = new List<WeatherHour>();
            for (int i = 1; i <= trCount; i++)
            {
                for (int j = 1; j <= tdCount; j++)
                {
                    xStr = "/html/body/div[1]/div/div[2]/div/div[2]/div[1]/div[3]/div/div[2]/div[1]/div[1]/div[1]/div[2]/div[" + i.ToString() + "]/div[" + j.ToString() + "]/div/div[2]";
                    temperature = GetTemperature(htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText);
                    xStr = "/html/body/div[1]/div/div[2]/div/div[2]/div[1]/div[3]/div/div[2]/div[1]/div[1]/div[1]/div[2]/div[" + i.ToString() + "]/div[" + j.ToString() + "]/div/div[1]";
                    hour = GetHour(htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText);

                    WeatherHour weatherHour = new WeatherHour(temperature, hour);
                    weatherHours.Add(weatherHour);
                }
            }

            WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);
            return weatherDay;
        }


        async public Task<string> GetWeatherDayHtml(City city)
        {
            // https://meteo.ua/front/forecast/autocomplete?lang=ru&format=json&phrase=%D1%85%D0%B0%D1%80%D1%8C%D0%BA%D0%BE%D0%B2
            string link = Forecaster.BaseLink + "/front/forecast/autocomplete?lang=ru&format=json&phrase=" + city.Ukrainian.ToLower();
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(link);
            string myJsonString = await response.Content.ReadAsStringAsync();
            myJsonString = "{\"data\":" + myJsonString + "}";
            JObject myJObject = JObject.Parse(myJsonString);
            string RouteLink = myJObject.SelectToken("data")[0].SelectToken("url").Value<string>();
            link = Forecaster.BaseLink + "/" + RouteLink.Substring(1);
            client = new HttpClient();
            response = await client.GetAsync(link);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
