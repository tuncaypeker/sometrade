using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleRickshawMan : CandleIndicator
    {
        private double _bodyDojiPeriodTotal;
        private double _shadowLongPeriodTotal;
        private double _nearPeriodTotal;

        public CandleRickshawMan(in double[] open, in double[] high, in double[] low, in double[] close)
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
                new CandleResult(RetCode.BadParam, outBegIdx, outNBElement, outInteger);
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
            int shadowLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowLong);
            int nearTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.Near);
            
            int i = bodyDojiTrailingIdx;
            while (i < startIdx)
            {
                _bodyDojiPeriodTotal += GetCandleRange(CandleSettingType.BodyDoji, i);
                i++;
            }

            i = shadowLongTrailingIdx;
            while (i < startIdx)
            {
                _shadowLongPeriodTotal += GetCandleRange(CandleSettingType.ShadowLong, i);
                i++;
            }

            i = nearTrailingIdx;
            while (i < startIdx)
            {
                _nearPeriodTotal += GetCandleRange(CandleSettingType.Near, i);
                i++;
            }

            /* Proceed with the calculation for the requested range.
             *
             * Must have:
             * - doji body
             * - two long shadows
             * - body near the midpoint of the high-low range
             * The meaning of "doji" and "near" is specified with TA_SetCandleSettings
             * outInteger is always positive (1 to 100) but this does not mean it is bullish: rickshaw man shows uncertainty
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

                _shadowLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.ShadowLong, i) -
                    GetCandleRange(CandleSettingType.ShadowLong, shadowLongTrailingIdx);

                _nearPeriodTotal +=
                    GetCandleRange(CandleSettingType.Near, i) -
                    GetCandleRange(CandleSettingType.Near, nearTrailingIdx);

                i++;
                bodyDojiTrailingIdx++;
                shadowLongTrailingIdx++;
                nearTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isRickshawMan =
                // doji
                GetRealBody(i) <= GetCandleAverage(CandleSettingType.BodyDoji, _bodyDojiPeriodTotal, i) &&
                // long shadow
                GetLowerShadow(i) > GetCandleAverage(CandleSettingType.ShadowLong, _shadowLongPeriodTotal, i) &&
                // long shadow
                GetUpperShadow(i) > GetCandleAverage(CandleSettingType.ShadowLong, _shadowLongPeriodTotal, i) &&
                (
                    // body near midpoint
                    Min(_open[i], _close[i]) <= _low[i] + GetHighLowRange(i) / 2 + GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal, i) &&
                    Max(_open[i], _close[i]) >= _low[i] + GetHighLowRange(i) / 2 - GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal, i)
                );
            
            return isRickshawMan;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.BodyDoji, CandleSettingType.ShadowLong, CandleSettingType.Near);
        }
    }
}
