namespace SomeTrade.TA.Indicators
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// https://www.investopedia.com/terms/l/linearlyweightedmovingaverage.asp
    /// 
    /// Linearly Weighted Moving Average Indicator.
    /// A linearly weighted moving average (LWMA) is a moving average calculation that more heavily weights recent price data.
    /// The most recent price has the highest weighting, and each prior price has progressively less weight. 
    /// 
    /// WMA'nın bir mum geriden gelmesi özetle
    /// //@version=2
    ///  study("LWMA w/ Color Change V1.1", shorttitle="LWMA", overlay=true)
    ///
    ///  p = input(defval=35, title="Period", minval=1)
    ///
    ///  ma = wma(close, p)
    ///
    ///  c = ma[1] > ma[2] ? green : ma[1] < ma[2] ? red : yellow
    ///
    ///  plot(ma[1], color=c)
    ///  plot(ma, color=c)
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
    ///     
    /// </summary>
    public class LWMA
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

            var lwma = new double[price.Length];
            double avgsum = 0.0;
            double sum = 0.0;
            for (int i = 0; i < period - 1; i++)
            {
                avgsum += price[i] * (i + 1);
                sum += price[i];
            }

            var divider = period * (period + 1) / 2;
            for (int i = period - 1; i < price.Length-1; i++)
            {
                avgsum += price[i] * period;
                sum += price[i];
                lwma[i+1] = avgsum / divider;
                avgsum -= sum;
                sum -= price[i - period + 1];
            }

            return lwma;
        }
    }
}
