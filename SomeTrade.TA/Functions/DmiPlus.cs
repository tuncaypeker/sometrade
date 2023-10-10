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
	///     //@version=5
    ///     indicator(title="Directional Movement Index", shorttitle="DMI", format=format.price, precision=4, timeframe="", timeframe_gaps=true)
    ///     lensig = input.int(14, title="ADX Smoothing", minval=1, maxval=50)
    ///     len = input.int(14, minval=1, title="DI Length")
    ///     up = ta.change(high)
    ///     down = -ta.change(low)
    ///     plusDM = na(up) ? na : (up > down and up > 0 ? up : 0)
    ///     minusDM = na(down) ? na : (down > up and down > 0 ? down : 0)
    ///     trur = ta.rma(ta.tr, len)
    ///     plus = fixnan(100 * ta.rma(plusDM, len) / trur)
    ///     minus = fixnan(100 * ta.rma(minusDM, len) / trur)
    ///     sum = plus + minus
    ///     adx = 100 * ta.rma(math.abs(plus - minus) / (sum == 0 ? 1 : sum), lensig)
    ///     plot(adx, color=#F50057, title="ADX")
    ///     plot(plus, color=#2962FF, title="+DI")
    ///     plot(minus, color=#FF6D00, title="-DI")     
    /// </summary>
    public class DmiPlus
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period, double[] high, double[] low)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);
            Condition.Requires(high, "high")
                .IsNotNull();

            var mdm = new double[price.Length];
            mdm[0] = 0.0;

            var trueRanges = new double[price.Length];
            for (int i = 1; i < price.Length; ++i)
            { 
                 //calculate tr
                var trueHigh = high[i] > price[i - 1] ? high[i] : price[i - 1];
                var trueLow = low[i] < price[i - 1] ? low[i] : price[i - 1];
                var tr = trueHigh - trueLow;

                trueRanges[i] = tr;
            }
            var trueRangesEma = TA.EMA.Calculate(trueRanges, period);

            var plusDms = new double[price.Length];
            for (int i = 1; i < price.Length; ++i)
            {
                var up = high[i] - high[i - 1];
                var down = low[i - 1] - low[i];

                double plusDm = up.IsAlmostZero()
                    ? 0
                    : up > down && up > 0
                        ? 100 * up
                        : 0;

                plusDms[i] = plusDm;
            }
            var plusDmsEma = TA.EMA.Calculate(plusDms, period);

            for (int i = 1; i < price.Length; ++i) {
                mdm[i] = plusDmsEma[i] / trueRangesEma[i];
            }

            return mdm;
        }
    }
}
