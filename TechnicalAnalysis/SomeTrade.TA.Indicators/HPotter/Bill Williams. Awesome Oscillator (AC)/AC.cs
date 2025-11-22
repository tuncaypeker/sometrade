namespace SomeTrade.TA.Indicators.HPotter
{
    using CuttingEdge.Conditions;
    

    /// <summary>
    /// name:
    ///     Accelerator / Decelerator Indicator.
    /// description:
    ///     
    /// link:
    ///     https://www.metatrader5.com/en/terminal/help/indicators/bw_indicators/ao
    /// formula:
    ///     MEDIAN PRICE = (HIGH + LOW) / 2
    ///     AO = SMA (MEDIAN PRICE, 5) - SMA (MEDIAN PRICE, 34)
    ///     AC = AO - SMA (AO, 5)
    /// pinescript:
    ///     nLengthSlow = input(34, minval=1, title="Length Slow")
    ///     nLengthFast = input(5, minval=1, title="Length Fast")
    ///     xSMA1_hl2 = sma(hl2, nLengthFast)
    ///     xSMA2_hl2 = sma(hl2, nLengthSlow)
    ///     xSMA1_SMA2 = xSMA1_hl2 - xSMA2_hl2
    ///     sSMAlast = sma(xSMA1_SMA2,nLengthFast)
    ///
    ///     cClr = xSMA1_SMA2 > xSMA1_SMA2[1] ? blue : red
    ///     plot(sSMAlast - xSMA1_SMA2, style=histogram, linewidth=1, color=cClr)
    /// </summary>
    public class AC
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] high, double[] low, int fastLength, int slowLength)
        {
            Condition.Requires(high, "high").IsNotEmpty();

            var ao = AO.Calculate(high, low, fastLength, slowLength);
            var smaOfAo = TA.SMA.Calculate(ao, fastLength);
            var ac = new double[high.Length];
            for (var i = 0; i < low.Length; ++i)
            {
                ac[i] = ao[i] - smaOfAo[i];
            }

            return ac;
        }
    }
}
