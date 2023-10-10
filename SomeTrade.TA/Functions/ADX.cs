namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Average Directional Index
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     //@version=5
    ///     indicator("Average Directional Index", shorttitle="ADX", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
    ///     adxlen = input(14, title="ADX Smoothing")
    ///     dilen = input(14, title="DI Length")
    ///     dirmov(len) =>
    ///     	up = ta.change(high)
    ///     	down = -ta.change(low)
    ///     	plusDM = na(up) ? na : (up > down and up > 0 ? up : 0)
    ///     	minusDM = na(down) ? na : (down > up and down > 0 ? down : 0)
    ///     	truerange = ta.ema(ta.tr, len)
    ///     	plus = fixnan(100 * ta.ema(plusDM, len) / truerange)
    ///     	minus = fixnan(100 * ta.ema(minusDM, len) / truerange)
    ///     	[plus, minus]
    ///     adx(dilen, adxlen) =>
    ///     	[plus, minus] = dirmov(dilen)
    ///     	sum = plus + minus
    ///     	adx = 100 * ta.ema(math.abs(plus - minus) / (sum == 0 ? 1 : sum), adxlen)
    ///     sig = adx(dilen, adxlen)
    ///     plot(sig, color=color.red, title="ADX")
    /// </summary>
    public class ADX
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
            Condition.Requires(low, "timeSeries")
                .IsNotNull();

            var dx = new double[price.Length];
            var pDi = DmiPlus.Calculate(price, period, high, low);
            var mDi = DmiMinus.Calculate(price, period, high, low);

            for (var i = 0; i < price.Length; ++i)
            {
                var sum = pDi[i] + mDi[i];
                dx[i] = sum.IsAlmostZero()
                     ? 0
                     : 100 * (System.Math.Abs(pDi[i] - mDi[i]) / (sum == 0 ? 1 : sum));

                if (double.IsNaN(dx[i]))
                    dx[i] = 0;
            }

            var adx = TA.EMA.Calculate(dx, period);

            return adx;
        }

        public static double Last(double[] price, int period, double[] high, double[] low) => Calculate(price, period, high, low).Last();
        public static double Prev(double[] price, int period, double[] high, double[] low) => Calculate(price, period, high, low).TakeLast(2).First();
    }
}
