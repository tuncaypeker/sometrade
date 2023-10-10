namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;
    using System;

    /// <summary>
    /// name:
    ///    ICHIMOKU Kinko Hyo by KIVANC fr3762
    /// description:
    ///     Created in 1940's by Goichi Hosoda withe the help of University students in Japan.
    ///     Ichimoku is one of the best trend following indicators that works nearly perfect in all markets and time frames.
    ///
    ///     Ichimoku is originally an built in indicator in Tradingview but there are some problems like:
    ///     the indicator hast 5 lines but you can change only 4 parameters in the settings menu of Tradingview Charts which you could only control 3 of the lines effectively. A second problem is that Tradingview preferred to use English titles for the ICHIMOKU lines instead of giving them the most common original Japanese ones. (So I rewrite the indicator)
    ///
    ///     Kijun Sen (blue line): Also called standard line or base line, this is calculated by averaging the highest high and the lowest low for the past 26 periods.
    ///
    ///     Tenkan Sen (red line): This is also known as the turning line and is derived by averaging the highest high and the lowest low for the past nine periods.
    ///
    ///     Chikou Span (Plum line): This is called the lagging line. It is today’s closing price plotted 26 periods behind.
    ///
    ///     Senkou SpanA (green line): The first Senkou line is calculated by averaging the Tenkan Sen and the Kijun Sen and plotted 26 periods ahead.
    ///
    ///     Senkou SpanB (purple line):
    ///     The second Senkou line is determined by averaging the highest high and the lowest low for the past 52 periods and plotted 26 periods ahead.
    ///
    ///
    ///     PERSONALLY I ADVISE YOU TO USE ICHIMOKU WITH DEAFULT LENGTHS (9,26,26,52,26) IN ORDER FOR STOCK MARKETS AND FOREX MARKETS
    ///
    ///     FOR CRYPTO YOU'D BETTER USE:
    ///     10,30,30,60,30 OR 20,60,60,120,60
    ///     THE TRICKY THING IS THAT KEEPING THE 1-3-3-6-3 RATIO CONSTANT IS NECESSARY
    ///
    ///
    ///     Here's a link of my Youtube video explaining ICHIMOKU but unfortunately only in TURKISH:
    ///     https://www.youtube.com/watch?v=MyTlGtuE...
    ///
    ///     Developed by: Goichi Hosoda
    ///
    /// link:
    ///    https://tr.tradingview.com/script/X4pwbIqa/
    /// formula:
    ///     
    /// pinescript:
    ///    //@version=3
    ///
    ///    //author KývanÇ @fr3762 on twitter
    ///    //creator Goichi Hosoda
    ///    
    ///    study("ICHIMOKU", shorttitle="ICH", overlay=true)
    ///    
    ///    TKlength=input(9, "Tenkansen Length", minval=1)
    ///    KJlength=input(26, "Kijunsen Length", minval=1)
    ///    CSHSlength=input(26, "Chikouspan Length/Horizontal Shift", minval=1)
    ///    SBlength=input(52, "SenkouspanB Length", minval=1)
    ///    SAlength=input(26,"SenkouspanA Length", minval=1)
    ///    
    ///    TK=avg(lowest(TKlength), highest(TKlength))
    ///    KJ=avg(lowest(KJlength), highest(KJlength))
    ///    CS=close
    ///    SB=avg(lowest(SBlength), highest(SBlength))
    ///    SA=avg(TK,KJ)
    ///    
    ///    
    ///    
    ///    
    ///    plot(TK, linewidth=2, color=blue, title="TenkanSen")
    ///    plot(KJ, linewidth=2, color=red, title="KijunSen")
    ///    
    ///    plot(CS, offset=-CSHSlength, linewidth=2, color=#DDA0DD, title="ChikouSpan")
    ///    
    ///    SenkouA=plot(SA, offset=CSHSlength,  linewidth=1, color=green, title="SenkouSpanA")
    ///    SenkouB=plot(SB, offset=CSHSlength, linewidth=1, color=purple, title="SenkouSpanB")
    ///    
    ///    fill(SenkouA, SenkouB, color = SA > SB ? green : red)
    /// </summary>
    public class ICHIMOKUK
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="tenkansenLength">9</param>
        /// <param name="kijunsenLength">26</param>
        /// <param name="chikouspanLength">26</param>
        /// <param name="senkouspanBLength">52</param>
        /// <param name="senkouspanALength">26</param>
        /// <returns></returns>
        public static ICHIMOKUKResultDto Calculate(double[] low, double[] high, int tenkansenLength, int kijunsenLength, int chikouspanLength, int senkouspanBLength, int senkouspanALength)
        {
            Condition.Requires(low, "low").IsNotEmpty();
            Condition.Requires(high, "high").IsNotEmpty();

            var tKLowest = TA.Lowest.Calculate(low, tenkansenLength);
            var tKHighest = TA.Highest.Calculate(high, tenkansenLength);
            var TK = TA.Math.AVG(tKLowest, tKHighest);

            var kJLowest = TA.Lowest.Calculate(low, kijunsenLength);
            var kJHighest = TA.Highest.Calculate(high, kijunsenLength);
            var KJ = TA.Math.AVG(kJLowest, kJHighest);

            var SBLowest = TA.Lowest.Calculate(low, senkouspanBLength);
            var SBHighest = TA.Highest.Calculate(high, senkouspanBLength);
            var SBNormal = TA.Math.AVG(SBLowest, SBHighest);
            var SB = new double[SBNormal.Length];
            Array.Copy(SBNormal, 0, SB, chikouspanLength, SBNormal.Length - chikouspanLength);

            //SA Array'i chikouspanLength kadar saða shift edilecek
            var SANormal = TA.Math.AVG(TK, KJ);
            var SA = new double[SANormal.Length];
            Array.Copy(SANormal, 0, SA, chikouspanLength, SANormal.Length - chikouspanLength);

            return new ICHIMOKUKResultDto()
            {
                KijunSen = KJ,
                SenkouSpanA = SA,
                SenkouSpanB = SB,
                TenkanSen = TK
            };
        }
    }
}
