namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Volatilite ölçmek için kullanılır. Her işlem çifti için farklı degere sahiptir.
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// 
    /// //the same on pine
    /// isZero(val, eps) => math.abs(val) <= eps
    ///
    /// SUM(fst, snd) =>
    ///     EPS = 1e-10
    ///     res = fst + snd
    ///     if isZero(res, EPS)
    ///         res := 0
    ///     else
    ///         if not isZero(res, 1e-4)
    ///             res := res
    ///         else
    ///             15
    ///
    /// pine_stdev(src, length) =>
    ///     avg = ta.sma(src, length)
    ///     sumOfSquareDeviations = 0.0
    ///     for i = 0 to length - 1
    ///         sum = SUM(src[i], -avg)
    ///         sumOfSquareDeviations := sumOfSquareDeviations + sum * sum
    ///
    ///     stdev = math.sqrt(sumOfSquareDeviations / length)
    /// plot(pine_stdev(close, 5))
    /// 
    /// </summary>
    public class STDDev
    {
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();
            var std = new double[price.Length];

            for (int i = period; i < price.Length; i++)
            {
                var currentBars = price.Skip(i - period).Take(period);

                double avg = currentBars.Average();
                std[i - 1] = System.Math.Sqrt(currentBars.Average(v => System.Math.Pow(v - avg, 2)));
            }

            return std;
        }
    }
}
