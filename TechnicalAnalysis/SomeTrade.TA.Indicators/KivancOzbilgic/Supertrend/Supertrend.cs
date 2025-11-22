namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    using CuttingEdge.Conditions;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Supertrend Indicator
    /// Biz ekledik
    /// </summary>
    /*
   
    //@version=4
    study("Supertrend", overlay = true, format=format.price, precision=2, resolution="")

    Periods = input(title="ATR Period", type=input.integer, defval=10)
    src = input(hl2, title="Source")
    Multiplier = input(title="ATR Multiplier", type=input.float, step=0.1, defval=3.0)
    changeATR= input(title="Change ATR Calculation Method ?", type=input.bool, defval=true)
    showsignals = input(title="Show Buy/Sell Signals ?", type=input.bool, defval=true)
    highlighting = input(title="Highlighter On/Off ?", type=input.bool, defval=true)
    atr2 = sma(tr, Periods)
    atr= changeATR ? atr(Periods) : atr2
    up=src-(Multiplier*atr)
    up1 = nz(up[1],up)
    up := close[1] > up1 ? max(up,up1) : up
    dn=src+(Multiplier*atr)
    dn1 = nz(dn[1], dn)
    dn := close[1] < dn1 ? min(dn, dn1) : dn
    trend = 1
    trend := nz(trend[1], trend)
    trend := trend == -1 and close > dn1 ? 1 : trend == 1 and close < up1 ? -1 : trend
    upPlot = plot(trend == 1 ? up : na, title="Up Trend", style=plot.style_linebr, linewidth=2, color=color.green)
    buySignal = trend == 1 and trend[1] == -1
    plotshape(buySignal ? up : na, title="UpTrend Begins", location=location.absolute, style=shape.circle, size=size.tiny, color=color.green, transp=0)
    plotshape(buySignal and showsignals ? up : na, title="Buy", text="Buy", location=location.absolute, style=shape.labelup, size=size.tiny, color=color.green, textcolor=color.white, transp=0)
    dnPlot = plot(trend == 1 ? na : dn, title="Down Trend", style=plot.style_linebr, linewidth=2, color=color.red)
    sellSignal = trend == -1 and trend[1] == 1
    plotshape(sellSignal ? dn : na, title="DownTrend Begins", location=location.absolute, style=shape.circle, size=size.tiny, color=color.red, transp=0)
    plotshape(sellSignal and showsignals ? dn : na, title="Sell", text="Sell", location=location.absolute, style=shape.labeldown, size=size.tiny, color=color.red, textcolor=color.white, transp=0)
    mPlot = plot(ohlc4, title="", style=plot.style_circles, linewidth=0)
    longFillColor = highlighting ? (trend == 1 ? color.green : color.white) : color.white
    shortFillColor = highlighting ? (trend == -1 ? color.red : color.white) : color.white
    fill(mPlot, upPlot, title="UpTrend Highligter", color=longFillColor)
    fill(mPlot, dnPlot, title="DownTrend Highligter", color=shortFillColor)
    alertcondition(buySignal, title="SuperTrend Buy", message="SuperTrend Buy!")
    alertcondition(sellSignal, title="SuperTrend Sell", message="SuperTrend Sell!")
    changeCond = trend != trend[1]
    alertcondition(changeCond, title="SuperTrend Direction Change", message="SuperTrend has changed direction!")


     */
    /// <summary>
    /// name:
    ///     
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     
    /// </summary>
    public class Supertrend
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, double[] close, double[] high, double[] low, int period)
        {
            bool changeATR = true;
            var multiplier = 3.0;

            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var trueRanges = new double[price.Length];
            for (int i = 1; i < price.Length; ++i)
            {
                //calculate tr
                var trueHigh = high[i] > price[i - 1] ? high[i] : price[i - 1];
                var trueLow = low[i] < price[i - 1] ? low[i] : price[i - 1];
                var tr = trueHigh - trueLow;

                trueRanges[i] = tr;
            }
            var atr2 = TA.SMA.Calculate(trueRanges, period);
            var atr = changeATR
                ? TA.ATR.Calculate(close, high, low, period, "rma")
                : atr2;

            var up = new double[price.Length];
            for (int i = period-1; i < price.Length; ++i)
            {
                up[i] = price[i] - (multiplier * atr[i]);
                if (close[i - 1] > up[i - 1])
                    up[i] = new List<double>() { up[i], up[i - 1] }.Max();
            }

            var down = new double[price.Length];
            for (int i = period-1; i < price.Length; ++i)
            {
                down[i] = price[i] + (multiplier * atr[i]);

                if (close[i - 1] < down[i - 1])
                    down[i] = new List<double>() { down[i], down[i - 1] }.Min();
            }

            var trend = new double[price.Length];
            trend[0] = 1;
            for (int i = 1; i < price.Length; ++i)
            {
                trend[i] = 1;
                if (trend[i - 1] != 0)
                    trend[i] = trend[i - 1];

                trend[i] = trend[i] == -1 && close[i] > down[i - 1]
                     ? 1
                     : trend[i] == 1 && close[i] < up[i - 1]
                          ? -1
                          : trend[i];
            }

            return trend;
        }
    }
}
