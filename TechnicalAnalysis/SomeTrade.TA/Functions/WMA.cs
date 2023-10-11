namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Weighted Moving Average
    /// wmax = ta.wma(close, 10)
    /// plot(wmax)
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
    /// kivanc- Hareketli Ortalamalarla Algo Trade:
    ///     Her barın kendi ağırlığı var ve son bara yaklaştıkça ağırlık artar
    ///     (C1 * 1 + C2 * 2 + C3 * 3 + C4 * 4 + C5 * 5)/(5 * (5+1) / 2) => 5 * 5+1 aslında tüm agırlıkları toplamına denk geliyor
    ///     İlk sıradaki barların agırlıgını çok çok azalır.
    ///     // same on pine, but much less efficient
    ///      pine_wma(x, y) =>
    ///          norm = 0.0
    ///          sum = 0.0
    ///          for i = 0 to y - 1
    ///              weight = (y - i) * y
    ///              norm := norm + weight
    ///              sum := sum + x[i] * weight
    ///          sum / norm
    ///      plot(pine_wma(close, 15))
    ///  pinescript:
    /*
     
    //@version=5
    indicator(title="Moving Average Weighted", shorttitle="WMA", overlay=true, timeframe="", timeframe_gaps=true)
    len = input.int(9, minval=1, title="Length")
    src = input(close, title="Source")
    offset = input.int(title="Offset", defval=0, minval=-500, maxval=500)
    out = ta.wma(src, len)
    plot(out, title="WMA", color=color.blue, offset=offset)
      
     */ 
    /// </summary>
    public class WMA
    {
        /// <summary>
        /// Calculates indicator.
        /// Talib Technical Analysisden kopyaladim birebir
        /// mutlaka sadeleştrilmesi gerekiyor
        /// ATR gibi indikatorlerde kullanildigi icin eklemek durumnda kaldim
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

            double tempReal;
            int startIdx = 0;
            int endIdx = price.Length - 1;

            //validation
            var result = new double[price.Length];
            int lookbackTotal = period - 1;
            if (startIdx < lookbackTotal)
                startIdx = lookbackTotal;

            if (startIdx > endIdx)
                return result;

            int divider = period * (period + 1) >> 1;
            int outIdx = period - 1;
            int trailingIdx = startIdx - lookbackTotal;
            double periodSub = 0.0;
            double periodSum = periodSub;


            int inIdx = trailingIdx;
            int i = 1;
            while (true)
            {
                if (inIdx >= startIdx)
                    break;

                tempReal = price[inIdx];
                inIdx++;
                periodSub += tempReal;
                periodSum += tempReal * i;
                i++;
            }

            double trailingValue = 0.0;
            while (true)
            {
                if (inIdx > endIdx || outIdx >= result.Length)
                    break;

                tempReal = price[inIdx];
                inIdx++;
                periodSub += tempReal;
                periodSub -= trailingValue;
                periodSum += tempReal * period;
                trailingValue = price[trailingIdx];
                trailingIdx++;
                result[outIdx] = periodSum / divider;
                outIdx++;
                periodSum -= periodSub;
            }

            return result;
        }
    }
}
