using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleBreakaway : CandleIndicator
    {
        private double _bodyLongPeriodTotal;

        public CandleBreakaway(in double[] open, in double[] high, in double[] low, in double[] close)
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
            
            int i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, i - 4);
                i++;
            }

            i = startIdx;
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - first candle: long black (white)
             * - second candle: black (white) day whose body gaps down (up)
             * - third candle: black or white day with lower (higher) high and lower (higher) low than prior candle's
             * - fourth candle: black (white) day with lower (higher) high and lower (higher) low than prior candle's
             * - fifth candle: white (black) day that closes inside the gap, erasing the prior 3 days
             * The meaning of "long" is specified with TA_SetCandleSettings
             * outInteger is positive (1 to 100) when bullish or negative (-1 to -100) when bearish;
             * the user should consider that breakaway is significant in a trend opposite to the last candle, while this 
             * function does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? GetCandleColor(i) * 100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyLong, i - 4) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx - 4);

                i++;
                bodyLongTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isBreakaway =
                // 1st long
                GetRealBody(i - 4) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, i - 4) &&
                // 1st, 2nd, 4th same color, 5th opposite
                GetCandleColor(i - 4) == GetCandleColor(i - 3) &&
                GetCandleColor(i - 3) == GetCandleColor(i - 1) &&
                GetCandleColor(i - 1) == -GetCandleColor(i) &&
                (
                    (
                        // when 1st is black:
                        GetCandleColor(i - 4) == -1 &&
                        // 2nd gaps down
                        GetRealBodyGapDown(i - 3, i - 4) &&
                        // 3rd has lower high and low than 2nd
                        _high[i - 2] < _high[i - 3] &&
                        _low[i - 2] < _low[i - 3] &&
                        // 4th has lower high and low than 3rd
                        _high[i - 1] < _high[i - 2] &&
                        _low[i - 1] < _low[i - 2] &&
                        // 5th closes inside the gap
                        _close[i] > _open[i - 3] &&
                        _close[i] < _close[i - 4]
                    )
                    ||
                    (
                        // when 1st is white:
                        GetCandleColor(i - 4) == 1 &&
                        // 2nd gaps up
                        GetRealBodyGapUp(i - 3, i - 4) &&
                        // 3rd has higher high and low than 2nd
                        _high[i - 2] > _high[i - 3] &&
                        _low[i - 2] > _low[i - 3] &&
                        // 4th has higher high and low than 3rd
                        _high[i - 1] > _high[i - 2] &&
                        _low[i - 1] > _low[i - 2] &&
                        // 5th closes inside the gap
                        _close[i] < _open[i - 3] &&
                        _close[i] > _close[i - 4]
                    )
                );
            
            return isBreakaway;
        }

        public override int GetLookback()
        {
            return GetCandleAvgPeriod(CandleSettingType.BodyLong) + 4;
        }
    }
}
