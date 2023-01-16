using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Requests;
using PdfSharp.Pdf;
using Telegram.Bot.Types.InputFiles;
using WeatherForecastAggregator.Models;
using WeatherForecastAggregator.Data;
using WeatherForecastAggregator.View;
using System.Net;
using System.IO;
using System.IO.Pipes;

namespace WeatherForecastAggregator.Bot
{
    public class Bot
    {
        public Dictionary<int, City> citiesDictionary;
        public List<string> baseLinks;
        public Bot(List<City> cities, List<string> baseLinks)
        {
            Dictionary<int, City> d = new Dictionary<int, City> ();
            int id = 0;
            foreach (City c in cities)
            {
                d.Add(id, c);
                id++;
            }
            citiesDictionary = d;
            this.baseLinks = baseLinks;
        }

        public void Run()
        {
            var botClient = new TelegramBotClient("5909015866:AAEAG3iU5fb00gqIbLZKPAQpKfrSfmVUqQc");
            botClient.StartReceiving(BotUpdate, BotError);
        }


        async private Task BotUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message)
            {
                await MessageUpdate(botClient, update, token);
            }
            if (update.Type == UpdateType.CallbackQuery)
            {
                await CallbackQueryUpdate(botClient, update, token);
            }

        }

        async private Task MessageUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                if (message.Text.ToLower().Contains("привет"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "ну привет");

                    List<List<InlineKeyboardButton>> buttons = new();
                    foreach (var pair in citiesDictionary)
                    {
                        City city = pair.Value;
                        int id = pair.Key;
                        InlineKeyboardButton urlButton = new InlineKeyboardButton(city.Russian);
                        urlButton.CallbackData = Convert.ToString(id);
                        buttons.Add(new List<InlineKeyboardButton> { urlButton });
                    }
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);

                    await botClient.SendTextMessageAsync(message.Chat.Id, "Выберите город", replyMarkup: keyboard);
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "поздоровайся");
                    return;
                }
            }
        }


        async private Task CallbackQueryUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            long chatId = update.CallbackQuery.Message.Chat.Id;
            int id = Convert.ToInt32(update.CallbackQuery.Data);
            City curCity = citiesDictionary[id];
            string cityName = curCity.Russian;

            string htmlStr = await GeneralView.GetHtmlAPI(new List<City> { curCity }, baseLinks);
            PdfDocument pdf = PhotoConverter.ConvertHtmlToPDF(htmlStr);

            /*
            string dirName = "ftp://91.238.103.67/wfa.user25206.realhost-free.net/Data/pdf/" + Convert.ToString(chatId);
            string path = dirName + "/" + cityName + ".pdf";
            try
            {
                NetworkCredential credentials = new NetworkCredential("user25206", "RpX6F8kGl68E");

                try
                {
                    FtpWebRequest request1 = (FtpWebRequest)WebRequest.Create(dirName);
                    request1.Credentials = credentials;
                    request1.Method = WebRequestMethods.Ftp.MakeDirectory;
                    FtpWebResponse response1 = (FtpWebResponse)request1.GetResponse();
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(chatId, "ошибка возникла при создании директории на сервере");
                    await botClient.SendTextMessageAsync(chatId, e.Message);
                }

                try
                {
                    FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(path);
                    request2.Credentials = credentials;
                    request2.Method = WebRequestMethods.Ftp.UploadFile;
                    using (MemoryStream memStream = new MemoryStream())
                    using (Stream ftpStream = request2.GetRequestStream())
                    {
                        pdf.Save(memStream, false); 
                        memStream.CopyTo(ftpStream);
                    }
                    FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(chatId, "ошибка возникла при загрузке пдф на сервер");
                    await botClient.SendTextMessageAsync(chatId, e.Message);
                }

                try
                {
                    FtpWebRequest request3 = (FtpWebRequest)WebRequest.Create(path);
                    request3.Credentials = credentials;
                    request3.Method = WebRequestMethods.Ftp.DownloadFile;
                    using (Stream ftpStream = request3.GetRequestStream())
                    {
                        InputOnlineFile iof = new InputOnlineFile(ftpStream, "your.pdf");
                        var send = await botClient.SendDocumentAsync(chatId, iof);
                    }
                    FtpWebResponse response3 = (FtpWebResponse)request3.GetResponse();
                }
                catch (Exception e)
                {
                    await botClient.SendTextMessageAsync(chatId, "ошибка возникла при скачивании пдф с сервера");
                    await botClient.SendTextMessageAsync(chatId, e.Message);
                }
            }
            catch (Exception e)
            {
                await botClient.SendTextMessageAsync(chatId, "ошибка возникла");
                await botClient.SendTextMessageAsync(chatId, e.Message);
            }
            */

            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    pdf.Save(memStream, false);
                    string uploadFileName = cityName + ".pdf";
                    InputOnlineFile iof = new InputOnlineFile(memStream, uploadFileName);
                    chatId = update.CallbackQuery.Message.Chat.Id;
                    var send = await botClient.SendDocumentAsync(chatId, iof);
                }
            }
            catch(Exception e)
            {
                await botClient.SendTextMessageAsync(chatId, "ошибка возникла");
                await botClient.SendTextMessageAsync(chatId, e.Message);
            }
            return;
        }


        private Task BotError(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            throw exception;
        }
    }
}
