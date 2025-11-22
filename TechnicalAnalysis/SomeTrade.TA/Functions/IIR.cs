namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Infinite Impulse Response Moving Average Indicator.
    /// </summary>
    public class IIR
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();

            return FIR.Calculate(price, new double[] { 2, 4, 0, 0, -1 });
        }
    }
}
