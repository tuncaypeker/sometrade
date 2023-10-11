using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleStalledPattern : CandleIndicator
    {
        private readonly double[] _bodyLongPeriodTotal = new double[3];
        private readonly double[] _nearPeriodTotal = new double[3];
        private double _bodyShortPeriodTotal;
        private double _shadowVeryShortPeriodTotal;

        public CandleStalledPattern(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int bodyLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            int bodyShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyShort);
            int shadowVeryShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort);
            int nearTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.Near);
            
            int i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyLongPeriodTotal[2] += GetCandleRange(CandleSettingType.BodyLong, i - 2);
                _bodyLongPeriodTotal[1] += GetCandleRange(CandleSettingType.BodyLong, i - 1);
                i++;
            }

            i = bodyShortTrailingIdx;
            while (i < startIdx)
            {
                _bodyShortPeriodTotal += GetCandleRange(CandleSettingType.BodyShort, i);
                i++;
            }

            i = shadowVeryShortTrailingIdx;
            while (i < startIdx)
            {
                _shadowVeryShortPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryShort, i - 1);
                i++;
            }

            i = nearTrailingIdx;
            while (i < startIdx)
            {
                _nearPeriodTotal[2] += GetCandleRange(CandleSettingType.Near, i - 2);
                _nearPeriodTotal[1] += GetCandleRange(CandleSettingType.Near, i - 1);
                i++;
            }

            i = startIdx;

            /* Proceed with the calculation for the requested range.
             * Must have:
             * - three white candlesticks with consecutively higher closes
             * - first candle: long white
             * - second candle: long white with no or very short upper shadow opening within or near the previous white real body
             * and closing higher than the prior candle
             * - third candle: small white that gaps away or "rides on the shoulder" of the prior long real body (= it's at 
             * the upper end of the prior real body)
             * The meanings of "long", "very short", "short", "near" are specified with TA_SetCandleSettings;
             * outInteger is negative (-1 to -100): stalled pattern is always bearish;
             * the user should consider that stalled pattern is significant when it appears in uptrend, while this function 
             * does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? -100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                for (int totIdx = 2; totIdx >= 1; --totIdx)
                {
                    _bodyLongPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.BodyLong, i - totIdx) -
                        GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx - totIdx);

                    _nearPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.Near, i - totIdx) -
                        GetCandleRange(CandleSettingType.Near, nearTrailingIdx - totIdx);
                }

                _bodyShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyShort, i) -
                    GetCandleRange(CandleSettingType.BodyShort, bodyShortTrailingIdx);

                _shadowVeryShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowVeryShort, i - 1) -
                    GetCandleRange(CandleSettingType.ShadowVeryShort, shadowVeryShortTrailingIdx - 1);

                i++;
                bodyLongTrailingIdx++;
                bodyShortTrailingIdx++;
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
            bool isStalledPattern =
                // 1st white
                GetCandleColor(i - 2) == 1 &&
                // 2nd white
                GetCandleColor(i - 1) == 1 &&
                // 3rd white
                GetCandleColor(i) == 1 &&
                // consecutive higher closes
                _close[i] > _close[i - 1] &&
                _close[i - 1] > _close[i - 2] &&
                // 1st: long real body
                GetRealBody(i - 2) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal[2], i - 2) &&
                // 2nd: long real body
                GetRealBody(i - 1) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal[1], i - 1) &&
                // very short upper shadow 
                GetUpperShadow(i - 1) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i - 1) &&
                // opens within/near 1st real body
                _open[i - 1] > _open[i - 2] &&
                _open[i - 1] <= _close[i - 2] + GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal[2], i - 2) &&
                // 3rd: small real body
                GetRealBody(i) < GetCandleAverage(CandleSettingType.BodyShort, _bodyShortPeriodTotal, i) &&
                // rides on the shoulder of 2nd real body
                _open[i] >= _close[i - 1] - GetRealBody(i) - GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal[1], i - 1);
            
            return isStalledPattern;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyLong, CandleSettingType.BodyShort, CandleSettingType.ShadowVeryShort, CandleSettingType.Near) + 2;
        }
    }
}
