namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    /// <summary>
    /// pinescript:
    /*
    
    //@version=5
indicator(title="Momentum", shorttitle="Mom", timeframe="", timeframe_gaps=true)
len = input.int(10, minval=1, title="Length")
src = input(close, title="Source")
mom = src - src[len]
plot(mom, color=#2962FF, title="MOM")

     */ 
    /// </summary>
    public class Momentum
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var momentum = new double[price.Length];
            for (int i = period; i < price.Length; i++)
                momentum[i] = price[i] / price[i - period] * 100;

            return momentum;
        }

        public static double[] Calculate2(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var result = new double[price.Length];
            int endIdx = price.Length - 1;
            int startIdx = period;

            if (startIdx > endIdx)
                return result;

            int outIdx = period;
            int inIdx = startIdx;
            int trailingIdx = startIdx - period;

            while (true)
            {
                if (inIdx > endIdx)
                    break;

                result[outIdx] = price[inIdx] - price[trailingIdx];

                outIdx++;
                trailingIdx++;
                inIdx++;
            }

            return result;
        }
    }
}
