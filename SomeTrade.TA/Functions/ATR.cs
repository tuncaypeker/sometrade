namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///     Average True Range
    /// description:
    ///     Piyasanın oynaklıgını ölçer, varsayilan 14 periyotluk kullanilir
    ///     Fiyatlar yatay ilerliyorsa ATR küçüktür, yukarı aşapı yön göstermez
    ///     Mutlak değer ile calışır, negatif alamaz
    ///     Genelde üssel ortalama kullanılır
    ///     
    ///     Daha çok filtreleme ve onay mekanizması olarak kullanılır
    ///     Stop Loss belirlemekte yaygın olarak kullanılır
    ///         O gün ki ATR değerinin 2 katını ekleyerek stop belirlenebilir ve iz süren stop stratejisi olarak kullanılabilir
    /// link:
    ///     https://www.youtube.com/watch?v=Z9CeIS8nwpM kivanc anlatiyor
    /// formula:
    ///     ATR = MA(TR)
    ///     TR =  Max[(H − L),Abs(H − CP),Abs(L − CP)] where CP = Close[Period]
    /// strateji:
    ///     Fiyat ATR'nin 2 katından fazla yükselirse AL
    ///     Fiyat ATR'nin 2 katından fazla düşerse SAT
    /// pinescript:
    ///     //@version=5
    ///     indicator("ta.atr")
    ///     plot(ta.atr(14))
    ///     
    ///     //the same on pine
    ///     pine_atr(length) =>
    ///         trueRange = na(high[1])? high-low : math.max(math.max(high - low, math.abs(high - close[1])), math.abs(low - close[1]))
    ///         //true range can be also calculated with ta.tr(true)
    ///         ta.rma(trueRange, length)
    ///     
    ///     plot(pine_atr(14))
    ///     
    ///     //@version=5
    ///     indicator(title="Average True Range", shorttitle="ATR", overlay=false, timeframe="", timeframe_gaps=true)
    ///     length = input.int(title="Length", defval=14, minval=1)
    ///     smoothing = input.string(title="Smoothing", defval="RMA", options=["RMA", "SMA", "EMA", "WMA"])
    ///     ma_function(source, length) =>
	///         switch smoothing
	///            "RMA" => ta.rma(source, length)
	///            "SMA" => ta.sma(source, length)
	///            "EMA" => ta.ema(source, length)
	///            => ta.wma(source, length)
    ///     plot(ma_function(ta.tr(true), length), title = "ATR", color=color.new(#B71C1C, 0))
    /// </summary>
    public class ATR
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="period">Indicator period.</param>
        /// <param name="timeSeries">Instrument <c>ohlc</c> time series.</param>
        /// <param name="maType">sma, wma, ema, rma</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] close, double[] high, double[] low, int period, string maType = "sma")
        {
            Condition.Requires(close, "close").IsNotNull();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(close.Length);

            var temp = new double[close.Length];
            temp[0] = 0;

            for (var i = 1; i < close.Length; i++)
            {
                var diff1 = System.Math.Abs(close[i - 1] - high[i]);
                var diff2 = System.Math.Abs(close[i - 1] - low[i]);
                var diff3 = high[i] - low[i];

                var max = diff1 > diff2 ? diff1 : diff2;

                temp[i] = max > diff3 ? max : diff3;
            }

            double[] atr;
            switch (maType)
            {
                case "sma": atr = SMA.Calculate(temp, period); break;
                case "ema": atr = EMA.Calculate(temp, period); break;
                case "wma": atr = WMA.Calculate(temp, period); break;
                case "rma": atr = RMA.Calculate(temp, period); break;
                default: atr = new double[0]; break;
            }

            //bastaki hesaplamalari sifirlayalim
            for (int i = 0; i < period; i++)
                temp[i] = 0;

            return atr;
        }
    }
}
