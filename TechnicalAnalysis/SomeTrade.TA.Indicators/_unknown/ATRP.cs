namespace SomeTrade.TA.Indicators
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///     
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     
    /// </summary>
    public class ATRP
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] close, double[] high, double[] low,  int period)
        {
            Condition.Requires(close, "close")
                .IsNotNull();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(close.Length);

            var atr = TA.ATR.Calculate(close, high, low, period, "ema");

            var atrp = new double[atr.Length];
            for (var i = period; i < close.Length; i++)
                atrp[i] = atr[i] * 100.0 / close[i];

            return atrp;
        }
    }
}
