using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs.Helpers
{
    public static class Extensions
    {
        public static double Round(this double value, int round = 2)
        {
            return Math.Round(value, round);
        }

        // Ex: collection.TakeLast(5);
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        /// <summary>
        /// Sondan bir oncekini alır
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T TakePrev<T>(this IEnumerable<T> source)
        {
            return source.TakeLast(2).FirstOrDefault();
        }

        public static Binance.Net.Enums.KlineInterval ToBinanceInterval(this Model.TimeIntervalEnum timeIntervalEnum)
        {
            switch (timeIntervalEnum)
            {
                case Model.TimeIntervalEnum.EightHour: return Binance.Net.Enums.KlineInterval.EightHour;
                case Model.TimeIntervalEnum.FifteenMinutes: return Binance.Net.Enums.KlineInterval.FifteenMinutes;
                case Model.TimeIntervalEnum.FiveMinutes: return Binance.Net.Enums.KlineInterval.FiveMinutes;
                case Model.TimeIntervalEnum.FourHour: return Binance.Net.Enums.KlineInterval.FourHour;
                case Model.TimeIntervalEnum.OneDay: return Binance.Net.Enums.KlineInterval.OneDay;
                case Model.TimeIntervalEnum.OneHour: return Binance.Net.Enums.KlineInterval.OneHour;
                case Model.TimeIntervalEnum.OneMinute: return Binance.Net.Enums.KlineInterval.OneMinute;
                case Model.TimeIntervalEnum.OneMonth: return Binance.Net.Enums.KlineInterval.OneMonth;
                case Model.TimeIntervalEnum.OneWeek: return Binance.Net.Enums.KlineInterval.OneWeek;
                case Model.TimeIntervalEnum.SixHour: return Binance.Net.Enums.KlineInterval.SixHour;
                case Model.TimeIntervalEnum.ThirtyMinutes: return Binance.Net.Enums.KlineInterval.ThirtyMinutes;
                case Model.TimeIntervalEnum.ThreeDay: return Binance.Net.Enums.KlineInterval.ThreeDay;
                case Model.TimeIntervalEnum.ThreeMinutes: return Binance.Net.Enums.KlineInterval.ThreeMinutes;
                case Model.TimeIntervalEnum.TwelveHour: return Binance.Net.Enums.KlineInterval.TwelveHour;
                case Model.TimeIntervalEnum.TwoHour: return Binance.Net.Enums.KlineInterval.TwoHour;
                default: return Binance.Net.Enums.KlineInterval.OneMonth;
            }
        }

        public static int ToMinute(this Model.TimeIntervalEnum timeIntervalEnum)
        {
            switch (timeIntervalEnum)
            {
                case Model.TimeIntervalEnum.EightHour: return 8 * 60;
                case Model.TimeIntervalEnum.FifteenMinutes: return 15;
                case Model.TimeIntervalEnum.FiveMinutes: return 5;
                case Model.TimeIntervalEnum.FourHour: return 4 * 60;
                case Model.TimeIntervalEnum.OneDay: return 24 * 60;
                case Model.TimeIntervalEnum.OneHour: return 1 * 60;
                case Model.TimeIntervalEnum.OneMinute: return 1;
                case Model.TimeIntervalEnum.OneMonth: return 30 * 24 * 60;
                case Model.TimeIntervalEnum.OneWeek: return 7 * 24 * 60;
                case Model.TimeIntervalEnum.SixHour: return 6 * 1;
                case Model.TimeIntervalEnum.ThirtyMinutes: return 30;
                case Model.TimeIntervalEnum.ThreeDay: return 3 * 24 * 60;
                case Model.TimeIntervalEnum.ThreeMinutes: return 3;
                case Model.TimeIntervalEnum.TwelveHour: return 20 * 60;
                case Model.TimeIntervalEnum.TwoHour: return 2 * 60;
                default: return 30 * 24 * 60;
            }
        }

        /// <summary>
        /// Gecici sinyale izin verilmediginde, mum kapanisindan sonra 
        /// ne kadar sure icinde kontrol edilirse isleme izin vereyim
        /// 30 dklık mum da kapandıktan sonra ilk 60 sn içinde işlem açtın açtın mesela
        /// 1 dklık mum da 10 sn içinde açtın açtın gibi
        /// </summary>
        /// <param name="timeIntervalEnum"></param>
        /// <returns></returns>
        public static int ToTolerateInSeconds(this Model.TimeIntervalEnum timeIntervalEnum)
        {
            switch (timeIntervalEnum)
            {
                case Model.TimeIntervalEnum.EightHour: return 60;
                case Model.TimeIntervalEnum.FifteenMinutes: return 30;
                case Model.TimeIntervalEnum.FiveMinutes: return 30;
                case Model.TimeIntervalEnum.FourHour: return 60;
                case Model.TimeIntervalEnum.OneDay: return 120;
                case Model.TimeIntervalEnum.OneHour: return 60;
                case Model.TimeIntervalEnum.OneMinute: return 10;
                case Model.TimeIntervalEnum.OneMonth: return 300;
                case Model.TimeIntervalEnum.OneWeek: return 150;
                case Model.TimeIntervalEnum.SixHour: return 60;
                case Model.TimeIntervalEnum.ThirtyMinutes: return 30;
                case Model.TimeIntervalEnum.ThreeDay: return 150;
                case Model.TimeIntervalEnum.ThreeMinutes: return 10;
                case Model.TimeIntervalEnum.TwelveHour: return 90;
                case Model.TimeIntervalEnum.TwoHour: return 60;
                default: return 10;
            }
        }

        public static string ToTurkishMeaning(this Model.TimeIntervalEnum timeIntervalEnum)
        {
            switch (timeIntervalEnum)
            {
                case Model.TimeIntervalEnum.EightHour: return "8S";
                case Model.TimeIntervalEnum.FifteenMinutes: return "15dk";
                case Model.TimeIntervalEnum.FiveMinutes: return "5dk";
                case Model.TimeIntervalEnum.FourHour: return "4S";
                case Model.TimeIntervalEnum.OneDay: return "1G";
                case Model.TimeIntervalEnum.OneHour: return "1S";
                case Model.TimeIntervalEnum.OneMinute: return "1dk";
                case Model.TimeIntervalEnum.OneMonth: return "1A";
                case Model.TimeIntervalEnum.OneWeek: return "1H";
                case Model.TimeIntervalEnum.SixHour: return "6S";
                case Model.TimeIntervalEnum.ThirtyMinutes: return "30dk";
                case Model.TimeIntervalEnum.ThreeDay: return "3G";
                case Model.TimeIntervalEnum.ThreeMinutes: return "3dk";
                case Model.TimeIntervalEnum.TwelveHour: return "20S";
                case Model.TimeIntervalEnum.TwoHour: return "2S";
                default: return "";
            }
        }
    }
}
