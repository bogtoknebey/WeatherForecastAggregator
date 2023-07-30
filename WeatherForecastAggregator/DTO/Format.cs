using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecastAggregator.DTO
{
    public static class Format
    {
        // DateTime to YYYY-MM-DD hh:mm:ss  
        // DateOnly to YYYY-MM-DD 
        public static string DBFormat(DateTime dt)
        {
            string res = $"{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}";
            return res;
        }
        public static string DBFormat(DateOnly d)
        {
            string res = $"{d.Year}-{d.Month}-{d.Day}";
            return res;
        }


        public static DateTime CodeDateTimeFormat(string s)
        {
            DateTime d = new DateTime();
            string[] sepStr = s.Split(' ');
            if (sepStr.Length == 3)
            {
                // 7/24/2023 3:56:08 AM
                string[] dateStr = sepStr[0].Split('/');
                string[] timeStr = sepStr[1].Split(':');
                string dayPart = sepStr[2];

                int year = Convert.ToInt32(dateStr[2]);
                int month = Convert.ToInt32(dateStr[0]);
                int day = Convert.ToInt32(dateStr[1]);

                int hours = Convert.ToInt32(timeStr[0]);
                if (dayPart == "AM")
                {
                    if (hours == 12)
                        hours = 0;
                }
                else if (dayPart == "PM")
                {
                    if (hours != 12)
                        hours += 12;
                }


                int minutes = Convert.ToInt32(timeStr[1]);
                int seconds = Convert.ToInt32(timeStr[2]);

                d = new DateTime(
                    year,
                    month,
                    day,
                    hours,
                    minutes,
                    seconds
                );

                // d = DateTime.ParseExact(s, "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
            }
            else if (sepStr.Length == 2)
            {
                // 21.07.2023 23:07:14
                string[] dateStr = sepStr[0].Split('.');
                string[] timeStr = sepStr[1].Split(':');

                d = new DateTime(
                    Convert.ToInt32(dateStr[2]),
                    Convert.ToInt32(dateStr[1]),
                    Convert.ToInt32(dateStr[0]),
                    Convert.ToInt32(timeStr[0]),
                    Convert.ToInt32(timeStr[1]),
                    Convert.ToInt32(timeStr[2])
                );

            }

            return d;
        }
        public static DateOnly CodeDateOnlyFormat(string s)
        {
            // 10.07.2023 0:00:00            
            string[] sepStr = s.Split(' ')[0].Split('.');
            int day = Convert.ToInt32(sepStr[0]);
            int month = Convert.ToInt32(sepStr[1]);
            int  year = Convert.ToInt32(sepStr[2]);
            DateOnly d = new DateOnly(
                year,
                month,
                day
            );

            return d;
        }


        public static void Test()
        {
            string dbDO = "2000-10-10";
            string dbDT = "2000-10-10 10:10:10";
            DateOnly date = new DateOnly(2000, 10, 20);
            DateTime dateTime = new DateTime(2000, 10, 20, 10, 10, 10);


            Console.WriteLine(Format.DBFormat(date));
            Console.WriteLine(Format.DBFormat(dateTime));

            Console.WriteLine(Format.CodeDateOnlyFormat(dbDO));
            Console.WriteLine(Format.CodeDateTimeFormat(dbDT));
        }
    }
}
