using SomeTrade.Jobs.Helpers;
using System;

namespace SomeTrade.Jobs
{
	public static class _telegramHelper
	{
		public static void _trySendAlertToTelegram(string symbol, Model.TimeIntervalEnum interval, string message)
        {
             Telegram.Bot.TelegramBotClient telegramBot 
                = new Telegram.Bot.TelegramBotClient("5390153090:AAGbFkgDENFKE-LESgjhOMXe9q_ADAdUTgI", new System.Net.Http.HttpClient());

            try
            {
                //tuncaypeker
                var me = telegramBot.SendTextMessageAsync("1246177031", $"#{symbol} #{interval.ToTurkishMeaning()} => {message}").Result;

                //sevketpeker
                //var sevk = telegramBot.SendTextMessageAsync("499385402", $"#{symbol} #{interval.ToTurkishMeaning()} => {message}").Result;
            }
            catch (Exception exc)
            {

            }
        }
	}
}
