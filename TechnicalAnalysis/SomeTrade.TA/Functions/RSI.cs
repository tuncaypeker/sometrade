namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    /// Relative Strength Index Indicator.
    ///     
    /// description:
    ///     
    /// link:
    ///     https://www.youtube.com/watch?v=Sf8VVScxBqg
    /// formula:
    ///     
    /// notlar:
    ///     Bir Momentum indikatörü ve aşırı alım satım noktalarını anlamak için kullanılıyor
    ///     Momentum güç ölçümünü değişimden, farktan gücünü alır
    ///     Momentum göstergeleri öncü göstergelerdir.
    ///     Fiyat hareketlerien duyarlıdır
    ///     Öncü oldukları için de uyumsuzluklar önemli hale gelir
    ///     
    ///     - Tepe ve Dipleri Gösterir [Genelde >70 ve <30 değeri altında tepe ve dipler görülür]
    ///     - Formasyonları Gösterir 
    ///     - Trend Dönüşlerini Gösterir
    ///     
    ///     RSI Kısa vadeli işlemlerde çok fazla fake yiyebilir.
    ///     RSI grafiği üzerinde de trend çizilmeli, mum formasyonları aranmalı
    /// pinescript:
    /*
     //@version=5
    indicator(title="Relative Strength Index", shorttitle="RSI", format=format.price, precision=2, timeframe="", timeframe_gaps=true)

    ma(source, length, type) =>
        switch type
            "SMA" => ta.sma(source, length)
            "Bollinger Bands" => ta.sma(source, length)
            "EMA" => ta.ema(source, length)
            "SMMA (RMA)" => ta.rma(source, length)
            "WMA" => ta.wma(source, length)
            "VWMA" => ta.vwma(source, length)

    rsiLengthInput = input.int(14, minval=1, title="RSI Length", group="RSI Settings")
    rsiSourceInput = input.source(close, "Source", group="RSI Settings")
    maTypeInput = input.string("SMA", title="MA Type", options=["SMA", "Bollinger Bands", "EMA", "SMMA (RMA)", "WMA", "VWMA"], group="MA Settings")
    maLengthInput = input.int(14, title="MA Length", group="MA Settings")
    bbMultInput = input.float(2.0, minval=0.001, maxval=50, title="BB StdDev", group="MA Settings")

    up = ta.rma(math.max(ta.change(rsiSourceInput), 0), rsiLengthInput)
    down = ta.rma(-math.min(ta.change(rsiSourceInput), 0), rsiLengthInput)
    rsi = down == 0 ? 100 : up == 0 ? 0 : 100 - (100 / (1 + up / down))
    rsiMA = ma(rsi, maLengthInput, maTypeInput)
    isBB = maTypeInput == "Bollinger Bands"

    plot(rsi, "RSI", color=#7E57C2)
    plot(rsiMA, "RSI-based MA", color=color.yellow)
    rsiUpperBand = hline(70, "RSI Upper Band", color=#787B86)
    hline(50, "RSI Middle Band", color=color.new(#787B86, 50))
    rsiLowerBand = hline(30, "RSI Lower Band", color=#787B86)
    fill(rsiUpperBand, rsiLowerBand, color=color.rgb(126, 87, 194, 90), title="RSI Background Fill")
    bbUpperBand = plot(isBB ? rsiMA + ta.stdev(rsi, maLengthInput) * bbMultInput : na, title = "Upper Bollinger Band", color=color.green)
    bbLowerBand = plot(isBB ? rsiMA - ta.stdev(rsi, maLengthInput) * bbMultInput : na, title = "Lower Bollinger Band", color=color.green)
    fill(bbUpperBand, bbLowerBand, color= isBB ? color.new(color.green, 90) : na, title="Bollinger Bands Background Fill")
     */ 
    /// </summary>
    public class RSI
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
                     .IsGreaterThan(0)
                     .IsLessOrEqual(price.Length);

            var rsi = new double[price.Length];

            double gain = 0.0;
            double loss = 0.0;

            // first RSI value
            rsi[0] = 0.0;
            for (int i = 1; i <= period; ++i)
            {
                var diff = price[i] - price[i - 1];
                if (diff >= 0)
                {
                    gain += diff;
                }
                else
                {
                    loss -= diff;
                }
            }

            double avrg = gain / period;
            double avrl = loss / period;
            double rs = gain / loss;
            rsi[period] = 100 - 100 / (1 + rs);

            for (int i = period + 1; i < price.Length; ++i)
            {
                var diff = price[i] - price[i - 1];

                if (diff >= 0)
                {
                    avrg = (avrg * (period - 1) + diff) / period;
                    avrl = avrl * (period - 1) / period;
                }
                else
                {
                    avrl = (avrl * (period - 1) - diff) / period;
                    avrg = avrg * (period - 1) / period;
                }

                rs = avrg / avrl;

                rsi[i] = 100 - 100 / (1 + rs);
            }

            return rsi;
        }
    }
}
