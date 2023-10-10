namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Simple Moving Average Indicator.
    /// description:
    ///     The Simple Moving Average (SMA) is calculated by adding the price of an instrument over a number of time periods 
    ///     and then dividing the sum by the number of time periods. 
    ///     
    ///     SMA 50 cok sık kullanılan bir parametre
    ///     Artçıdır hareketli ortalamalar gecikir, teyit amacı ile kullanılır
    ///     SMA50 Güzel destek dirençler yapabilir.
    ///     
    ///     Örnek strateji
    ///       : Fiyat SMA50 üstüne çıkarsa al
    ///       : Alış noktasından %5 is süren stop ile takip et
    ///       
    /// link:
    ///     https://www.investopedia.com/ask/answers/042815/how-simple-moving-average-calculated.asp
    /// formula:
    ///     SMA = (Sum(Price,n))/n 
    /// kivanc- Hareketli Ortalamalarla Algo Trade:
    ///     Gürültü denilen fiyat zigzaglarını filtrelemek için kullanılır
    ///     Uzun periyotlarda etkilidir
    ///   // same on pine, but much less efficient
    ///     pine_sma(x, y) =>
    ///         sum = 0.0
    ///         for i = 0 to y - 1
    ///             sum := sum + x[i] / y
    ///         sum
    ///     plot(pine_sma(close, 15))    
    /// 
    /// </summary>
    public class SMA
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

            double sum = 0;
            var sma = new double[price.Length];

            for (var i = 0; i < period; i++)
                sum += price[i];

            for (var i = period - 1; i < price.Length; i++)
            {
                sum = 0;
                for (var j = i; j > i - period; j--)
                    sum += price[j];

                sma[i] = sum / period;
            }

            return sma;
        }

        public static double Last(double[] price, int period) => Calculate(price, period).Last();
        public static double Prev(double[] price, int period) => Calculate(price, period).TakeLast(2).First();
    }
}
