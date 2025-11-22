using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleInvertedHammer : CandleIndicator
    {
        private double _bodyPeriodTotal;
        private double _shadowLongPeriodTotal;
        private double _shadowVeryShortPeriodTotal;

        public CandleInvertedHammer(in double[] open, in double[] high, in double[] low, in double[] close)
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
            {
                return new CandleResult(RetCode.OutOfRangeStartIndex, outBegIdx, outNBElement, outInteger);
            }

            if (endIdx < 0 || endIdx < startIdx)
            {
                return new CandleResult(RetCode.OutOfRangeEndIndex, outBegIdx, outNBElement, outInteger);
            }

            // Verify required price component.
            if (_open == null || _high == null || _low == null || _close == null)
            {
                return new CandleResult(RetCode.BadParam, outBegIdx, outNBElement, outInteger);
            }

            // Identify the minimum number of price bar needed to calculate at least one output.
            int lookbackTotal = GetLookback();

            // Move up the start index if there is not enough initial data.
            if (startIdx < lookbackTotal)
            {
                startIdx = lookbackTotal;
            }

            // Make sure there is still something to evaluate.
            if (startIdx > endIdx)
            {
                return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
            }

            // Do the calculation using tight loops.
            // Add-up the initial period, except for the last value.
            int bodyTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyShort);
            int shadowLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowLong);
            int shadowVeryShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort);
            
            int i = bodyTrailingIdx;
            while (i < startIdx)
            {
                _bodyPeriodTotal += GetCandleRange(CandleSettingType.BodyShort, i);
                i++;
            }

            i = shadowLongTrailingIdx;
            while (i < startIdx)
            {
                _shadowLongPeriodTotal += GetCandleRange(CandleSettingType.ShadowLong, i);
                i++;
            }

            i = shadowVeryShortTrailingIdx;
            while (i < startIdx)
            {
                _shadowVeryShortPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryShort, i);
                i++;
            }

            /* Proceed with the calculation for the requested range.
             * Must have:
             * - small real body
             * - long upper shadow
             * - no, or very short, lower shadow
             * - gap down
             * The meaning of "short", "very short" and "long" is specified with TA_SetCandleSettings;
             * outInteger is positive (1 to 100): inverted hammer is always bullish;
             * the user should consider that an inverted hammer must appear in a downtrend, while this function does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyShort, i) -
                    GetCandleRange(CandleSettingType.BodyShort, bodyTrailingIdx);

                _shadowLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowLong, i) -
                    GetCandleRange(CandleSettingType.ShadowLong, shadowLongTrailingIdx);

                _shadowVeryShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowVeryShort, i) -
                    GetCandleRange(CandleSettingType.ShadowVeryShort, shadowVeryShortTrailingIdx);

                i++;
                bodyTrailingIdx++;
                shadowLongTrailingIdx++;
                shadowVeryShortTrailingIdx++;
            } while (i <= endIdx);
            
            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isInvertedHammer =
                // small rb
                GetRealBody(i) < GetCandleAverage(CandleSettingType.BodyShort, _bodyPeriodTotal, i) &&
                // long upper shadow
                GetUpperShadow(i) > GetCandleAverage(CandleSettingType.ShadowLong, _shadowLongPeriodTotal, i) &&
                // very short lower shadow
                GetLowerShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i) &&
                // gap down
                GetRealBodyGapDown(i, i - 1);
            
            return isInvertedHammer;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyShort, CandleSettingType.ShadowLong, CandleSettingType.ShadowVeryShort) + 1;
        }
    }
}
