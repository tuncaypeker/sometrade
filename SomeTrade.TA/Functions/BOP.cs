namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///     Balance of Power
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     //@version=5
    ///     indicator(title="Balance of Power", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
    ///     plot((close - open) / (high - low), color=color.red)
    /// </summary>
    public class BOP
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] open, double[] high, double[] close, double[] low)
        {
            Condition.Requires(open, "open").IsNotNull();

            var bop = new double[open.Length];
            for (var i = 0; i < open.Length; i++)
            {
                bop[i] = (high[i].AlmostEqual(low[i]))
                     ? 0
                     : (close[i] - open[i]) / (high[i] - low[i]);
            }

            return bop;
        }
    }
}
