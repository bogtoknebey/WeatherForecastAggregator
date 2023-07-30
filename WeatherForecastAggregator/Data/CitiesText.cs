using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Net;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using WeatherForecastAggregator.Models;

namespace WeatherForecastAggregator.Data
{
    public class CitiesText
    {
        public static List<City> GetCities(int citiesCount = int.MaxValue)
        {
            List<City> cities = new List<City>();
            string path = "ftp://91.238.103.67/wfa.user25206.realhost-free.net/Data/Cities.txt";
            NetworkCredential credentials = new NetworkCredential("user25206", "RpX6F8kGl68E");

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = credentials;
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            //using (Stream ftpStream = request.GetResponse().GetResponseStream())
            //using (Stream fileStream = File.Create(@"C:\local\path\file.zip"))
            using (Stream ftpStream = request.GetResponse().GetResponseStream())
            using (StreamReader reader = new StreamReader(ftpStream))
            {
                bool isOn = true;
                int counter = 0;
                while (isOn && counter < citiesCount)
                {
                    List<string> currCityProp = new List<string>();
                    for (int i = 0; i < 4; i++)
                    {
                        string? currStr = reader.ReadLine();
                        currCityProp.Add(currStr);
                    }
                    City currCity = new City()
                    {
                        Russian = currCityProp[0],
                        Ukrainian = currCityProp[1],
                        RussianTranslit = currCityProp[2],
                        UkrainianTranslit = currCityProp[3]
                    };
                    cities.Add(currCity);

                    try
                    {
                        reader.ReadLine();
                    }
                    catch
                    {
                        isOn = false;
                    }

                    counter++;
                }
            }

            return cities;
        }


        public static City Kharkiv = new City
        {
            Russian = "харьков",
            Ukrainian = "харків",
            RussianTranslit = "kharkov",
            UkrainianTranslit = "kharkiv"
        };
        public static City Kyiv = new City
        {
            Russian = "киев",
            Ukrainian = "київ",
            RussianTranslit = "kiev",
            UkrainianTranslit = "kyiv"
        };
        public static City Dnipro = new City
        {
            Russian = "днепропетровск",
            Ukrainian = "дніпро",
            RussianTranslit = "dnepr",
            UkrainianTranslit = "dnipr"
        };
        public static City Ternopil = new City
        {
            Russian = "тернополь",
            Ukrainian = "тернопіль",
            RussianTranslit = "ternopol",
            UkrainianTranslit = "ternopil"
        };
    }
}
