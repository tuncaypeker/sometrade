namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Rate of Change (ROC)
    /// The Rate-of-Change (ROC) indicator, which is also referred to as simply Momentum, is a pure momentum oscillator. 
    /// The ROC calculation compares the current price with the price "n" periods ago.
    /// pinescript:
    /*
    //@version=5
    indicator(title="Rate Of Change", shorttitle="ROC", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
    length = input.int(9, minval=1)
    source = input(close, "Source")
    roc = 100 * (source - source[length])/source[length]
    plot(roc, color=#2962FF, title="ROC")
    hline(0, color=#787B86, title="Zero Line") 

     */ 
    /// </summary>
    public class ROC
    {
        public static double[] Execute(double[] inReal, int period)
        {
            Condition.Requires(inReal, "price").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(inReal.Length);

            var result = new double[inReal.Length];
            int endIdx = inReal.Length - 1;
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

                double tempReal = inReal[trailingIdx];
                trailingIdx++;
                result[outIdx] = tempReal != 0.0
                    ? (inReal[inIdx] / tempReal - 1.0) * 100.0
                    : 0.0;

                outIdx++;
                inIdx++;
            }

            return result;
        }
    }
}
