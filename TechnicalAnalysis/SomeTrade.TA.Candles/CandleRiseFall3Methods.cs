using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleRiseFall3Methods : CandleIndicator
    {
        private readonly double[] _bodyPeriodTotal = new double[5];

        public CandleRiseFall3Methods(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int bodyShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyShort);
            int bodyLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            
            int i = bodyShortTrailingIdx;
            while (i < startIdx)
            {
                _bodyPeriodTotal[3] += GetCandleRange(CandleSettingType.BodyShort, i - 3);
                _bodyPeriodTotal[2] += GetCandleRange(CandleSettingType.BodyShort, i - 2);
                _bodyPeriodTotal[1] += GetCandleRange(CandleSettingType.BodyShort, i - 1);
                i++;
            }

            i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyPeriodTotal[4] += GetCandleRange(CandleSettingType.BodyLong, i - 4);
                _bodyPeriodTotal[0] += GetCandleRange(CandleSettingType.BodyLong, i);
                i++;
            }

            i = startIdx;

            /* Proceed with the calculation for the requested range.
             * Must have:
             * - first candle: long white (black) candlestick
             * - then: group of falling (rising) small real body candlesticks (commonly black (white)) that hold within 
             *   the prior long candle's range: ideally they should be three but two or more than three are ok too
             * - final candle: long white (black) candle that opens above (below) the previous small candle's close 
             *   and closes above (below) the first long candle's close
             * The meaning of "short" and "long" is specified with TA_SetCandleSettings; here only patterns with 3 small candles
             * are considered;
             * outInteger is positive (1 to 100) or negative (-1 to -100)
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? 100 * GetCandleColor(i - 4) : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                _bodyPeriodTotal[4] +=
                    GetCandleRange(CandleSettingType.BodyLong, i - 4) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx - 4);

                for (int totIdx = 3; totIdx >= 1; --totIdx)
                {
                    _bodyPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.BodyShort, i - totIdx) -
                        GetCandleRange(CandleSettingType.BodyShort, bodyShortTrailingIdx - totIdx);
                }

                _bodyPeriodTotal[0] +=
                    GetCandleRange(CandleSettingType.BodyLong, i) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx);

                i++;
                bodyShortTrailingIdx++;
                bodyLongTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isRiseFall3Methods =
                // 1st long, then 3 small, 5th long
                GetRealBody(i - 4) > GetCandleAverage(CandleSettingType.BodyLong, _bodyPeriodTotal[4], i - 4) &&
                GetRealBody(i - 3) < GetCandleAverage(CandleSettingType.BodyShort, _bodyPeriodTotal[3], i - 3) &&
                GetRealBody(i - 2) < GetCandleAverage(CandleSettingType.BodyShort, _bodyPeriodTotal[2], i - 2) &&
                GetRealBody(i - 1) < GetCandleAverage(CandleSettingType.BodyShort, _bodyPeriodTotal[1], i - 1) &&
                GetRealBody(i) > GetCandleAverage(CandleSettingType.BodyLong, _bodyPeriodTotal[0], i) &&
                // white, 3 black, white  ||  black, 3 white, black
                GetCandleColor(i - 4) == -GetCandleColor(i - 3) &&
                GetCandleColor(i - 3) == GetCandleColor(i - 2) &&
                GetCandleColor(i - 2) == GetCandleColor(i - 1) &&
                GetCandleColor(i - 1) == -GetCandleColor(i) &&
                // 2nd to 4th hold within 1st: a part of the real body must be within 1st range
                Min(_open[i - 3], _close[i - 3]) < _high[i - 4] &&
                Max(_open[i - 3], _close[i - 3]) > _low[i - 4] &&
                Min(_open[i - 2], _close[i - 2]) < _high[i - 4] &&
                Max(_open[i - 2], _close[i - 2]) > _low[i - 4] &&
                Min(_open[i - 1], _close[i - 1]) < _high[i - 4] &&
                Max(_open[i - 1], _close[i - 1]) > _low[i - 4] &&
                // 2nd to 4th are falling (rising)
                _close[i - 2] * GetCandleColor(i - 4) < _close[i - 3] * GetCandleColor(i - 4) &&
                _close[i - 1] * GetCandleColor(i - 4) < _close[i - 2] * GetCandleColor(i - 4) &&
                // 5th opens above (below) the prior close
                _open[i] * GetCandleColor(i - 4) > _close[i - 1] * GetCandleColor(i - 4) &&
                // 5th closes above (below) the 1st close
                _close[i] * GetCandleColor(i - 4) > _close[i - 4] * GetCandleColor(i - 4);
            
            return isRiseFall3Methods;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyShort, CandleSettingType.BodyLong) + 4;
        }
    }
}
