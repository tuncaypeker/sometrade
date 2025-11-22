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
    /*
        //@version=5
indicator(title="Triple EMA", shorttitle="TEMA", overlay=true, timeframe="", timeframe_gaps=true)
length = input.int(9, minval=1)
ema1 = ta.ema(close, length)
ema2 = ta.ema(ema1, length)
ema3 = ta.ema(ema2, length)
out = 3 * (ema1 - ema2) + ema3
plot(out, "TEMA", color=#2962FF)
     */ 
    /// </summary>
    public class TEMA
    {
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var ema1 = TA.EMA.Calculate(price, period);
            var ema2 = TA.EMA.Calculate(ema1, period);
            var ema3 = TA.EMA.Calculate(ema2, period);

            var result = new double[price.Length];
            for (int i = 0; i < price.Length; i++)
                result[i] = (3.0 * ema1[i]) - (3.0 * ema2[i]) + ema3[i];

            return result;
        }
    }
}
