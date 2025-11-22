namespace SomeTrade.TA.Indicators.Serkan
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;

    /// <summary>
    /// name:
    ///    Serkan Edge
    /// description:
    ///     
    /// link:
    ///    
    /// formula:
    ///     
    /// pinescript:
    ///    //@version=3
    ///     study(title="SerkanEdge", overlay=true)
    ///     src = input(defval=close, title="Source")
    ///     per = input(defval=100, minval=1, title="Sampling Period")
    ///     mult = input(defval=3.0, minval=0.1, title="Range Multiplier")
    ///
    ///     smoothrng(x, t, m)=>
    ///         wper      = (t*2) - 1
    ///         avrng     = ema(abs(x - x[1]), t)
    ///         smoothrng = ema(avrng, wper)*m
    ///         smoothrng
    ///     smrng = smoothrng(src, per, mult)
    ///
    ///     // Range Filter
    ///     rngfilt(x, r)=>
    ///         rngfilt  = x
    ///         rngfilt := x > nz(rngfilt[1]) ? ((x - r) < nz(rngfilt[1]) ? nz(rngfilt[1]) : (x - r)) : ((x + r) > nz(rngfilt[1]) ? nz(rngfilt[1]) : (x + r))
    ///         rngfilt
    ///     filt = rngfilt(src, smrng)
    ///
    ///     // Filter Direction
    ///     upward   = 0.0
    ///     upward  := filt > filt[1] ? nz(upward[1]) + 1 : filt < filt[1] ? 0 : nz(upward[1])
    ///     downward = 0.0
    ///     downward := filt < filt[1] ? nz(downward[1]) + 1 : filt > filt[1] ? 0 : nz(downward[1])
    ///
    ///     // Target Bands
    ///     hband = filt + smrng
    ///     lband = filt - smrng
    ///
    ///     // Colors
    ///     filtcolor = upward > 0 ? lime : downward > 0 ? red : orange
    ///     barcolor  = (src > filt) and (src > src[1]) and (upward > 0) ? lime : (src > filt) and (src < src[1]) and (upward > 0) ? green : 
    ///        (src < filt) and (src < src[1]) and (downward > 0) ? red : (src < filt) and (src > src[1]) and (downward > 0) ? maroon : orange
    ///
    ///     filtplot = plot(filt, color=filtcolor, linewidth=3, title="Range Filter")
    ///
    ///     // Target
    ///     hbandplot = plot(hband, color=aqua, transp=100, title="High Target")
    ///     lbandplot = plot(lband, color=fuchsia, transp=100, title="Low Target")
    ///
    ///     // Fills
    ///     fill(hbandplot, filtplot, color=aqua, title="High Target Range")
    ///     fill(lbandplot, filtplot, color=fuchsia, title="Low Target Range")
    ///
    ///     // Bar Color
    ///     barcolor(barcolor)
    ///
    ///     // Break Outs
    ///     longCond = na
    ///     shortCond = na
    ///     longCond := ((src > filt) and (src > src[1]) and (upward > 0)) or ((src > filt) and (src < src[1]) and (upward > 0)) 
    ///     shortCond := ((src < filt) and (src < src[1]) and (downward > 0)) or ((src < filt) and (src > src[1]) and (downward > 0))
    ///
    ///     CondIni = 0
    ///     CondIni := longCond ? 1 : shortCond ? -1 : CondIni[1]
    ///     longCondition = longCond and CondIni[1] == -1
    ///     shortCondition = shortCond and CondIni[1] == 1
    ///
    ///     //Alerts
    ///
    ///     plotshape(longCondition, title = "Buy Signal", text ="AL", textcolor = white, style=shape.labelup, size = size.small, location=location.belowbar, color = green, transp = 0)
    ///     plotshape(shortCondition, title = "Sell Signal", text ="SAT", textcolor = white, style=shape.labeldown, size = size.small, location=location.abovebar, color = red, transp = 0)
    ///
    ///     alertcondition(longCondition, title="Buy Alert", message = "Short")
    ///     alertcondition(longCondition, title="Buy Alert", message = "Short")
    ///     alertcondition(longCondition, title="Buy Alert", message = "Short")
    ///     alertcondition(shortCondition, title="Sell Alert", message = "Long")
    /// </summary>
    public class EDGE
    {
        public static SerkaEdgeResultDto Calculate(double[] closeArray, int length, double multiplier)
        {
            var edgeResult = new SerkaEdgeResultDto();

            Condition.Requires(closeArray, "closeArray").IsNotEmpty();

            var smrng = SmoothRng(closeArray, length, multiplier);
            var filt = rngfilt(closeArray, smrng);

            var upward = new double[closeArray.Length];
            var downward = new double[closeArray.Length];
            var hband = new double[closeArray.Length];
            var lband = new double[closeArray.Length];
            var longCond = new bool[closeArray.Length];
            var shortCond = new bool[closeArray.Length];

            for (int i = 1; i < closeArray.Length; i++)
            {
                upward[i] = filt[i] > filt[i - 1]
                    ? upward[i - 1] + 1
                    : filt[i] < filt[i - 1] ? 0 : upward[i - 1];
                downward[i] = filt[i] < filt[i - 1]
                    ? downward[i - 1] + 1
                    : filt[i] > filt[i - 1] ? 0 : downward[i - 1];

                // Target Bands
                hband[i] = filt[i] + smrng[i];
                lband[i] = filt[i] - smrng[i];

                longCond[i] = ((closeArray[i] > filt[i]) && (closeArray[i] > closeArray[i-1]) && (upward[i] > 0)) 
                    || ((closeArray[i] > filt[i]) && (closeArray[i] < closeArray[i-1]) && (upward[i] > 0));
                shortCond[i] = ((closeArray[i] < filt[i]) && (closeArray[i] < closeArray[i - 1]) && (downward[i] > 0))
                    || ((closeArray[i] < filt[i]) && (closeArray[i] > closeArray[i - 1]) && (downward[i] > 0));
            }

            edgeResult.Filt = filt;
            edgeResult.Hband = hband;
            edgeResult.Lband = lband;
            edgeResult.LongCondition = longCond;
            edgeResult.ShortCondition = shortCond;

            return edgeResult;
        }

        /// smoothrng(x, t, m)=>
        ///         wper      = (t*2) - 1
        ///         avrng     = ema(abs(x - x[1]), t)
        ///         smoothrng = ema(avrng, wper)*m
        ///         smoothrng
        ///     smrng = smoothrng(src, per, mult)

        private static double[] SmoothRng(double[] priceArray, int period, double multiplier)
        {
            var wper = (period * 2) - 1;

            var absArray = new double[priceArray.Length];
            for (int i = 1; i < priceArray.Length; i++)
                absArray[i] = System.Math.Abs(priceArray[i] - priceArray[i - 1]);

            var avrng = TA.EMA.Calculate(absArray, period);
            var smoothrng = TA.EMA.Calculate(avrng, wper);

            for (int i = 0; i < smoothrng.Length; i++)
                smoothrng[i] = smoothrng[i] * multiplier;

            return smoothrng;
        }

        private static double[] rngfilt(double[] sourceArray, double[] smoothArray)
        {
            var rangeFilterArray = new double[sourceArray.Length];
            for (int i = 1; i < sourceArray.Length; i++)
            {
                rangeFilterArray[i] = sourceArray[i];
                rangeFilterArray[i] = sourceArray[i] > rangeFilterArray[i - 1]
                    ? ((sourceArray[i] - smoothArray[i]) < rangeFilterArray[i - 1]
                            ? rangeFilterArray[i - 1]
                            : (sourceArray[i] - smoothArray[i]))
                    : ((sourceArray[i] + smoothArray[i]) > rangeFilterArray[i - 1]
                            ? rangeFilterArray[i - 1]
                            : (sourceArray[i] + smoothArray[i]));
            }

            return rangeFilterArray;
        }
    }
}