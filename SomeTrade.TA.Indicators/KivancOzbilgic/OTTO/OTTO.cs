using CuttingEdge.Conditions;
using System.Linq;

namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    public class OTTO
    {
        /// <summary>
        /// //@version=4
        /// // This source code is subject to the terms of the Mozilla Public License 2.0 at https://mozilla.org/MPL/2.0/
        /// // © KivancOzbilgic
        ///
        /// //OTT created by: @Anil_Ozeksi
        /// //OTTO developer: Kamil HAsan ALPAY
        /// //author: @kivancozbilgic
        ///
        /// study("Optimized Trend Tracker Oscillator","OTTO", overlay=false)
        /// length=input(2, "OTT Period", minval=1)
        /// percent=input(0.6, "OTT Optimization Coeff", type=input.float, step=0.1, minval=0)
        /// flength=input(10, "FAST VIDYA Length", minval=1)
        /// slength=input(25, "SLOW VIDYA Length", minval=1)
        /// coco=input(100000, "Correcting Constant", minval=1)
        ///
        /// src1 = input(close, title='Source')
        /// showsignalsc = input(title="OTTO Crossing Signals?", type=input.bool, defval=true)
        /// highlighting = input(title="Highlighter On/Off ?", type=input.bool, defval=true)
        /// mav = input(title="Moving Average Type", defval="VAR", options=["SMA", "EMA", "WMA", "DEMA", "TMA", "VAR", "WWMA", "ZLEMA", "TSF", "HULL"])
        ///
        ///
        /// Var_Func1(src1,length)=>
        ///     valpha1=2/(length+1)
        ///     vud11=src1>src1[1] ? src1-src1[1] : 0
        ///     vdd11=src1<src1[1] ? src1[1]-src1 : 0
        ///     vUD1=sum(vud11,9)
        ///     vDD1=sum(vdd11,9)
        ///     vCMO1=nz((vUD1-vDD1)/(vUD1+vDD1))
        ///     VAR1=0.0
        ///     VAR1:=nz(valpha1*abs(vCMO1)*src1)+(1-valpha1*abs(vCMO1))*nz(VAR1[1])
        ///
        /// VAR1=Var_Func1(src1,length)
        ///
        ///
        /// mov1=Var_Func1(src1,slength/2)
        /// mov2=Var_Func1(src1,slength)
        /// mov3=Var_Func1(src1,slength*flength)
        ///
        /// src=mov1/(mov2-mov3+coco)
        ///
        /// Var_Func(src,length)=>
        ///     valpha=2/(length+1)
        ///     vud1=src>src[1] ? src-src[1] : 0
        ///     vdd1=src<src[1] ? src[1]-src : 0
        ///     vUD=sum(vud1,9)
        ///     vDD=sum(vdd1,9)
        ///     vCMO=nz((vUD-vDD)/(vUD+vDD))
        ///     VAR=0.0
        ///     VAR:=nz(valpha*abs(vCMO)*src)+(1-valpha*abs(vCMO))*nz(VAR[1])
        ///
        /// VAR=Var_Func(src,length)
        ///
        ///
        /// DEMA = ( 2 * ema(src,length)) - (ema(ema(src,length),length) )
        /// Wwma_Func(src,length)=>
        ///     wwalpha = 1/ length
        ///     WWMA = 0.0
        ///     WWMA := wwalpha*src + (1-wwalpha)*nz(WWMA[1])
        /// WWMA=Wwma_Func(src,length)
        /// Zlema_Func(src,length)=>
        ///     zxLag = length/2==round(length/2) ? length/2 : (length - 1) / 2
        ///     zxEMAData = (src + (src - src[zxLag]))
        ///     ZLEMA = ema(zxEMAData, length)
        /// ZLEMA=Zlema_Func(src,length)
        /// Tsf_Func(src,length)=>
        ///     lrc = linreg(src, length, 0)
        ///     lrc1 = linreg(src,length,1)
        ///     lrs = (lrc-lrc1)
        ///     TSF = linreg(src, length, 0)+lrs
        /// TSF=Tsf_Func(src,length)
        /// HMA = wma(2 * wma(src, length / 2) - wma(src, length), round(sqrt(length)))
        /// Var_Funcl(srcl,length)=>
        ///     valphal=2/(length+1)
        ///     vud1l=srcl>srcl[1] ? srcl-srcl[1] : 0
        ///     vdd1l=srcl<srcl[1] ? srcl[1]-srcl : 0
        ///     vUDl=sum(vud1l,9)
        ///     vDDl=sum(vdd1l,9)
        ///     vCMOl=nz((vUDl-vDDl)/(vUDl+vDDl))
        ///     VARl=0.0
        ///     VARl:=nz(valphal*abs(vCMOl)*srcl)+(1-valphal*abs(vCMOl))*nz(VARl[1])
        ///
        /// getMA(src, length) =>
        ///     ma = 0.0
        ///     if mav == "SMA"
        ///         ma := sma(src, length)
        ///         ma
        ///
        ///     if mav == "EMA"
        ///         ma := ema(src, length)
        ///         ma
        ///
        ///     if mav == "WMA"
        ///         ma := wma(src, length)
        ///         ma
        ///
        ///     if mav == "DEMA"
        ///         ma := DEMA
        ///         ma
        ///
        ///     if mav == "TMA"
        ///         ma := sma(sma(src, ceil(length / 2)), floor(length / 2) + 1)
        ///         ma
        ///
        ///     if mav == "VAR"
        ///         ma := VAR
        ///         ma
        ///
        ///     if mav == "WWMA"
        ///         ma := WWMA
        ///         ma
        ///
        ///     if mav == "ZLEMA"
        ///         ma := ZLEMA
        ///         ma
        ///
        ///     if mav == "TSF"
        ///         ma := TSF
        ///         ma
        ///
        ///     if mav == "HULL"
        ///         ma := HMA
        ///         ma
        ///     ma
        ///
        ///     MAvg=getMA(src, length)
        ///     fark=MAvg*percent*0.01
        ///     longStop = MAvg - fark
        ///     longStopPrev = nz(longStop[1], longStop)
        ///     longStop := MAvg > longStopPrev ? max(longStop, longStopPrev) : longStop
        ///     shortStop =  MAvg + fark
        ///     shortStopPrev = nz(shortStop[1], shortStop)
        ///     shortStop := MAvg < shortStopPrev ? min(shortStop, shortStopPrev) : shortStop
        ///     dir = 1
        ///     dir := nz(dir[1], dir)
        ///     dir := dir == -1 and MAvg > shortStopPrev ? 1 : dir == 1 and MAvg < longStopPrev ? -1 : dir
        ///     MT = dir==1 ? longStop: shortStop
        ///     HOTT=MAvg>MT ? MT*(200+percent)/200 : MT*(200-percent)/200 
        ///     HOTTC = color.new(color.red, 0)
        ///
        ///     LOTT=src
        ///     LOTTC = color.new(color.blue, 0)
        ///     HOTTLine=plot(nz(HOTT[2]), title="HOTT", color=HOTTC, linewidth=2, style=plot.style_line)
        ///     LOTTLine=plot(nz(LOTT), title="LOTT", color=LOTTC, linewidth=2, style=plot.style_line)
        ///     FillColor = highlighting and LOTT<HOTT[2] ? color.new(color.red, 20) : color.new(color.green, 20)
        ///     fill(HOTTLine, LOTTLine, title='Highligter', color=FillColor)
        ///     alertcondition(crossunder(HOTT[2], LOTT), title="Crossover Alarm", message="OTTO - BUY SIGNAL!")
        ///     alertcondition(crossover(HOTT[2], LOTT), title="Crossunder Alarm", message="OTTO - SELL SIGNAL!")
        ///
        ///     buySignalc = crossunder(HOTT[2], LOTT)
        ///     plotshape(buySignalc and showsignalsc ? HOTT[2]*0.9995 : na, title="Buy", text="Buy", location=location.absolute, style=shape.labelup, size=size.tiny, color=color.green, textcolor=color.white)
        ///     sellSignallc = crossover(HOTT[2], LOTT)
        ///     plotshape(sellSignallc and showsignalsc ? LOTT*1.0005 : na, title="Sell", text="Sell", location=location.absolute, style=shape.labeldown, size=size.tiny, color=color.red, textcolor=color.white)
        ///
        /// </summary>
        /// <param name="closeArray"></param>
        /// <param name="length">OTT Period</param>
        /// <param name="percent">OTT Optimization Coeff</param>
        /// <param name="fLength">FAST VIDYA Length</param>
        /// <param name="sLength">SLOW VIDYA Length</param>
        /// <param name="coco">Correcting Constant</param>
        /// <param name="maType">"SMA", "EMA", "WMA", "DEMA", "TMA", "VAR", "WWMA", "ZLEMA", "TSF", "HULL"</param>
        /// <returns></returns>
        public static double[] Calculate(double[] closeArray, int length = 2, double percent = 0.6, int fLength = 10
            , int sLength = 25, int coco = 100000, string maType = "SMA")
        {
            Condition.Requires(closeArray, "closeArray").IsNotEmpty();

            return null;
        }

        private static void Var_Func1(double[] closeArray, int length)
        {
            var valpha1 = 2 / (length + 1);
            var vud11 = closeArray.Last() > closeArray.TakePrev()
                ? closeArray.Last() - closeArray.TakePrev()
                : 0;

            var vdd11 = closeArray.Last() < closeArray.TakePrev()
                ? closeArray.TakePrev() - closeArray.Last()
                : 0;

            /*
            valpha1 = 2 / (length + 1)
            vud11 = src1 > src1[1] ? src1 - src1[1] : 0
            vdd11 = src1 < src1[1] ? src1[1] - src1 : 0
            vUD1 = sum(vud11, 9)
            vDD1 = sum(vdd11, 9)
            vCMO1 = nz((vUD1 - vDD1) / (vUD1 + vDD1))
            VAR1 = 0.0
            
            VAR1:= nz(valpha1 * abs(vCMO1) * src1) + (1 - valpha1 * abs(vCMO1)) * nz(VAR1[1])
            */

        }
    }
}
