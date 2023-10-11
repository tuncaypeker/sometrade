namespace SomeTrade.TA.Indicators.Everget
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///     Normalized Average True Range (NATR)
    /// description:
    ///     Normalized Average True Range (NATR) attempts to normalize the average true range values across instruments by using the formula below.
    /// formula:
    ///     NATR = ATR(n) / Close * 100
    /// link: 
    ///     https://www.tradingtechnologies.com/xtrader-help/x-study/technical-indicator-definitions/normalized-average-true-range-natr/
    /// pinescript:
    ///     length = input(title="Length", type=integer, defval=14)
    ///     natr = 100 * atr(length) / close
    ///     plot(natr, color=#ff9800, transp=0)
    /// </summary>
    public class NATR
    {
        public static double[] Calculate(double[] close, double[] high, double[] low, int period, string maType = "sma")
        {
            Condition.Requires(close, "close").IsNotEmpty();
            Condition.Requires(high, "high").IsNotEmpty();
            Condition.Requires(low, "low").IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(close.Length);


            var atr = TA.ATR.Calculate(close, high, low, period, maType);
            var natr = new double[close.Length];

            for (int i = 0; i < close.Length; i++)
                natr[i] = atr[i] / close[i] * 100.00;

            return natr;
        }
    }
}
