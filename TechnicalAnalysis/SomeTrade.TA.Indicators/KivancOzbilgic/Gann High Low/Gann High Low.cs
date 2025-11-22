namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    /// <summary>
    /// Gann HiLo
    /// pinescript:
    ///     //@version=2
    ///     study("Gann High Low", overlay=true)
    ///     HPeriod= input(13,"HIGH Period")
    ///     LPeriod= input(21,"LOW Period")
    ///     HLd= iff(close>nz(sma(high,HPeriod))[1],1,iff(close<nz(sma(low,LPeriod))[1],-1,0))
    ///     HLv= valuewhen(HLd!=0,HLd,0)
    ///     HiLo= iff(HLv==-1,sma(high,HPeriod),sma(low,LPeriod))
    ///     HLcolor= HLv==-1 ? maroon : blue
    ///     plot(HiLo,linewidth=2, color=HLcolor)
    /// </summary>
    class GannHiLo
    {
        
    }
}
