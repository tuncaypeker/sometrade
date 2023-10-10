namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
	using SomeTrade.TA.Dto;

	/// <summary>
	/// name:
	///     
	/// description:
	///     Hareketli ortalamalarýn farký alýnarak hesaplanýr
	///     26 bar hareketli ortalamadan, 12 barlýk üssel harketli ortalama çýkarýlýr [MACD]
	///     Bu çýkan sonucun 9 üssel harketli ortalamasi alýnarak bir çizgi daha elde edilir [MACD Signal ya da Trigger]
	///     Macd - Macd Trigger 3. bir çizgi oluþturur [Histogram]
	/// link:
	///     
	/// formula:
	///     
	/// pinescript:
	/*
     
    //@version=5
indicator(title="Moving Average Convergence Divergence", shorttitle="MACD", timeframe="", timeframe_gaps=true)
// Getting inputs
fast_length = input(title="Fast Length", defval=12)
slow_length = input(title="Slow Length", defval=26)
src = input(title="Source", defval=close)
signal_length = input.int(title="Signal Smoothing",  minval = 1, maxval = 50, defval = 9)
sma_source = input.string(title="Oscillator MA Type",  defval="EMA", options=["SMA", "EMA"])
sma_signal = input.string(title="Signal Line MA Type", defval="EMA", options=["SMA", "EMA"])
// Plot colors
col_macd = input(#2962FF, "MACD Line  ", group="Color Settings", inline="MACD")
col_signal = input(#FF6D00, "Signal Line  ", group="Color Settings", inline="Signal")
col_grow_above = input(#26A69A, "Above   Grow", group="Histogram", inline="Above")
col_fall_above = input(#B2DFDB, "Fall", group="Histogram", inline="Above")
col_grow_below = input(#FFCDD2, "Below Grow", group="Histogram", inline="Below")
col_fall_below = input(#FF5252, "Fall", group="Histogram", inline="Below")
// Calculating
fast_ma = sma_source == "SMA" ? ta.sma(src, fast_length) : ta.ema(src, fast_length)
slow_ma = sma_source == "SMA" ? ta.sma(src, slow_length) : ta.ema(src, slow_length)
macd = fast_ma - slow_ma
signal = sma_signal == "SMA" ? ta.sma(macd, signal_length) : ta.ema(macd, signal_length)
hist = macd - signal
plot(hist, title="Histogram", style=plot.style_columns, color=(hist>=0 ? (hist[1] < hist ? col_grow_above : col_fall_above) : (hist[1] < hist ? col_grow_below : col_fall_below)))
plot(macd, title="MACD", color=col_macd)
plot(signal, title="Signal", color=col_signal)

     */
	/// strateji:
	///     1: MACD çizgisi, Trigger çizgisini yukarý kestiginde AL, tersi oldugunda SAT
	///     2: MACD çizgisi 0'ý yukarý kestiðinde AL, aþaðý kestiðinde SAT (Aslýnda bunun için MACD'yi gerek yok, EMA12 ve EMA26 Kesiþimi de ayný noktalarý ifade eder)
	/// parameters:
	///     default deðerleri 26,12,9 ama burda mantýk þu eskiden forex piyasalarý 6 gün olduðu için 26 = 1Ay, 12 = 2 hafta diye düþünülmeli
	///     Tüm indikatörler için mantýk þudur, perioyt küçüldükçe daha sýk sinyal ve hata artar, büyürse az sinyal az hata alýnýr ama geç girilir
	/// </summary>
	public class MACD
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="price"></param>
        /// <param name="fastPeriod"></param>
        /// <param name="slowPeriod"></param>
        /// <param name="signalPeriod"></param>
        /// <param name="maType">ema or sma</param>
        /// <returns></returns>
        public static MacdResultDto Calculate(double[] price, int fastPeriod, int slowPeriod, int signalPeriod, string maType = "ema")
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var result = new Dto.MacdResultDto()
            {
                MACD = new double[price.Length],
                MACDHistory = new double[price.Length],
                MACDSignal = new double[price.Length]
            };

            var fast_ma = maType == "sma" ? TA.SMA.Calculate(price, fastPeriod) : TA.EMA.Calculate(price, fastPeriod);
            var slow_ma = maType == "sma" ? TA.SMA.Calculate(price, slowPeriod) : TA.EMA.Calculate(price, slowPeriod);

            for (int i = 0; i < price.Length; i++)
            {
                result.MACD[i] = fast_ma[i] - slow_ma[i];
            }

            result.MACDSignal = maType == "sma" ? TA.SMA.Calculate(result.MACD, signalPeriod) : TA.EMA.Calculate(result.MACD, signalPeriod);
            for (int i = 0; i < price.Length; i++)
            {
                result.MACDHistory[i] = result.MACD[i] - result.MACDSignal[i];
            }

            return result;
        }
    }
}
