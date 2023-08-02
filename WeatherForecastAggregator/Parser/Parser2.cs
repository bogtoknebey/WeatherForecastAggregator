using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Parser
{
    public class Parser2 : IParser
    {
        public const int respondDelay = 15000;
        public static Forecaster Forecaster { get; set; } = new Forecaster(1, "meteofor", "https://meteofor.com.ua/");

        public WeatherDay GetWeatherDay(City city)
        {

            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + '\\';
            IWebDriver driver = new ChromeDriver(pathToFile);
            driver.Navigate().GoToUrl(Forecaster.BaseLink);

            string searchInputStr = "/html/body/header/div[2]/div[2]/div/div/div[1]/div/input";
            IWebElement searchInput = driver.FindElement(By.XPath(searchInputStr));
            searchInput.SendKeys(city.Russian);
            Thread.Sleep(2000);

            
            string hrefStr = "//a[@class=\"search-item list-item icon-menu icon-menu-gray\"][1]";
            var element = TryFindElement(driver, hrefStr, 3);
            element?.Click();

            // <span>5<sup class="time-sup">00</sup></span>
            // <span class="unit unit_temperature_c">−4</span>
            string xStr = "";
            string hourStr = "";
            int temperature, hour;
            int tdCount = 8;
            List<WeatherHour> weatherHours = new List<WeatherHour>();
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[3]/div/div/div[" + i.ToString() + "]/span[1]";
                temperature = Convert.ToInt32(driver.FindElement(By.XPath(xStr)).Text.Replace('−', '-'));
                

                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[1]/div[" + i.ToString() + "]/span";
                hourStr = driver.FindElement(By.XPath(xStr)).Text;
                hourStr = hourStr.Substring(0, hourStr.Length - 2);
                hour = Convert.ToInt32(hourStr);

                WeatherHour weatherHour = new WeatherHour(temperature, hour);
                weatherHours.Add(weatherHour);
            }

            driver.Close();
            driver.Quit();

            WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);
            return weatherDay;
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


        async public Task<WeatherDay> GetWeatherDayAPI(City city)
        {
            string html = await GetWeatherDayHtml(city);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);


            string xStr = "";
            string hourStr = "";
            int temperature, hour;
            int tdCount = 8;
            List<WeatherHour> weatherHours = new List<WeatherHour>();
            for (int i = 1; i <= tdCount; i++)
            {
                // /html/body/section/div[1]/section[3]/div[1]/div/div/div[4]/div/div/div[1]/span[1]
                // /html/body/section/div[1]/section[3]/div[1]/div/div/div[4]/div/div/div[2]/span[1]
                xStr = "/html/body/section/div[1]/section[3]/div[1]/div/div/div[4]/div/div/div[" + i.ToString() + "]/span[1]";
                string strTemperature = htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText.Replace("&minus;", "-");
                temperature = Convert.ToInt32(strTemperature);

                // /html/body/section/div[1]/section[3]/div[1]/div/div/div[2]/div[1]/span
                // /html/body/section/div[1]/section[3]/div[1]/div/div/div[2]/div[2]/span
                xStr = "/html/body/section/div[1]/section[3]/div[1]/div/div/div[2]/div[" + i.ToString() + "]/span";
                hourStr = htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText;
                hourStr = hourStr.Trim();
                hourStr = hourStr.Substring(0, hourStr.Length - 2);
                hour = Convert.ToInt32(hourStr);

                WeatherHour weatherHour = new WeatherHour(temperature, hour);
                weatherHours.Add(weatherHour);
            }

            WeatherDay weatherDay = new WeatherDay(weatherHours, city, Forecaster, DateOnly.FromDateTime(DateTime.Now), DateTime.Now);
            return weatherDay;
        }


        async public Task<string> GetWeatherDayHtml(City city)
        {
            // https://meteofor.com.ua/mq/search/тернополь/1/
            // TODO https://2cyr.com/decode/?lang=ru

            string link = Forecaster.BaseLink + "/mq/search/" + city.Russian.ToLower() + "/1/";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(link);
            string myJsonString = await response.Content.ReadAsStringAsync();
            JObject myJObject = JObject.Parse(myJsonString);

            link = Forecaster.BaseLink + myJObject.SelectToken("data")[0].SelectToken("url").Value<string>();
            client = new HttpClient();
            response = await client.GetAsync(link);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
