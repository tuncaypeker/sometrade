using CuttingEdge.Conditions;

namespace SomeTrade.TA.Indicators
{
    /// <summary>
    /// Typical Price
    /// </summary>
    public class TypPrice
    {
        /// <summary>
        /// The Typical Price indicator provides a simple, single-line plot of the day’s average price. Some investors use the Typical Price rather than the closing price
        /// when creating moving-average penetration systems.
        /// </summary>
        /// <param name="inHigh"></param>
        /// <param name="inLow"></param>
        /// <param name="inClose"></param>
        /// <returns></returns>
        public static double[] Calculate(double[] inHigh, double[] inLow, double[] inClose)
        {
            Condition.Requires(inHigh, "inHigh").IsNotNull().IsNotEmpty();
            Condition.Requires(inLow, "inLow").IsNotNull().IsNotEmpty();
            Condition.Requires(inClose, "inClose").IsNotNull().IsNotEmpty();

            var result = new double[inClose.Length];
            for (int i = 0; i < inClose.Length; i++)
                result[i] = (inHigh[i] + inLow[i] + inClose[i]) / 3.0;

            return result;
        }
    }
}
