using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleAdvanceBlock : CandleIndicator
    {
        private readonly double[] _shadowShortPeriodTotal = new double[3];
        private readonly double[] _shadowLongPeriodTotal = new double[2];
        private readonly double[] _nearPeriodTotal = new double[3];
        private readonly double[] _farPeriodTotal = new double[3];
        private double _bodyLongPeriodTotal;

        public CandleAdvanceBlock(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int shadowShortTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowShort);
            int shadowLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.ShadowLong);
            int nearTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.Near);
            int farTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.Far);
            int bodyLongTrailingIdx = startIdx - GetCandleAvgPeriod(CandleSettingType.BodyLong);
            
            int i = shadowShortTrailingIdx;
            while (i < startIdx)
            {
                _shadowShortPeriodTotal[2] += GetCandleRange(CandleSettingType.ShadowShort, i - 2);
                _shadowShortPeriodTotal[1] += GetCandleRange(CandleSettingType.ShadowShort, i - 1);
                _shadowShortPeriodTotal[0] += GetCandleRange(CandleSettingType.ShadowShort, i);
                i++;
            }

            i = shadowLongTrailingIdx;
            while (i < startIdx)
            {
                _shadowLongPeriodTotal[1] += GetCandleRange(CandleSettingType.ShadowLong, i - 1);
                _shadowLongPeriodTotal[0] += GetCandleRange(CandleSettingType.ShadowLong, i);
                i++;
            }

            i = nearTrailingIdx;
            while (i < startIdx)
            {
                _nearPeriodTotal[2] += GetCandleRange(CandleSettingType.Near, i - 2);
                _nearPeriodTotal[1] += GetCandleRange(CandleSettingType.Near, i - 1);
                i++;
            }

            i = farTrailingIdx;
            while (i < startIdx)
            {
                _farPeriodTotal[2] += GetCandleRange(CandleSettingType.Far, i - 2);
                _farPeriodTotal[1] += GetCandleRange(CandleSettingType.Far, i - 1);
                i++;
            }

            i = bodyLongTrailingIdx;
            while (i < startIdx)
            {
                _bodyLongPeriodTotal += GetCandleRange(CandleSettingType.BodyLong, i - 2);
                i++;
            }

            i = startIdx;
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - three white candlesticks with consecutively higher closes
             * - each candle opens within or near the previous white real body 
             * - first candle: long white with no or very short upper shadow (a short shadow is accepted too for more flexibility)
             * - second and third candles, or only third candle, show signs of weakening: progressively smaller white real bodies 
             * and/or relatively long upper shadows; see below for specific conditions
             * The meanings of "long body", "short shadow", "far" and "near" are specified with TA_SetCandleSettings;
             * outInteger is negative (-1 to -100): advance block is always bearish;
             * the user should consider that advance block is significant when it appears in uptrend, while this function 
             * does not consider it
             */
            int outIdx = 0;
            do
            {
                outInteger[outIdx++] = GetPatternRecognition(i) ? -100 : 0;

                /* add the current range and subtract the first range: this is done after the pattern recognition 
                 * when avgPeriod is not 0, that means "compare with the previous candles" (it excludes the current candle)
                 */
                for (int totIdx = 2; totIdx >= 0; --totIdx)
                {
                    _shadowShortPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.ShadowShort, i - totIdx) -
                        GetCandleRange(CandleSettingType.ShadowShort, shadowShortTrailingIdx - totIdx);
                }

                for (int totIdx = 1; totIdx >= 0; --totIdx)
                {
                    _shadowLongPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.ShadowLong, i - totIdx) -
                        GetCandleRange(CandleSettingType.ShadowLong, shadowLongTrailingIdx - totIdx);
                }

                for (int totIdx = 2; totIdx >= 1; --totIdx)
                {
                    _farPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.Far, i - totIdx) -
                        GetCandleRange(CandleSettingType.Far, farTrailingIdx - totIdx);

                    _nearPeriodTotal[totIdx] +=
                        GetCandleRange(CandleSettingType.Near, i - totIdx) -
                        GetCandleRange(CandleSettingType.Near, nearTrailingIdx - totIdx);
                }

                _bodyLongPeriodTotal +=
                    GetCandleRange(CandleSettingType.BodyLong, i - 2) -
                    GetCandleRange(CandleSettingType.BodyLong, bodyLongTrailingIdx - 2);

                i++;
                shadowShortTrailingIdx++;
                shadowLongTrailingIdx++;
                nearTrailingIdx++;
                farTrailingIdx++;
                bodyLongTrailingIdx++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool isAdvanceBlock =
                // 1st white
                GetCandleColor(i - 2) == 1 &&
                // 2nd white
                GetCandleColor(i - 1) == 1 &&
                // 3rd white
                GetCandleColor(i) == 1 &&
                // consecutive higher closes
                _close[i] > _close[i - 1] && _close[i - 1] > _close[i - 2] &&
                // 2nd opens within/near 1st real body
                _open[i - 1] > _open[i - 2] &&
                _open[i - 1] <= _close[i - 2] + GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal[2], i - 2) &&
                // 3rd opens within/near 2nd real body
                _open[i] > _open[i - 1] &&
                _open[i] <= _close[i - 1] + GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal[1], i - 1) &&
                // 1st: long real body
                GetRealBody(i - 2) > GetCandleAverage(CandleSettingType.BodyLong, _bodyLongPeriodTotal, i - 2) &&
                // 1st: short upper shadow
                GetUpperShadow(i - 2) < GetCandleAverage(CandleSettingType.ShadowShort, _shadowShortPeriodTotal[2], i - 2) &&
                (
                    // ( 2 far smaller than 1 && 3 not longer than 2 )
                    // advance blocked with the 2nd, 3rd must not carry on the advance
                    (
                        GetRealBody(i - 1) < GetRealBody(i - 2) -
                        GetCandleAverage(CandleSettingType.Far, _farPeriodTotal[2], i - 2) &&
                        GetRealBody(i) < GetRealBody(i - 1) +
                        GetCandleAverage(CandleSettingType.Near, _nearPeriodTotal[1], i - 1)
                    ) ||
                    // 3 far smaller than 2
                    // advance blocked with the 3rd
                    (
                        GetRealBody(i) < GetRealBody(i - 1) -
                        GetCandleAverage(CandleSettingType.Far, _farPeriodTotal[1], i - 1)
                    ) ||
                    // ( 3 smaller than 2 && 2 smaller than 1 && (3 or 2 not short upper shadow) )
                    // advance blocked with progressively smaller real bodies and some upper shadows
                    (
                        GetRealBody(i) < GetRealBody(i - 1) &&
                        GetRealBody(i - 1) < GetRealBody(i - 2) &&
                        (
                            GetUpperShadow(i) >
                            GetCandleAverage(CandleSettingType.ShadowShort, _shadowShortPeriodTotal[0], i) ||
                            GetUpperShadow(i - 1) >
                            GetCandleAverage(CandleSettingType.ShadowShort, _shadowShortPeriodTotal[1], i - 1)
                        )
                    ) ||
                    // ( 3 smaller than 2 && 3 long upper shadow )
                    // advance blocked with 3rd candle's long upper shadow and smaller body
                    (
                        GetRealBody(i) < GetRealBody(i - 1) &&
                        GetUpperShadow(i) > GetCandleAverage(CandleSettingType.ShadowLong, _shadowLongPeriodTotal[0], i)
                    )
                );
            
            return isAdvanceBlock;
        }

        public override int GetLookback()
        {
            return GetCandleMaxAvgPeriod(CandleSettingType.ShadowLong, CandleSettingType.ShadowShort, CandleSettingType.Far, CandleSettingType.Near, CandleSettingType.BodyLong) + 2;
        }
    }
}
