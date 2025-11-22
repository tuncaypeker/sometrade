namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Biz ekledik bunu
    /// Relative Moving Average Indicator.
    /// <summary>
    /// name:
    ///     
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// </summary>
    public class RMA
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

            var rma = new double[price.Length];
            var sum = new double[price.Length];
            double alpha = 1 / (double)period;

            for (var i = period - 1; i < price.Length; i++)
            {
                sum[i] = alpha * price[i] + (1 - alpha) * sum[i - 1];
                rma[i] = sum[i];
            }

            return rma;
        }
    }
}
