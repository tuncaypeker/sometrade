namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// On-Balance Volume (OBV)
    /// pinescript:
    /*
    
    //@version=5
indicator(title="On Balance Volume", shorttitle="OBV", format=format.volume, timeframe="", timeframe_gaps=true)
var cumVol = 0.
cumVol += nz(volume)
if barstate.islast and cumVol == 0
    runtime.error("No volume is provided by the data vendor.")
src = close
obv = ta.cum(math.sign(ta.change(src)) * volume)
plot(obv, color=#2962FF, title="OnBalanceVolume")

ma(source, length, type) =>
    switch type
        "SMA" => ta.sma(source, length)
        "EMA" => ta.ema(source, length)
        "SMMA (RMA)" => ta.rma(source, length)
        "WMA" => ta.wma(source, length)
        "VWMA" => ta.vwma(source, length)

typeMA = input.string(title = "Method", defval = "SMA", options=["SMA", "EMA", "SMMA (RMA)", "WMA", "VWMA"], group="Smoothing")
smoothingLength = input.int(title = "Length", defval = 5, minval = 1, maxval = 100, group="Smoothing")

smoothingLine = ma(obv, smoothingLength, typeMA)
plot(smoothingLine, title="Smoothing Line", color=#f37f20, display=display.none)


     */ 
    /// </summary>
    public class OBV
    {
        public static double[] Execute(double[] inReal, double[] inVolume)
        {
            Condition.Requires(inReal, "inReal").IsNotEmpty();
            Condition.Requires(inVolume, "inVolume").IsNotEmpty();

            var result = new double[inReal.Length];

            double prevOBV = 0;
            double prevReal = inReal[0];
            for (int i = 1; i <= inReal.Length - 1; i++)
            {
                double tempReal = inReal[i];
                if (tempReal > prevReal)
                    prevOBV += inVolume[i];
                else if (tempReal < prevReal)
                    prevOBV -= inVolume[i];

                result[i] = prevOBV;
                prevReal = tempReal;
            }

            return result;
        }
    }
}
