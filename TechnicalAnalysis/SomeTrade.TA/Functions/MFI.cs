namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// Money Flow Index
    /// pinescript:
    /*
     //@version=5
    indicator(title="Money Flow Index", shorttitle="MFI", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
    length = input.int(title="Length", defval=14, minval=1, maxval=2000)
    src = hlc3
    mf = ta.mfi(src, length)
    plot(mf, "MF", color=#7E57C2)
    overbought=hline(80, title="Overbought", color=#787B86)
    hline(50, "Middle Band", color=color.new(#787B86, 50))
    oversold=hline(20, title="Oversold", color=#787B86)
    fill(overbought, oversold, color=color.rgb(126, 87, 194, 90), title="Background")
     */ 
    /// </summary>
    public class MFI
    {
        public static double[] Execute(double[] inHigh, double[] inLow, double[] inClose, double[] inVolume, int period)
        {
            Condition.Requires(inHigh, "inHigh").IsNotEmpty();
            Condition.Requires(inLow, "inLow").IsNotEmpty();
            Condition.Requires(inClose, "inClose").IsNotEmpty();
            Condition.Requires(inVolume, "inVolume").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(2).IsLessOrEqual(inHigh.Length);

            int mflow_Idx = 0;
            int maxIdx_mflow = 49;
            int startIdx = 0;
            int endIdx = inHigh.Length - 1;

            //validation
            var result = new double[inHigh.Length];
            double[] mflowNegative = new double[inHigh.Length];
            double[] mflowPositive = new double[inHigh.Length];

            maxIdx_mflow = period - 1;
            int lookbackTotal = period;
            if (startIdx < lookbackTotal)
                startIdx = lookbackTotal;

            if (startIdx <= endIdx)
            {
                double tempValue1;
                double tempValue2;
                int outIdx = period;
                int today = startIdx - lookbackTotal;
                double prevValue = (inHigh[today] + inLow[today] + inClose[today]) / 3.0;
                double posSumMF = 0.0;
                double negSumMF = 0.0;
                today++;
                for (int i = period; i > 0; i--)
                {
                    tempValue1 = (inHigh[today] + inLow[today] + inClose[today]) / 3.0;
                    tempValue2 = tempValue1 - prevValue;
                    prevValue = tempValue1;
                    tempValue1 *= inVolume[today];
                    today++;

                    if (tempValue2 < 0.0)
                    {
                        mflowNegative[i] = tempValue1;
                        negSumMF += tempValue1;
                        mflowPositive[i] = 0.0;
                    }
                    else if (tempValue2 > 0.0)
                    {
                        mflowPositive[i] = tempValue1;
                        posSumMF += tempValue1;
                        mflowNegative[i] = 0.0;
                    }
                    else
                    {
                        mflowPositive[mflow_Idx] = 0.0;
                        mflowNegative[mflow_Idx] = 0.0;
                    }

                    mflow_Idx++;
                    if (mflow_Idx > maxIdx_mflow)
                    {
                        mflow_Idx = 0;
                    }
                }

                if (today > startIdx)
                {
                    tempValue1 = posSumMF + negSumMF;
                    result[outIdx] = tempValue1 < 1.0
                         ? 0.0
                         : 100.0 * (posSumMF / tempValue1);

                    outIdx++;
                }
                else
                {
                    while (today < startIdx)
                    {
                        posSumMF -= mflowPositive[mflow_Idx];
                        negSumMF -= mflowNegative[mflow_Idx];
                        tempValue1 = (inHigh[today] + inLow[today] + inClose[today]) / 3.0;
                        tempValue2 = tempValue1 - prevValue;
                        prevValue = tempValue1;
                        tempValue1 *= inVolume[today];
                        today++;
                        if (tempValue2 < 0.0)
                        {
                            mflowNegative[mflow_Idx] = tempValue1;
                            negSumMF += tempValue1;
                            mflowPositive[mflow_Idx] = 0.0;
                        }
                        else if (tempValue2 > 0.0)
                        {
                            mflowPositive[mflow_Idx] = tempValue1;
                            posSumMF += tempValue1;
                            mflowNegative[mflow_Idx] = 0.0;
                        }
                        else
                        {
                            mflowPositive[mflow_Idx] = 0.0;
                            mflowNegative[mflow_Idx] = 0.0;
                        }

                        mflow_Idx++;
                        if (mflow_Idx > maxIdx_mflow)
                            mflow_Idx = 0;
                    }
                }

                while (today <= endIdx)
                {
                    posSumMF -= mflowPositive[mflow_Idx];
                    negSumMF -= mflowNegative[mflow_Idx];
                    tempValue1 = (inHigh[today] + inLow[today] + inClose[today]) / 3.0;
                    tempValue2 = tempValue1 - prevValue;
                    prevValue = tempValue1;
                    tempValue1 *= inVolume[today];
                    today++;
                    if (tempValue2 < 0.0)
                    {
                        mflowNegative[mflow_Idx] = tempValue1;
                        negSumMF += tempValue1;
                        mflowPositive[mflow_Idx] = 0.0;
                    }
                    else if (tempValue2 > 0.0)
                    {
                        mflowPositive[mflow_Idx] = tempValue1;
                        posSumMF += tempValue1;
                        mflowNegative[mflow_Idx] = 0.0;
                    }
                    else
                    {
                        mflowPositive[mflow_Idx] = 0.0;
                        mflowNegative[mflow_Idx] = 0.0;
                    }

                    tempValue1 = posSumMF + negSumMF;
                    result[outIdx] = tempValue1 < 1.0
                        ? 0.0
                        : 100.0 * (posSumMF / tempValue1);

                    outIdx++;

                    mflow_Idx++;
                    if (mflow_Idx > maxIdx_mflow)
                    {
                        mflow_Idx = 0;
                    }
                }
            }

            return result;
        }

        /*
        /// <summary>
        /// þu haline sadeleþtirilebilir
        /// </summary>
        /// <param name="inHigh"></param>
        /// <param name="inLow"></param>
        /// <param name="inClose"></param>
        /// <param name="inVolume"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private static double[] ExecuteX(double[] inHigh, double[] inLow, double[] inClose, double[] inVolume, int period)
        {
            Condition.Requires(inHigh, "inHigh").IsNotEmpty();
            Condition.Requires(inLow, "inLow").IsNotEmpty();
            Condition.Requires(inClose, "inClose").IsNotEmpty();
            Condition.Requires(inVolume, "inVolume").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(2).IsLessOrEqual(inHigh.Length);

            var typicalPrice = TypPrice.Calculate(inHigh, inLow, inClose);
            var result = new double[inHigh.Length];

            for (var i = period; i < inHigh.Length; i++)
            {
                var moneyFlow = typicalPrice[i] * inVolume[i];
                double positiveMoneyFlow = 0;
                double negativeMoneyFlow = 0;

                for (var j = i; j > i - period + 1 && j > 0; j--)
                {
                    var typicalPriceCurrent = typicalPrice[j];
                    var typicalPricePrevious = typicalPrice[j - 1];

                    if (typicalPriceCurrent > typicalPricePrevious)
                        positiveMoneyFlow += moneyFlow;
                    else if (typicalPricePrevious > typicalPriceCurrent)
                        negativeMoneyFlow += moneyFlow;
                }

                result[i] = 100 - 100 / (1 + positiveMoneyFlow / negativeMoneyFlow);
            }

            return result;
        }
        */
    }
}
