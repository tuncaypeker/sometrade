using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleCounterAttack : CandleIndicator
    {
        private readonly double[] _bodyLongPeriodTotal = new double[2];
        private double _equalPeriodTotal;

        public CandleCounterAttack(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int equalTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.Equal);
            int bodyLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            
            int i = equalTrailingIdx;
            while (i < startIdx)
            {
                _equalPeriodTotal += GetCandleRange(CandleSettingType.Equal, i - 1);
                i++;
            }

            i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyLongPeriodTotal[1] += GetCandleRange(CandleSettingType.BodyLong, i - 1);
                _bodyLongPeriodTotal[0] += GetCandleRange(CandleSettingType.BodyLong, i);
                i++;
            }

            i = startIdx;

            /* Proceed with the calculation for the requested range.
             * Must have:
             * - first candle: long black (white)
             * - second candle: long white (black) with close equal to the prior close
             * The meaning of "equal" and "long" is specified with TA_SetCandleSettings
             * outInteger is positive (1 to 100) when bullish or negative (-1 to -100) when bearish;
             * the user should consider that counterattack is significant in a trend, while this function does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? GetCandleColor(i) * 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _equalPeriodTotal +=
                    GetCandleRange(CandleSettingType.Equal, i - 1) -
                    GetCandleRange(CandleSettingType.Equal, equalTrailingIdx - 1);

                int totIdx;
                for (totIdx = 1; totIdx >= 0; --totIdx)
                {
                    _bodyLongPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.BodyLong, i - totIdx) -
                        GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx - totIdx);
                }

                i++;
                equalTrailingIdx++;
                bodyLongTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isCounterAttack =
                // opposite candles
                GetCandleColor(i - 1) == -GetCandleColor(i) &&
                // 1st long
                GetRealBody(i - 1) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal[1], i - 1) &&
                // 2nd long
                GetRealBody(i) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal[0], i) &&
                // equal closes
                _close[i] <= _close[i - 1] + GetCandleAverage(CandleSettingType.Equal, _equalPeriodTotal, i - 1) &&
                _close[i] >= _close[i - 1] - GetCandleAverage(CandleSettingType.Equal, _equalPeriodTotal, i - 1);
            
            return isCounterAttack;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.Equal, CandleSettingType.BodyLong) + 1;
        }
    }
}
