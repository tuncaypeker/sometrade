namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Chande Momentum Oscillator
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     // the same on pine
    ///     f_cmo(src, length) =>
    ///         float mom = ta.change(src)
    ///         float sm1 = math.sum((mom >= 0) ? mom : 0.0, length)
    ///         float sm2 = math.sum((mom >= 0) ? 0.0 : -mom, length)
    ///         100 * (sm1 - sm2) / (sm1 + sm2)
    ///
    ///     plot(f_cmo(close, 5))
    /// </summary>
    public class CMO
    {
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(price.Length);

            double[] cmo = new double[price.Length];
            double[] m1 = new double[price.Length];
            double[] m2 = new double[price.Length];

            for (int i = 1; i < price.Length; i++)
            {
                var mom = price[i] - price[i - 1];
                m1[i] = mom >= 0.0 ? mom : 0.0;
                m2[i] = mom >= 0.0 ? 0.0 : (-1 * mom);

                double sm1 = m1.Take(i).TakeLast(period).Sum(x => x);
                double sm2 = m2.Take(i).TakeLast(period).Sum(x => x);

                cmo[i-1] = (i < period - 1)
                     ? 0
                     : 100 * (sm1 - sm2) / (sm1 + sm2);
            }

            return cmo;
        }
    }
}
