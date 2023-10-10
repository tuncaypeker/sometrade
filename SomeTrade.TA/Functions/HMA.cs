namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///     Bu hull mudur, teyit edelim
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     
    /// </summary>
    public class HMA
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

            var wmaLengthHalf = WMA.Calculate(price, period / 2);
            var wmaLength = WMA.Calculate(price, period);
            var hmaBeforeArray = new double[price.Length];

            for (var i = period - 1; i < price.Length; i++)
            {
                hmaBeforeArray[i] = 2 * wmaLengthHalf[i] - wmaLength[i];
            }

            var hma = WMA.Calculate(hmaBeforeArray, (int)System.Math.Floor(System.Math.Sqrt(period)));

            return hma;
        }
    }
}
