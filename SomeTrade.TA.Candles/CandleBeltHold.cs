using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleBeltHold : CandleIndicator
    {
        private double _bodyLongPeriodTotal;
        private double _shadowVeryShortPeriodTotal;

        public CandleBeltHold(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int shadowVeryShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowVeryShort);
            
            int i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, i);
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
             * - long white (black) real body
             * - no or very short lower (upper) shadow
             * The meaning of "long" and "very short" is specified with TA_SetCandleSettings
             * outInteger is positive (1 to 100) when white (bullish), negative (-1 to -100) when black (bearish)
             */
            int outIdx = 0;
            do
            {
                bool isBeltHold = GetPatternRecognition(i);

                outInteger[outIdx++] = isBeltHold ? GetCandleColor(i) * 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyLong, i) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx);

                _shadowVeryShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowVeryShort, i) -
                    GetCandleRange(CandleSettingType.ShadowVeryShort, shadowVeryShortTrailingIdx);

                i++;
                bodyLongTrailingIdx++;
                shadowVeryShortTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isBeltHold =
                // long body
                GetRealBody(i) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, i) &&
                (
                    ( // white body and very short lower shadow
                        GetCandleColor(i) == 1 &&
                        GetLowerShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i)
                    ) ||
                    ( // black body and very short upper shadow
                        GetCandleColor(i) == -1 &&
                        GetUpperShadow(i) < GetCandleAverage(CandleSettingType.ShadowVeryShort, _shadowVeryShortPeriodTotal, i)
                    )
                );
            
            return isBeltHold;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyLong, CandleSettingType.ShadowVeryShort);
        }
    }
}
