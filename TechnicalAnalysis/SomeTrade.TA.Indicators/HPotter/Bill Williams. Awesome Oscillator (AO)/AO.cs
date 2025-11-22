namespace SomeTrade.TA.Indicators.HPotter
{
    using CuttingEdge.Conditions;
    

    /// <summary>
    /// name:
    ///     Awesome Oscillator Indicator.
    /// description:
    ///     
    /// link:
    ///     https://www.metatrader5.com/en/terminal/help/indicators/bw_indicators/awesome
    /// </summary>
    public class AO 
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] high, double[] low, int fastLength, int slowLength)
        {
            Condition.Requires(high, "high").IsNotEmpty();
            Condition.Requires(low, "low").IsNotEmpty();

            var medianPrice = new double[high.Length];
            for (int i = 0; i < medianPrice.Length; i++)
                medianPrice[i] = (high[i] + low[i]) / 2;

            var fastSma = TA.SMA.Calculate(medianPrice, fastLength);
            var slowSma = TA.SMA.Calculate(medianPrice, slowLength);
            var ao = new double[high.Length];

            for (int i = 0; i < high.Length; i++)
                ao[i] = fastSma[i] - slowSma[i];

            return ao;
        }
    }
}
