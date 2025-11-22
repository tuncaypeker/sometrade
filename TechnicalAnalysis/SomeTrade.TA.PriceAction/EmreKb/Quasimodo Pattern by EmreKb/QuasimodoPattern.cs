using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SomeTrade.TA.PriceAction.EmreKb.Quasimodo_Pattern_by_EmreKb
{
    internal class QuasimodoPattern
    {
        /// <summary>
        /// Find Quasimodo Patterns and returns ABCDE Points Also Quasimodo Line Level
        /// </summary>
        /// <param name="price"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double[] Calculate(double[] close, double[] open, double[] low, double[] high, int zigZagLength = 13)
        {
            double[] high_points_arr = new double[5];
            int[] high_index_arr = new int[5];
            double[] low_points_arr = new double[5];
            int[] low_index_arr = new int[5];
            int[] trend = new int[close.Length];

            for (int i = 1; i < close.Length; i++)
            {
                var to_up = high[i] >= TA.Highest.Calculate(high, zigZagLength)[0];
                var to_down = low[i] <= TA.Lowest.Calculate(low, zigZagLength)[0];

                int trendC = 1;
                trend[i] = trendC == 1 && to_down
                    ? -1
                    : trendC == -1 && to_up
                        ? 1
                        : trendC;
            }

            return null;
        }
    }
}
