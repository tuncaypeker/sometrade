using SomeTrade.TA.Indicators.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomeTrade.TA.Indicators.KivancOzbilgic.OTT
{
    /// <summary>
    /*
     
    //@version=4
    // This source code is subject to the terms of the Mozilla Public License 2.0 at https://mozilla.org/MPL/2.0/
    // © KivancOzbilgic

    //created by: @Anil_Ozeksi
    //developer: ANIL ÖZEKŞİ
    //author: @kivancozbilgic

    study("Optimized Trend Tracker","OTT", overlay=true)
    src = input(close, title="Source")
    length=input(40, "OTT Period", minval=1)
    percent=input(1, "OTT Percent", type=input.float, step=0.1, minval=0)
    showsupport = input(title="Show Support Line?", type=input.bool, defval=true)

    mav = input(title="Moving Average Type", defval="VAR", options=["SMA", "EMA", "WMA", "TMA", "VAR", "WWMA", "ZLEMA", "TSF"])
    Var_Func(src,length)=>
        valpha=2/(length+1)
        vud1=src>src[1] ? src-src[1] : 0
        vdd1=src<src[1] ? src[1]-src : 0
        vUD=sum(vud1,9)
        vDD=sum(vdd1,9)
        vCMO=nz((vUD-vDD)/(vUD+vDD))
        VAR=0.0
        VAR:=nz(valpha*abs(vCMO)*src)+(1-valpha*abs(vCMO))*nz(VAR[1])
    VAR=Var_Func(src,length)
    Wwma_Func(src,length)=>
        wwalpha = 1/ length
        WWMA = 0.0
        WWMA := wwalpha*src + (1-wwalpha)*nz(WWMA[1])
    WWMA=Wwma_Func(src,length)
    Zlema_Func(src,length)=>
        zxLag = length/2==round(length/2) ? length/2 : (length - 1) / 2
        zxEMAData = (src + (src - src[zxLag]))
        ZLEMA = ema(zxEMAData, length)
    ZLEMA=Zlema_Func(src,length)
    Tsf_Func(src,length)=>
        lrc = linreg(src, length, 0)
        lrc1 = linreg(src,length,1)
        lrs = (lrc-lrc1)
        TSF = linreg(src, length, 0)+lrs
    TSF=Tsf_Func(src,length)
    getMA(src, length) =>
        ma = 0.0
        if mav == "SMA"
            ma := sma(src, length)
            ma

        if mav == "EMA"
            ma := ema(src, length)
            ma

        if mav == "WMA"
            ma := wma(src, length)
            ma

        if mav == "TMA"
            ma := sma(sma(src, ceil(length / 2)), floor(length / 2) + 1)
            ma

        if mav == "VAR"
            ma := VAR
            ma

        if mav == "WWMA"
            ma := WWMA
            ma

        if mav == "ZLEMA"
            ma := ZLEMA
            ma

        if mav == "TSF"
            ma := TSF
            ma
        ma
    
    MAvg=getMA(src, length)
    fark=MAvg*percent*0.01
    longStop = MAvg - fark
    longStopPrev = nz(longStop[1], longStop)
    longStop := MAvg > longStopPrev ? max(longStop, longStopPrev) : longStop
    shortStop =  MAvg + fark
    shortStopPrev = nz(shortStop[1], shortStop)
    shortStop := MAvg < shortStopPrev ? min(shortStop, shortStopPrev) : shortStop
    dir = 1
    dir := nz(dir[1], dir)
    dir := dir == -1 and MAvg > shortStopPrev ? 1 : dir == 1 and MAvg < longStopPrev ? -1 : dir
    MT = dir==1 ? longStop: shortStop
    OTT=MAvg>MT ? MT*(200+percent)/200 : MT*(200-percent)/200 
    plot(showsupport ? MAvg : na, color=#0585E1, linewidth=2, title="Support Line")

    pALL=plot(nz(OTT[2]), color=#B800D9 , linewidth=2, title="OTT", transp=0)

    mPlot = plot(ohlc4, title="", style=plot.style_circles, linewidth=0,display=display.none)

     */
    /// </summary>
    public class OTT
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period length.</param>
        /// <param name="opt">Indicator period, Kıvanc'ta OTT Percent olarak tanimli.</param>
        /// <param name="MA">Moving Average Type, Anıl VMA Kullanıyor. Seçenekler: "SMA", "EMA", "WMA","VAR"</param>
        /// Belki eklenecek MA'lar "TMA",  "WWMA", "ZLEMA", "TSF"
        /// <returns>Calculated indicator series.</returns>
        public static OTTResultDto Calculate(double[] price, int period, double opt, string MA = "SMA")
        {
            //MA seçimine göre degisecektir tabi
            double[] ma;
                
            switch (MA)
            {
                case "SMA": ma = TA.SMA.Calculate(price, period);  break;
                case "EMA": ma = TA.EMA.Calculate(price, period);  break;
                case "WMA": ma = TA.WMA.Calculate(price, period);  break;
                case "VAR": ma = TA.VAR.Calculate(price, period);  break;
                default: ma = TA.SMA.Calculate(price, period);  break;
            }

            var ottResult = new OTTResultDto()
            {
                OTTLine = new double[price.Length],
                SupportLine = ma
            };

            var longStops = new double[price.Length];
            var shortStops = new double[price.Length];
            var dirs = new double[price.Length];

            for (var i = 1; i < price.Length; i++)
            {
                int fark = (int)System.Math.Round(ma[i] * opt * 0.01);

                longStops[i] = ma[i] - fark;
                var longStopPrev = i == 1 ? longStops[i] : longStops[i - 1];
                if (ma[i] > longStopPrev && longStopPrev > longStops[i])
                    longStops[i] = longStopPrev;

                shortStops[i] = ma[i] + fark;
                var shortStopPrev = i == 1 ? shortStops[i] : shortStops[i - 1];
                if (ma[i] < shortStopPrev && shortStopPrev < shortStops[i])
                    shortStops[i] = shortStopPrev;

                dirs[i] = i == 1 ? 1 : dirs[i - 1];
                dirs[i] = dirs[i] == -1 && ma[i] > shortStopPrev
                    ? 1
                    : dirs[i] == 1 && ma[i] < longStopPrev
                        ? -1
                        : dirs[i];

                var mt = dirs[i] == 1 ? longStops[i] : shortStops[i];

                if (i < 2) continue;

                ottResult.OTTLine[i - 2] = ma[i] > mt
                    ? mt * (200 + opt) / 200
                    : mt * (200 - opt) / 200;
            }

            for (int i = ottResult.OTTLine.Length-1; i > 4; i--)
                ottResult.OTTLine[i] = ottResult.OTTLine[i - 4];

            return ottResult;
        }
    }
}
