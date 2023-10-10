namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    
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
    public class FIR
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="weights">Indicator weights.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, double[] weights)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(weights, "weights")
                .IsNotNull()
                .IsNotEmpty();

            var fir = new double[price.Length];
            var divider = 0.0;

            for (int i = 0; i < weights.Length; ++i)
            {
                fir[i] = 0;
                divider += weights[i];
            }

            for (int i = weights.Length; i < price.Length; ++i)
            {
                var sum = 0.0;
                for (int w = 0; w < weights.Length; w++)
                    sum += weights[w] * price[i - w];

                fir[i] = sum / divider;
            }

            return fir;
        }
    }
}
