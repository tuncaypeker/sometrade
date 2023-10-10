namespace SomeTrade.TA
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
    /*
    
    //@version=5
    indicator("Bull Bear Power", shorttitle="BBP")
    lengthInput = input.int(13, title="Length")
    bullPower = high - ta.ema(close, lengthInput)
    bearPower = low - ta.ema(close, lengthInput)
    plot(bullPower + bearPower, title="BBPower")

     */ 
    /// </summary>
    public class BullsPower
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period, double[] high)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);
            Condition.Requires(high, "high")
                .IsNotNull();

            var bulls = new double[price.Length];

            var ema = TA.EMA.Calculate(price, period);
            for (var i = 0; i < price.Length; i++)
            {
                bulls[i] = high[i] - ema[i];
            }

            return bulls;
        }
    }
}
