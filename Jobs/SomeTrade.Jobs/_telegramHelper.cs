using SomeTrade.Jobs.Helpers;
using System;

namespace SomeTrade.Jobs
{
	public static class _telegramHelper
	{
		public static void _trySendAlertToTelegram(string symbol, Model.TimeIntervalEnum interval, string message)
        {
             Telegram.Bot.TelegramBotClient telegramBot 
                = new Telegram.Bot.TelegramBotClient("########", new System.Net.Http.HttpClient());

            try
            {
                //tp
                var me = telegramBot.SendTextMessageAsync("########", $"#{symbol} #{interval.ToTurkishMeaning()} => {message}").Result;

                //sp
                //var sevk = telegramBot.SendTextMessageAsync("########", $"#{symbol} #{interval.ToTurkishMeaning()} => {message}").Result;
            }
            catch (Exception exc)
            {

            }
        }
	}
}
