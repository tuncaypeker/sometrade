using SomeTrade.Candles.Common;
using static System.Math;

namespace SomeTrade.Candles
{
    public class CandleHikkake : CandleIndicator
    {
        public CandleHikkake(in double[] open, in double[] high, in double[] low, in double[] close)
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
            int patternIdx = 0;
            int patternResult = 0;
            
            int i = startIdx - 3;
            while (i < startIdx)
            {
                if (GetPatternRecognition(i))
                {
                    patternResult = 100 * (_high[i] < _high[i - 1] ? 1 : -1);
                    patternIdx = i;
                }
                // search for confirmation if hikkake was no more than 3 bars ago
                else if (GetPatternConfirmation(i, patternIdx, patternResult))
                {
                    patternIdx = 0;
                }

                i++;
            }

            i = startIdx;
            
            /* Proceed with the calculation for the requested range.
             * Must have:
             * - first and second candle: inside bar (2nd has lower high and higher low than 1st)
             * - third candle: lower high and lower low than 2nd (higher high and higher low than 2nd)
             * outInteger[hikkakebar] is positive (1 to 100) or negative (-1 to -100) meaning bullish or bearish hikkake
             * Confirmation could come in the next 3 days with:
             * - a day that closes higher than the high (lower than the low) of the 2nd candle
             * outInteger[confirmationbar] is equal to 100 + the bullish hikkake result or -100 - the bearish hikkake result
             * Note: if confirmation and a new hikkake come at the same bar, only the new hikkake is reported (the new hikkake
             * overwrites the confirmation of the old hikkake)
             */
            int outIdx = 0;
            do
            {
                if (GetPatternRecognition(i))
                {
                    patternResult = 100 * (_high[i] < _high[i - 1] ? 1 : -1);
                    patternIdx = i;
                    outInteger[outIdx++] = patternResult;
                }
                else
                {
                    /* search for confirmation if hikkake was no more than 3 bars ago */
                    if (GetPatternConfirmation(i, patternIdx, patternResult))
                    {
                        outInteger[outIdx++] = patternResult + 100 * (patternResult > 0 ? 1 : -1);
                        patternIdx = 0;
                    }
                    else
                    {
                        outInteger[outIdx++] = 0;
                    }
                }

                i++;
            } while (i <= endIdx);

            // All done. Indicate the output limits and return.
            outNBElement = outIdx;
            outBegIdx = startIdx;
            
            return new CandleResult(RetCode.Success, outBegIdx, outNBElement, outInteger);
        }

        public override bool GetPatternRecognition(int i)
        {
            bool patternRecognition =
                // 1st + 2nd: lower high and higher low
                _high[i - 1] < _high[i - 2] && _low[i - 1] > _low[i - 2] &&
                (
                    // (bull) 3rd: lower high and lower low
                    (_high[i] < _high[i - 1] && _low[i] < _low[i - 1]) ||
                    // (bear) 3rd: higher high and higher low
                    (_high[i] > _high[i - 1] && _low[i] > _low[i - 1])
                );
            
            return patternRecognition;
        }

        private bool GetPatternConfirmation(int i, int patternIdx, int patternResult)
        {
            bool patternConfirmation =
                i <= patternIdx + 3 &&
                (
                    // close higher than the high of 2nd
                    (patternResult > 0 && _close[i] > _high[patternIdx - 1]) ||
                    // close lower than the low of 2nd
                    (patternResult < 0 && _close[i] < _low[patternIdx - 1])
                );
            
            return patternConfirmation;
        }

        public override int GetLookback()
        {
            return 5;
        }
    }
}
