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
        public string BaseLink = BaseLinks.Link2;


        public WeatherDay GetWeatherDay(City city)
        {
            WeatherDay weatherDay = new WeatherDay() { City = city, Day = DateTime.Now };
            string pathToFile = AppDomain.CurrentDomain.BaseDirectory + '\\';
            IWebDriver driver = new ChromeDriver(pathToFile);
            driver.Navigate().GoToUrl(BaseLink);

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
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[3]/div/div/div[" + i.ToString() + "]/span[1]";
                temperature = Convert.ToInt32(driver.FindElement(By.XPath(xStr)).Text.Replace('−', '-'));
                

                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[1]/div[" + i.ToString() + "]/span";
                hourStr = driver.FindElement(By.XPath(xStr)).Text;
                hourStr = hourStr.Substring(0, hourStr.Length - 2);
                hour = Convert.ToInt32(hourStr);

                WeatherHour weatherHour = new WeatherHour { Temperature = temperature, Hour = hour };
                weatherDay.WeatherHours.Add(weatherHour);
            }

            driver.Close();
            driver.Quit();
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

            WeatherDay weatherDay = new WeatherDay() { City = city, Day = DateTime.Now };

            string xStr = "";
            string hourStr = "";
            int temperature, hour;
            int tdCount = 8;
            for (int i = 1; i <= tdCount; i++)
            {
                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[3]/div/div/div[" + i.ToString() + "]/span[1]";
                string strTemperature = htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText.Replace("&minus;", "-");
                temperature = Convert.ToInt32(strTemperature);


                xStr = "/html/body/section/div[1]/section[3]/div/div/div/div[1]/div[" + i.ToString() + "]/span";
                hourStr = htmlDoc.DocumentNode.SelectSingleNode(xStr).InnerText;
                hourStr = hourStr.Substring(0, hourStr.Length - 2);
                hour = Convert.ToInt32(hourStr);

                WeatherHour weatherHour = new WeatherHour { Temperature = temperature, Hour = hour };
                weatherDay.WeatherHours.Add(weatherHour);
            }

            return weatherDay;
        }


        async public Task<string> GetWeatherDayHtml(City city)
        {
            // https://www.gismeteo.ua/mq/search/тернополь/1/
            // TODO https://2cyr.com/decode/?lang=ru

            string link = BaseLink + "/mq/search/" + city.Russian.ToLower() + "/1/";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(link);
            string myJsonString = await response.Content.ReadAsStringAsync();
            JObject myJObject = JObject.Parse(myJsonString);

            link = BaseLink + myJObject.SelectToken("data")[0].SelectToken("url").Value<string>();
            client = new HttpClient();
            response = await client.GetAsync(link);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
