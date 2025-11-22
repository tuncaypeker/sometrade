using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.TA
{
    public static class _Extensions
    {
       /// <summary>
        /// Checks if the number is almost zero.
        /// </summary>
        /// <param name="value">Checked number.</param>
        /// <returns>true if is almost zero</returns>
        public static bool IsAlmostZero(this double value)
        {
            return System.Math.Abs(value) < double.Epsilon;
        }

        /// <summary>
        /// Compares the numbers.
        /// </summary>
        /// <param name="value">First number.</param>
        /// <param name="compareTo">Second number.</param>
        /// <returns>true if almost equal</returns>
        public static bool AlmostEqual(this double value, double compareTo)
        {
            return System.Math.Abs(value - compareTo) < double.Epsilon;
        }

        /// <summary>
        /// Subtracts one series from another.
        /// </summary>
        /// <param name="src">Source series.</param>
        /// <param name="dst">Subtracted series.</param>
        public static void Subtract(this double[] src, double[] dst)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src[i] -= dst[i];
            }
        }

        /// <summary>
        /// Subtracts one series from another and creates new series.
        /// </summary>
        /// <param name="src">Source series.</param>
        /// <param name="dst">Subtracted series.</param>
        /// <returns>New series.</returns>
        public static double[] CreateSubstract(this double[] src, double[] dst)
        {
            var t = new double[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                t[i] = src[i] - dst[i];
            }

            return t;
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
    }
}
