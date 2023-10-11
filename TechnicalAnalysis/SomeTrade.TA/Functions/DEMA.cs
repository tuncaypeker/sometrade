namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Collections.Generic;
    using System.Linq;
    using System;

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
    /// 
    /*
        //@version=5
        indicator(title="Double EMA", shorttitle="DEMA", overlay=true, timeframe="", timeframe_gaps=true)
        length = input.int(9, minval=1)
        src = input(close, title="Source")
        e1 = ta.ema(src, length)
        e2 = ta.ema(e1, length)
        dema = 2 * e1 - e2
        plot(dema, "DEMA", color=#43A047)

     */

    /// 
    ///  kivanc- Hareketli Ortalamalarla Algo Trade:
    ///     2 * EMA(close) - EMA(EMA(close))
    ///     Daha agresif bir yapıya sahip
    /// </summary>
    public class DEMA
    {
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var ema1 = TA.EMA.Calculate(price, period);
            var ema2 = TA.EMA.Calculate(ema1, period);

            var result = new double[price.Length];
            for (int i = 0; i < price.Length; i++)
                result[i] = 2.0 * ema1[i] - ema2[i];

            return result;
        }

    }
}
