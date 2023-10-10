namespace SomeTrade.TA
{
    using System;
    using CuttingEdge.Conditions;

    public class Lowest
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

            var lowest = new double[price.Length];
            lowest[0] = price[0];

            for (int i = 1; i < period; ++i)
            {
                if (price[i] < lowest[i - 1])
                    lowest[i] = price[i];
                else
                    lowest[i] = lowest[i - 1];
            }

            int lowestIdx = 0;
            for (int i = period; i < price.Length; ++i)
            {
                double lowestLow = double.MaxValue;
                var start = System.Math.Max(i - period + 1, lowestIdx);
                for (int s = start; s <= i; ++s)
                {
                    if (price[s] < lowestLow)
                    {
                        lowestLow = price[s];
                        lowestIdx = s;
                    }
                }

                lowest[i] = lowestLow;
            }

            return lowest;
        }
    }
}
