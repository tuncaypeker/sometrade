namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Commodity Channel Index
    /// </summary>
    public class CCI
    {
        public static double[] Calculate(double[] inHigh, double[] inLow, double[] inClose, int optInTimePeriod)
        {
            Condition.Requires(inHigh, "inHigh").IsNotEmpty();
            Condition.Requires(inLow, "inLow").IsNotEmpty();
            Condition.Requires(inClose, "inClose").IsNotEmpty();
            Condition.Requires(optInTimePeriod, "optInTimePeriod").IsGreaterThan(2).IsLessThan(100000);

            var result = new double[inHigh.Length];

            int circBufferIdx = 0;
            int startIdx = 0;
            int endIdx = inHigh.Length - 1;
            int lookbackTotal = optInTimePeriod - 1;
            if (startIdx < lookbackTotal)
                startIdx = lookbackTotal;

            if (startIdx > endIdx)
                return result;

            double[] circBuffer = new double[optInTimePeriod];

            int maxIdxCircBuffer = optInTimePeriod - 1;
            int i = startIdx - lookbackTotal;

            while (i < startIdx)
            {
                circBuffer[circBufferIdx] = (inHigh[i] + inLow[i] + inClose[i]) / 3.0;
                i++;
                circBufferIdx++;
                if (circBufferIdx > maxIdxCircBuffer)
                    circBufferIdx = 0;
            }

            int outIdx = optInTimePeriod - 1;
            do
            {
                double lastValue = (inHigh[i] + inLow[i] + inClose[i]) / 3.0;
                circBuffer[circBufferIdx] = lastValue;
                double theAverage = 0.0;
                int j = 0;
                while (j < optInTimePeriod)
                {
                    theAverage += circBuffer[j];
                    j++;
                }

                theAverage /= optInTimePeriod;
                double tempReal2 = 0.0;
                for (j = 0; j < optInTimePeriod; j++)
                {
                    tempReal2 += System.Math.Abs(circBuffer[j] - theAverage);
                }

                double tempReal = lastValue - theAverage;
                result[outIdx] = tempReal != 0.0 && tempReal2 != 0.0
                    ? tempReal / (0.015 * (tempReal2 / optInTimePeriod))
                    : 0.0;

                outIdx++;
                if (outIdx >= result.Length)
                    break;

                circBufferIdx++;
                if (circBufferIdx > maxIdxCircBuffer)
                    circBufferIdx = 0;

                i++;
            }
            while (i <= endIdx);

            return result;
        }
    }
}
