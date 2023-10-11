namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Triple-smoothed Exponential Moving Average Indicator.
    /// pinescript:
    /*
     //@version=5
indicator(title="TRIX", shorttitle="TRIX", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
length = input.int(18, minval=1)
out = 10000 * ta.change(ta.ema(ta.ema(ta.ema(math.log(close), length), length), length))
plot(out, color=#F44336, title="TRIX")
hline(0, color=#787B86, title="Zero")
     */ 
    /// </summary>
    public class TRIX
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
                .IsGreaterThan(1)
                .IsLessOrEqual(price.Length);

            var trix = new double[price.Length];

            var ema1 = TA.EMA.Calculate(price, period);
            var ema2 = TA.EMA.Calculate(ema1, period);
            var ema3 = TA.EMA.Calculate(ema2, period);
            
            trix[0] = 0.0;
            for (int i = 1; i < price.Length; ++i)
            {
                trix[i] = ema3[i].IsAlmostZero()
                    ? 0.0
                    : 100.0 * (ema3[i] - ema3[i - 1]) / ema3[i - 1];
            }

            return trix;
        }
    }
}
