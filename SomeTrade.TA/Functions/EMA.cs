namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Exponential Moving Average Indicator.
    /// description:
    ///     The exponential moving average (EMA) is a technical chart indicator that tracks the price of an investment (like a stock or commodity) over time.
    /// link:
    ///     https://www.investopedia.com/ask/answers/122314/what-exponential-moving-average-ema-formula-and-how-ema-calculated.asp
    /// serkan:
    ///     En çok kullanılan hareketli ortalamalar
    ///         => Çok Kısa Dönem Teknik Analiz Fiyat Hareketleri İçin 6-18
    ///         => Kısa Dönem Trend Analizi İçin 14-25
    ///         => Orta Vadeli Trend Analizleri İçin 26-49
    ///         => Orta Uzun Vadeli Analizler İçin 50-100
    ///         => Uzun Vadeli Analizler İçin 100-200
    ///     Hareketli Ortalamalar, kıvanç yorumlar
    ///         => 5 Günlük EMA Güçlü Momentum, ani dönüşler, saatlik bar'da hemen al sat
    ///         => 10 Günlük EMA Kısa Vadeli Trend, hemen gir çıklar, saatlikte bakıyorsak 5-10 saat max, destek direnç olarak çalışmaz
    ///         => 20 Günlük Ema Pull Back Desteği, destek direnç göstermeye başlar
    ///         => 50 Günlük Ema Savunma Hattı Tepki Bölgesi, artık o kadar hız istemiyoruz. burdan sma'ya dönebiliriz. sma50'de önemlidir
    ///         => 100 Günlük Ema Kuvvetli Destek Direnç Bölgesi Tepki Bölgesi, onay alıp sonraki yeşil mum'da girilebilir
    ///         => 200 Günlük Ema Trend Yön değişim Bölgesi. Tepki Bölgesi. Trendin son kalesi. 
    ///  kivanc- Hareketli Ortalamalarla Algo Trade:
    ///     Son barları çok daha fazla alan hesaplama mantığı
    ///     Weight değeri, etki çarpanıdır ve genel olarak 2 alınır.
    ///     Weight son barın etki değerini arttırmak için kullanılır. 1 olursa neredeyse basit hareketli ortalama elde edilir
    ///     EMA(9) = Close * 2/(9+1) + OncekiEMA * (1-2/(9+1))
    ///     EMA fiyatı SMA'ya göre daha hızlı takip eder. Tabi 100-200 bar alınıyorsa aslında frene basıyoruz demektir
    ///     50'nin üstü ema'larda sma kullanmak daha saglıklı olabilir daha dogrusu aynı noktaya gelmiş olursun
    ///     Hızlı hareket eden piyasalarda kullanılır
    /// pinescript
    /*
    
    //@version=5
    indicator(title="Moving Average Exponential", shorttitle="EMA", overlay=true, timeframe="", timeframe_gaps=true)
    len = input.int(9, minval=1, title="Length")
    src = input(close, title="Source")
    offset = input.int(title="Offset", defval=0, minval=-500, maxval=500)
    out = ta.ema(src, len)
    plot(out, title="EMA", color=color.blue, offset=offset)

    ma(source, length, type) =>
        switch type
            "SMA" => ta.sma(source, length)
            "EMA" => ta.ema(source, length)
            "SMMA (RMA)" => ta.rma(source, length)
            "WMA" => ta.wma(source, length)
            "VWMA" => ta.vwma(source, length)

    typeMA = input.string(title = "Method", defval = "SMA", options=["SMA", "EMA", "SMMA (RMA)", "WMA", "VWMA"], group="Smoothing")
    smoothingLength = input.int(title = "Length", defval = 5, minval = 1, maxval = 100, group="Smoothing")

    smoothingLine = ma(out, smoothingLength, typeMA)
    plot(smoothingLine, title="Smoothing Line", color=#f37f20, offset=offset, display=display.none)


     */ 
    /// </summary>
    public class EMA
    {
        /// <summary>
        /// Calculates indicator.1
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period, double weight = 2.0)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(price.Length);

            //period'a kadar sma hesaplanmali
            var sma = SMA.Calculate(price.Take(period).ToArray(), period);

            double multiplier = weight / (period + 1);
            double[] ema = new double[price.Length];
            for (int i = period; i < price.Length; i++)
            {
                var prevEma = i == period
                     ? sma[i - 1]
                     : ema[i - 1];

                ema[i] = (price[i] - prevEma) * multiplier + prevEma;
            }

            return ema;
        }

        public static double Last(double[] price, int period) => Calculate(price, period).Last();
        public static double Prev(double[] price, int period) => Calculate(price, period).TakeLast(2).First();
    }
}
