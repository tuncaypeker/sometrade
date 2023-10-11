namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Williams Percent Range Indicator.
    /// </summary>
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
    /*
     //@version=5
indicator("Williams Percent Range", shorttitle="Williams %R", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
length = input(title="Length", defval=14)
src = input(close, "Source")
_pr(length) =>
	max = ta.highest(length)
	min = ta.lowest(length)
	100 * (src - max) / (max - min)
percentR = _pr(length)
obPlot = hline(-20, title="Upper Band", color=#787B86)
hline(-50, title="Middle Level", linestyle=hline.style_dotted, color=#787B86)
osPlot = hline(-80, title="Lower Band", color=#787B86)
fill(obPlot, osPlot, title="Background", color=color.rgb(126, 87, 194, 90))
plot(percentR, title="%R", color=#7E57C2)
     */ 
    /// </summary>
    public class WPR
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period, double[] high, double[] low, double[] close)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var wpr = new double[price.Length];

            for (var i = period; i < price.Length; i++)
            {
                var highest = double.MinValue;
                var lowest = double.MaxValue;

                for (int j = i - period + 1; j <= i; j++)
                {
                    if (high[j] > highest)
                    {
                        highest = high[j];
                    }

                    if (low[j] < lowest)
                    {
                        lowest = low[j];
                    }
                }

                wpr[i] = -100 * (highest - close[i]) / (highest - lowest);
            }

            return wpr;
        }
    }
}
