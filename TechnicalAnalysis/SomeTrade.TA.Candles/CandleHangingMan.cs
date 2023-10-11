using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleHangingMan : CandleIndicator
    {
        private double _bodyPeriodTotal;
        private double _shadowLongPeriodTotal;
        private double _shadowVeryShortPeriodTotal;
        private double _nearPeriodTotal;

        public CandleHangingMan(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int nearTrailingIdx = startIdx - 1 - GetCandleAvgPeriod(CandleSettingType.Near);
            
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

            i = nearTrailingIdx;
            while (i < startIdx - 1)
            {
                _nearPeriodTotal += GetCandleRange(CandleSettingType.Near, i);
                i++;
            }

            i = startIdx;
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - small real body
             * - long lower shadow
             * - no, or very short, upper shadow
             * - body above or near the highs of the previous candle
             * The meaning of "short", "long" and "near the highs" is specified with TA_SetCandleSettings;
             * outInteger is negative (-1 to -100): hanging man is always bearish;
             * the user should consider that a hanging man must appear in an uptrend, while this function does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? -100 : 0;

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

                _nearPeriodTotal +=
                    GetCandleRange(CandleSettingType.Near, i - 1) -
                    GetCandleRange(CandleSettingType.Near, nearTrailingIdx);

                i++;
                bodyTrailingIdx++;
                shadowLongTrailingIdx++;
                shadowVeryShortTrailingIdx++;
                nearTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isHangingMan =
                // small rb
                GetRealBody(i) < GetCandleAverage(CandleSettingType.BodyShort, _bodyPeriodTotal, i) &&
                // long lower shadow
                GetLowerShadow(i) > GetCandleAverage(CandleSettingType.ShadowLong, _shadowLongPeriodTotal, i) &&
                // very short upper shadow
                GetUpperShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i) &&
                // rb near the prior candle's highs
                Min(_close[i], _open[i]) >= _high[i - 1] - GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal, i - 1);
            
            return isHangingMan;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyShort, CandleSettingType.ShadowLong, CandleSettingType.ShadowVeryShort, CandleSettingType.Near) + 1;
        }
    }
}
