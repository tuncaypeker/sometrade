using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    internal static class Extensions
    {
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

        public static double RoundTo(this double value, int count)
        {
            return Math.Round(value, count);
        }
    }
}
