using System;

namespace SomeTrade.Infrastructure.Extensions
{
    public static class DateExtensions
    {
        public static DateTime ToUnixTimeToDateTime(this string timestamp)
        {
            long convertThis = long.Parse(timestamp);

            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds((double)convertThis);
            dateTime = dateTime.ToLocalTime();  // Change GMT time to your timezone
            return dateTime;
        }

        public static DateTime ToJsonTimeToDateTime(this string timestamp)
        {
            var seconds = long.Parse(timestamp);
            DateTimeOffset dateTimeOffset2 = DateTimeOffset.FromUnixTimeMilliseconds(seconds).DateTime;

            return dateTimeOffset2.DateTime;
        }

        public static long ToDateTimeToJsonTime(this DateTime datetime)
        {
            TimeSpan t = datetime - new DateTime(1970, 1, 1);
            long secondsSinceEpoch = (long)t.TotalSeconds;

            return secondsSinceEpoch;
        }

        public static bool EqualsUpToSeconds(this DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day &&
                   dt1.Hour == dt2.Hour && dt1.Minute == dt2.Minute && dt1.Second == dt2.Second;
        }
    }
}
