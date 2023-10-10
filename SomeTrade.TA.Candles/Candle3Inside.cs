using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class Candle3Inside : CandleIndicator
    {
        private double _bodyLongPeriodTotal;
        private double _bodyShortPeriodTotal;

        public Candle3Inside(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int bodyLongTrailingIdx = startIdx - 2 - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            int bodyShortTrailingIdx = startIdx - 1 - GetCandleAvgPeriod(CandleSettingType.BodyShort);
            
            int i = bodyLongTrailingIdx;
            while (i < startIdx - 2)
            {
                _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, i);
                i++;
            }

            i = bodyShortTrailingIdx;
            while (i < startIdx - 1)
            {
                _bodyShortPeriodTotal += GetCandleRange(CandleSettingType.BodyShort, i);
                i++;
            }

            i = startIdx;
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - first candle: long white (black) real body
             * - second candle: short real body totally engulfed by the first
             * - third candle: black (white) candle that closes lower (higher) than the first candle's open
             * The meaning of "short" and "long" is specified with TA_SetCandleSettings
             * outInteger is positive (1 to 100) for the three inside up or negative (-1 to -100) for the three inside down; 
             * the user should consider that a three inside up is significant when it appears in a downtrend and a three inside
             * down is significant when it appears in an uptrend, while this function does not consider the trend
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) 
                    ? -GetCandleColor(i - 2) * 100
                    : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyLong, i - 2) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx);

                _bodyShortPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyShort, i - 1) -
                    GetCandleRange(CandleSettingType.BodyShort, bodyShortTrailingIdx);

                i++;
                bodyLongTrailingIdx++;
                bodyShortTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool is3Inside =
                // 1st: long
                GetRealBody(i - 2) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, i - 2) &&
                // 2nd: short
                GetRealBody(i - 1) <= GetCandleAverage(CandleSettingType.BodyShort, _bodyShortPeriodTotal, i - 1) &&
                // engulfed by 1st
                Max(_close[i - 1], _open[i - 1]) < Max(_close[i - 2], _open[i - 2]) &&
                Min(_close[i - 1], _open[i - 1]) > Min(_close[i - 2], _open[i - 2]) &&
                (
                    ( // 3rd: opposite to 1st
                        GetCandleColor(i - 2) == 1 &&
                        GetCandleColor(i) == -1 &&
                        _close[i] < _open[i - 2]
                    ) ||
                    ( // and closing out
                        GetCandleColor(i - 2) == -1 &&
                        GetCandleColor(i) == 1 &&
                        _close[i] > _open[i - 2]
                    )
                );
            
            return is3Inside;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyShort, CandleSettingType.BodyLong) + 2;
        }
    }
}
