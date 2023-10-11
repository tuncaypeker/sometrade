namespace SomeTrade.TA.Indicators.Functions.KivancOzbilgic
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;

    /// <summary>
    /// name:
    ///     Tillson t3'ün Kývanç versiyonu
    /// description:
    /// 
    /// pinescript:
    ///     //@version=4
    ///      //Developed by Tim Tillson
    ///      //author: KIVANÇ @fr3762 on twitter
    ///      study("Tillson T3", overlay=true)
    ///      T3FiboLine = input(false, title="Show T3 Fibonacci Ratio Line?")
    ///
    ///
    ///      length1 = input(8, "T3 Length")
    ///      a1 = input(0.7, "Volume Factor")
    ///
    ///      e1 = ema((high + low + 2 * close) / 4, length1)
    ///      e2 = ema(e1, length1)
    ///      e3 = ema(e2, length1)
    ///      e4 = ema(e3, length1)
    ///      e5 = ema(e4, length1)
    ///      e6 = ema(e5, length1)
    ///      c1 = -a1 * a1 * a1
    ///      c2 = 3 * a1 * a1 + 3 * a1 * a1 * a1
    ///      c3 = -6 * a1 * a1 - 3 * a1 - 3 * a1 * a1 * a1
    ///      c4 = 1 + 3 * a1 + a1 * a1 * a1 + 3 * a1 * a1
    ///      T3 = c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3
    ///
    ///      col1 = T3 > T3[1]
    ///      col3 = T3 < T3[1]
    ///      color_1 = col1 ? color.green : col3 ? color.red : color.yellow
    ///      plot(T3, color=color_1, linewidth=3, title="T3")
    ///
    ///      length12 = input(5, "T3 Length fibo")
    ///      a12 = input(0.618, "Volume Factor fibo")
    ///
    ///      e12 = ema((high + low + 2 * close) / 4, length12)
    ///      e22 = ema(e12, length12)
    ///      e32 = ema(e22, length12)
    ///      e42 = ema(e32, length12)
    ///      e52 = ema(e42, length12)
    ///      e62 = ema(e52, length12)
    ///      c12 = -a12 * a12 * a12
    ///      c22 = 3 * a12 * a12 + 3 * a12 * a12 * a12
    ///      c32 = -6 * a12 * a12 - 3 * a12 - 3 * a12 * a12 * a12
    ///      c42 = 1 + 3 * a12 + a12 * a12 * a12 + 3 * a12 * a12
    ///      T32 = c12 * e62 + c22 * e52 + c32 * e42 + c42 * e32
    ///
    ///      col12 = T32 > T32[1]
    ///      col32 = T32 < T32[1]
    ///      color2 = col12 ? color.blue : col32 ? color.purple : color.yellow
    ///      plot(T3FiboLine and T32 ? T32 : na, color=color2, linewidth=2, title="T3fibo")
    ///
    ///      alertcondition(crossover(T3, T3[1]), title="T3 BUY", message="T3 BUY!")
    ///      alertcondition(crossunder(T3, T3[1]), title="T3 SELL", message="T3 SELL!")
    ///
    ///      alertcondition(cross(T3, T3[1]), title="Color ALARM", message="T3 has changed color!")
    /// </summary>
    public class T3K
    {
        public static SuperTrendResultDto Execute(double[] close, double[] high, double[] low, int period, double vFactor)
        {
            Condition.Requires(close, "close").IsNotNull().IsNotEmpty();
            Condition.Requires(high, "high").IsNotNull().IsNotEmpty();
            Condition.Requires(low, "low").IsNotNull().IsNotEmpty();
            Condition.Requires(vFactor, "vFactor").IsGreaterThan(0.0).IsLessThan(1.0);

            var ema1Input = new double[close.Length];
            for (int i = 0; i < close.Length; i++)
                ema1Input[i] = (high[i] + close[i] + (2 * close[i])) / 4;

            var ema1 = TA.EMA.Calculate(ema1Input, period);
            var ema2 = TA.EMA.Calculate(ema1, period);
            var ema3 = TA.EMA.Calculate(ema2, period);
            var ema4 = TA.EMA.Calculate(ema3, period);
            var ema5 = TA.EMA.Calculate(ema4, period);
            var ema6 = TA.EMA.Calculate(ema5, period);

            var c1 = -vFactor * vFactor * vFactor;
            var c2 = 3 * vFactor * vFactor + 3 * vFactor * vFactor * vFactor;
            var c3 = -6 * vFactor * vFactor - 3 * vFactor - 3 * vFactor * vFactor * vFactor;
            var c4 = 1 + 3 * vFactor + vFactor * vFactor * vFactor + 3 * vFactor * vFactor;

            var result = new SuperTrendResultDto() {
                Color = new string[high.Length],
                Result = new double[high.Length]
            };

            for (int i = 1; i < close.Length; i++)
            {
                result.Result[i] = c1 * ema6[i] + c2 * ema5[i] + c3 * ema4[i] + c4 * ema3[i];

                var con1 = result.Result[i] > result.Result[i - 1];
                var con3 = result.Result[i] < result.Result[i - 1];

                result.Color[i] = con1
                    ? "green"
                    : con3
                        ? "red"
                        : "yellow";
            }

            return result;
        }
    }
}
