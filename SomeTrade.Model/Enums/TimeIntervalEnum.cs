namespace SomeTrade.Model
{
    public enum TimeIntervalEnum
    {
        /// <summary>
        /// 1m
        /// </summary>
        OneMinute,
        /// <summary>
        /// 3m
        /// </summary>
        ThreeMinutes,
        /// <summary>
        /// 5m
        /// </summary>
        FiveMinutes,
        /// <summary>
        /// 15m
        /// </summary>
        FifteenMinutes,
        /// <summary>
        /// 30m
        /// </summary>
        ThirtyMinutes,
        /// <summary>
        /// 1h
        /// </summary>
        OneHour,
        /// <summary>
        /// 2h
        /// </summary>
        TwoHour,
        /// <summary>
        /// 4h
        /// </summary>
        FourHour,
        /// <summary>
        /// 6h
        /// </summary>
        SixHour,
        /// <summary>
        /// 8h
        /// </summary>
        EightHour,
        /// <summary>
        /// 12h
        /// </summary>
        TwelveHour,
        /// <summary>
        /// 1d
        /// </summary>
        OneDay,
        /// <summary>
        /// 3d
        /// </summary>
        ThreeDay,
        /// <summary>
        /// 1w
        /// </summary>
        OneWeek,
        /// <summary>
        /// 1M
        /// </summary>
        OneMonth
    }

    public static class TimeIntervalEnumExtension
    {
        public static string ToIntervalStr(this TimeIntervalEnum timeInterval)
        {
            switch (timeInterval)
            {
                case Model.TimeIntervalEnum.OneDay: return "1D";
                case Model.TimeIntervalEnum.ThreeDay: return "3D";
                case Model.TimeIntervalEnum.OneWeek: return "1W";
                case Model.TimeIntervalEnum.OneMonth: return "1mo";
                case Model.TimeIntervalEnum.OneMinute: return "1";
                case Model.TimeIntervalEnum.ThreeMinutes: return "3";
                case Model.TimeIntervalEnum.FiveMinutes: return "5";
                case Model.TimeIntervalEnum.FifteenMinutes: return "15";
                case Model.TimeIntervalEnum.ThirtyMinutes: return "30";
                case Model.TimeIntervalEnum.OneHour: return "60";
                case Model.TimeIntervalEnum.FourHour: return "240";
                case Model.TimeIntervalEnum.TwoHour: return "120";
                case Model.TimeIntervalEnum.SixHour: return "360";
                case Model.TimeIntervalEnum.EightHour: return "480";
                case Model.TimeIntervalEnum.TwelveHour: return "720";
                default: return "";
            }
        }

        /// <summary>
        /// Csv dosyalarinda bulunan interval sekline cevirir
        /// C:\Users\tuncay\Desktop\Workspace\SomeTrade\UI\SomeTrade.WebUI\wwwroot\historical_data\BTCUSDT\[1d].csv
        /// </summary>
        /// <param name="timeInterval"></param>
        /// <returns></returns>
        public static string ToFileIntervalStr(this TimeIntervalEnum timeInterval)
        {
            switch (timeInterval)
            {
                case Model.TimeIntervalEnum.OneDay: return "1d";
                case Model.TimeIntervalEnum.ThreeDay: return "3d";
                case Model.TimeIntervalEnum.OneHour: return "1h";
                case Model.TimeIntervalEnum.FourHour: return "4h";
                case Model.TimeIntervalEnum.OneWeek: return "1w";
                case Model.TimeIntervalEnum.OneMonth: return "1mo";
                case Model.TimeIntervalEnum.OneMinute: return "1m";
                case Model.TimeIntervalEnum.ThreeMinutes: return "3m";
                case Model.TimeIntervalEnum.FiveMinutes: return "5m";
                case Model.TimeIntervalEnum.FifteenMinutes: return "15m";
                case Model.TimeIntervalEnum.ThirtyMinutes: return "30m";
                case Model.TimeIntervalEnum.TwoHour: return "2h";
                case Model.TimeIntervalEnum.SixHour: return "6h";
                case Model.TimeIntervalEnum.EightHour: return "8h";
                case Model.TimeIntervalEnum.TwelveHour: return "12h";
                default: return "";
            }
        }

        public static string ToFileIntervalFromString(this string resolution)
        {
            switch (resolution)
            {
                case "1D": return "1d";
                case "D": return "1d";
                case "3D": return "3d";
                case "60": return "1h";
                case "1H": return "1h";
                case "240": return "4h";
                case "4H": return "4h";
                case "1W": return "1w";
                case "1M": return "1mo";
                case "M": return "1mo";
                case "1m": return "1m";
                case "1": return "1m";
                case "3m": return "3m";
                case "3": return "3m";
                case "5m": return "5m";
                case "5": return "5m";
                case "15m": return "15m";
                case "15": return "15m";
                case "30m": return "30m";
                case "30": return "30m";
                case "2H": return "2h";
                case "120": return "2h";
                case "6H": return "6h";
                case "480": return "8h";
                case "8H": return "8h";
                case "720": return "12h";
                case "12H": return "12h";
                default: return "";
            }
        }

        public static TimeIntervalEnum ToTimeIntervalFromString(this string resolution)
        {
            switch (resolution)
            {
                case "1D": return Model.TimeIntervalEnum.OneDay;
                case "D": return Model.TimeIntervalEnum.OneDay;
                case "3D": return Model.TimeIntervalEnum.ThreeDay;
                case "60": return Model.TimeIntervalEnum.OneHour;
                case "1H": return Model.TimeIntervalEnum.OneHour;
                case "240": return Model.TimeIntervalEnum.FourHour;
                case "4H": return Model.TimeIntervalEnum.FourHour;
                case "1W": return Model.TimeIntervalEnum.OneWeek;
                case "1M": return Model.TimeIntervalEnum.OneMonth;
                case "M": return Model.TimeIntervalEnum.OneMonth;
                case "1m": return Model.TimeIntervalEnum.OneMinute;
                case "1": return Model.TimeIntervalEnum.OneMinute;
                case "3m": return Model.TimeIntervalEnum.ThreeMinutes;
                case "3": return Model.TimeIntervalEnum.ThreeMinutes;
                case "5m": return Model.TimeIntervalEnum.FiveMinutes;
                case "5": return Model.TimeIntervalEnum.FiveMinutes;
                case "15m": return Model.TimeIntervalEnum.FifteenMinutes;
                case "15": return Model.TimeIntervalEnum.FifteenMinutes;
                case "30m": return Model.TimeIntervalEnum.ThirtyMinutes;
                case "30": return Model.TimeIntervalEnum.ThirtyMinutes;
                case "2H": return Model.TimeIntervalEnum.TwoHour;
                case "120": return Model.TimeIntervalEnum.TwoHour;
                case "6H": return Model.TimeIntervalEnum.SixHour;
                case "480": return Model.TimeIntervalEnum.EightHour;
                case "8H": return Model.TimeIntervalEnum.EightHour;
                case "720": return Model.TimeIntervalEnum.TwelveHour;
                case "12H": return Model.TimeIntervalEnum.TwelveHour;
                default: return Model.TimeIntervalEnum.OneDay;
            }
        }
    }


}
