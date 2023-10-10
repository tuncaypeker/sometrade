using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleTakuri : CandleIndicator
    {
        private double _bodyDojiPeriodTotal;
        private double _shadowVeryShortPeriodTotal;
        private double _shadowVeryLongPeriodTotal;

        public CandleTakuri(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int bodyDojiTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyDoji);
            int shadowVeryShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort);
            int shadowVeryLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryLong);
            
            int i = bodyDojiTrailingIdx;
            while (i < startIdx)
            {
                _bodyDojiPeriodTotal += GetCandleRange(CandleSettingType.BodyDoji, i);
                i++;
            }

            i = shadowVeryShortTrailingIdx;
            while (i < startIdx)
            {
                _shadowVeryShortPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryShort, i);
                i++;
            }

            i = shadowVeryLongTrailingIdx;
            while (i < startIdx)
            {
                _shadowVeryLongPeriodTotal += GetCandleRange(CandleSettingType.ShadowVeryLong, i);
                i++;
            }

            /* Proceed with the calculation for the requested range.
             *
             * Must have:
             * - doji body
             * - open and close at the high of the day = no or very short upper shadow
             * - very long lower shadow
             * The meaning of "doji", "very short" and "very long" is specified with TA_SetCandleSettings
             * outInteger is always positive (1 to 100) but this does not mean it is bullish: takuri must be considered
             * relatively to the trend
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyDojiPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyDoji, i) -
                    GetCandleRange(CandleSettingType.BodyDoji, bodyDojiTrailingIdx);

                _shadowVeryShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowVeryShort, i) -
                    GetCandleRange(CandleSettingType.ShadowVeryShort, shadowVeryShortTrailingIdx);

                _shadowVeryLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowVeryLong, i) -
                    GetCandleRange(CandleSettingType.ShadowVeryLong, shadowVeryLongTrailingIdx);

                i++;
                bodyDojiTrailingIdx++;
                shadowVeryShortTrailingIdx++;
                shadowVeryLongTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isTakuri =
                GetRealBody(i) <= GetCandleAverage(CandleSettingType.BodyDoji, _bodyDojiPeriodTotal, i) &&
                GetUpperShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i) &&
                GetLowerShadow(i) > GetCandleAverage(CandleSettingType.ShadowVeryLong, _shadowVeryLongPeriodTotal, i);
            
            return isTakuri;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyDoji, CandleSettingType.ShadowVeryShort, CandleSettingType.ShadowVeryLong);
        }
    }
}
