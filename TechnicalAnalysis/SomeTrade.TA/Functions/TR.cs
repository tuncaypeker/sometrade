namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// https://www.linnsoft.com/techind/true-range-tr
    /// Welles Wilder described these calculations to determine the trading range for a stock or commodity. True Range is defined as the largest of the following:
    /// 
    /// The distance from today's high to today's low.
    /// The distance from yesterday's close to today's high.
    /// The distance from yesterday's close to today's low.
    /// Wilder included price comparisons among subsequent bars in order to account for gaps in his range calculation.
    /// </summary>
    public class TR
    {
        public static double[] Execute(double[] inHigh, double[] inLow, double[] inClose)
        {
            Condition.Requires(inHigh, "inHigh").IsNotEmpty();
            Condition.Requires(inLow, "inLow").IsNotEmpty();
            Condition.Requires(inClose, "inClose").IsNotEmpty();

            int startIdx = 0;
            int endIdx = inHigh.Length - 1;

            //validation
            var result = new double[inHigh.Length];

            if (startIdx < 1)
                startIdx = 1;

            Condition.Requires(startIdx, "startIdx").IsLessThan(endIdx);

            int outIdx = 1;
            int today = startIdx;
            while (true)
            {
                if (today > endIdx)
                    break;

                double lowCurrent = inLow[today];
                double highCurrent = inHigh[today];
                double closePrevious = inClose[today - 1];

                double greatest = highCurrent - lowCurrent;
                double val2 = System.Math.Abs(closePrevious - highCurrent);
                if (val2 > greatest)
                    greatest = val2;

                double val3 = System.Math.Abs(closePrevious - lowCurrent);
                if (val3 > greatest)
                    greatest = val3;

                result[outIdx] = greatest;
                outIdx++;
                today++;
            }

            return result;
        }
    }
}
