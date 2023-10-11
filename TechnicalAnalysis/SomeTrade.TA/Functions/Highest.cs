namespace SomeTrade.TA
{
    using System;
    using CuttingEdge.Conditions;

    public class Highest
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var highest = new double[price.Length];
            highest[0] = price[0];
            for (int i = 1; i < period; ++i)
            {
                highest[i] = price[i] > highest[i - 1]
                    ? price[i]
                    : highest[i - 1];
            }

            int highestIdx = 0;
            for (int i = period; i < price.Length; ++i)
            {
                double highestHigh = double.MinValue;
                var start = System.Math.Max(i - period + 1, highestIdx);
                for (int s = start; s <= i; ++s)
                {
                    if (price[s] > highestHigh)
                    {
                        highestHigh = price[s];
                        highestIdx = s;
                    }
                }

                highest[i] = highestHigh;
            }

            return highest;
        }
    }
}
