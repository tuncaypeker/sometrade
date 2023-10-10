using CuttingEdge.Conditions;

namespace SomeTrade.TA.Indicators.Everget
{
    /// <summary>
    /// name:
    ///     Ahrens Moving Average
    /// description:
    ///     Ahrens Moving Average is an indicator developed by Richard D. Ahrens.
    ///     The indicator was described in the article “Build A Better Moving Average” in the magazine "Technical Analysis of Stocks and Commodities" (October 2013).
    /// </summary>
    public class AHMA
    {
        /// <summary>
        /// Calculates indicator
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(price.Length);

            double[] ahma = new double[price.Length];
            for (int i = period; i < price.Length; i++)
            {
                ahma[i] = ahma[i-1] + (price[i] - (ahma[i-1] + ahma[period]) / 2) / period;
            }

            return ahma;
        }
    }
}
