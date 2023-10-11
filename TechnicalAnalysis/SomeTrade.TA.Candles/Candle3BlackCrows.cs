using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class Candle3BlackCrows : CandleIndicator
    {
        private readonly double[] _shadowVeryShortPeriodTotal = new double[3];

        public Candle3BlackCrows(in double[] open, in double[] high, in double[] low, in double[] close)
            : base(open, high, low, close)
        {
        }

        public CandleResult Compute(int startIdx, int endIdx)
        {
            // Initialize output variables 
            int outBegIdx = default;
            int outNBElement = default;
            int[] outInteger = new int[endIdx - startIdx + 1];

            // Validate the requested output range.
            if (startIdx < 0)
                return new CandleResult(RetCode.OutOfRangeStartIndex, outBegIdx, outNBElement, outInteger);

            if (endIdx < 0 || endIdx < startIdx)
                return new CandleResult(RetCode.OutOfRangeEndIndex, outBegIdx, outNBElement, outInteger);

            // Verify required price component.
            if (_open == null || _high == null || _low == null || _close == null)
                return new CandleResult(RetCode.BadParam, outBegIdx, outNBElement, outInteger);

            // Identify the minimum number of price bar needed to calculate at least one output.
            int lookbackTotal = GetLookback();
            
            // Move up the start index if there is not enough initial data.
            if (startIdx < lookbackTotal)
                startIdx = lookbackTotal;

            // Make sure there is still something to evaluate.
            if (startIdx > endIdx)
                return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);

            // Do the calculation using tight loops.
            // Add-up the initial period, except for the last value.
            int shadowVeryShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort);
            
            int i = shadowVeryShortTrailingIdx;
            while (i < startIdx)
            {
                _shadowVeryShortPeriodTotal[2] = GetCandleRange(CandleSettingType.ShadowVeryShort, i - 2);
                _shadowVeryShortPeriodTotal[1] = GetCandleRange(CandleSettingType.ShadowVeryShort, i - 1);
                _shadowVeryShortPeriodTotal[0] = GetCandleRange(CandleSettingType.ShadowVeryShort, i);
                i++;
            }
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - three consecutive and declining black candlesticks
             * - each candle must have no or very short lower shadow
             * - each candle after the first must open within the prior candle's real body
             * - the first candle's close should be under the prior white candle's high
             * The meaning of "very short" is specified with TA_SetCandleSettings
             * outInteger is negative (-1 to -100): three black crows is always bearish; 
             * the user should consider that 3 black crows is significant when it appears after a mature advance or at high levels, 
             * while this function does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? -100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                for (int totIdx = 2; totIdx >= 0; --totIdx)
                {
                    _shadowVeryShortPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.ShadowVeryShort, i - totIdx) -
                        GetCandleRange(CandleSettingType.ShadowVeryShort, shadowVeryShortTrailingIdx - totIdx);
                }

                i++;
                shadowVeryShortTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.BadParam, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool is3BlackCrows =
                // white
                GetCandleColor(i - 3) == 1 &&
                // 1st black
                GetCandleColor(i - 2) == -1 &&
                // very short lower shadow
                GetLowerShadow(i - 2) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal[2], i - 2) &&
                // 2nd black
                GetCandleColor(i - 1) == -1 &&
                // very short lower shadow
                GetLowerShadow(i - 1) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal[1], i - 1) &&
                // 3rd black
                GetCandleColor(i) == -1 &&
                // very short lower shadow
                GetLowerShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal[0], i) &&
                // 2nd black opens within 1st black's rb
                _open[i - 1] < _open[i - 2] &&
                _open[i - 1] > _close[i - 2] &&
                // 3rd black opens within 2nd black's rb
                _open[i] < _open[i - 1] &&
                _open[i] > _close[i - 1] &&
                // 1st black closes under prior candle's high
                _high[i - 3] > _close[i - 2] &&
                // three declining
                _close[i - 2] > _close[i - 1] &&
                // three declining
                _close[i - 1] > _close[i];
            
            return is3BlackCrows;
        }

        public override int GetLookback()
        {
            return GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort) + 3;
        }
    }
}
