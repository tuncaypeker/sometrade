using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleLongLine : CandleIndicator
    {
        private double _bodyPeriodTotal;
        private double _shadowPeriodTotal;

        public CandleLongLine(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int bodyTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            int shadowTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowShort);
            
            int i = bodyTrailingIdx;
            while (i < startIdx)
            {
                _bodyPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, i);
                i++;
            }

            i = shadowTrailingIdx;
            while (i < startIdx)
            {
                _shadowPeriodTotal += GetCandleRange(CandleSettingType.ShadowShort, i);
                i++;
            }

            /* Proceed with the calculation for the requested range.
             * Must have:
             * - long real body
             * - short upper and lower shadow
             * The meaning of "long" and "short" is specified with TA_SetCandleSettings
             * outInteger is positive (1 to 100) when white (bullish), negative (-1 to -100) when black (bearish)
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? GetCandleColor(i) * 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyLong, i) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyTrailingIdx);

                _shadowPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowShort, i) -
                    GetCandleRange(CandleSettingType.ShadowShort, shadowTrailingIdx);

                i++;
                bodyTrailingIdx++;
                shadowTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isLongLine =
                GetRealBody(i) > GetCandleAverage(CandleSettingType.BodyLong, _bodyPeriodTotal, i) &&
                GetUpperShadow(i) < GetCandleAverage(CandleSettingType.ShadowShort, _shadowPeriodTotal, i) &&
                GetLowerShadow(i) < GetCandleAverage(CandleSettingType.ShadowShort, _shadowPeriodTotal, i);
            
            return isLongLine;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyLong, CandleSettingType.ShadowShort);
        }
    }
}
