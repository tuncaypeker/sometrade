namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    /// <summary>
    /// name:
    ///     
    /// description:
    /// 
    /// pinescript:
    ///      
    ///     // This source code is subject to the terms of the Mozilla Public License 2.0 at https://mozilla.org/MPL/2.0/
    ///     // Developer: John EHLERS
    ///     // © KivancOzbilgic
    ///     //@version=3
    ///     // Author:Kıvanç Özbilgiç
    ///     study("Laguerre RSI", shorttitle="LaRSI", overlay=false)
    ///     src = input(title="Source", defval=close)
    ///     alpha = input(title="Alpha", type=float, minval=0, maxval=1, step=0.1, defval=0.2)
    ///     colorchange = input(title="Change Color ?", type=bool, defval=false)
    ///     gamma=1-alpha
    ///     L0 = 0.0
    ///     L0 := (1-gamma) * src + gamma * nz(L0[1])
    ///     L1 = 0.0
    ///     L1 := -gamma * L0 + nz(L0[1]) + gamma * nz(L1[1])
    ///
    ///     L2 = 0.0
    ///     L2 := -gamma * L1 + nz(L1[1]) + gamma * nz(L2[1])
    ///
    ///     L3 = 0.0
    ///     L3 := -gamma * L2 + nz(L2[1]) + gamma * nz(L3[1])
    ///
    ///     cu= (L0>L1 ? L0-L1 : 0) + (L1>L2 ? L1-L2 : 0) + (L2>L3 ? L2-L3 : 0)
    ///
    ///     cd= (L0<L1 ? L1-L0 : 0) + (L1<L2 ? L2-L1 : 0) + (L2<L3 ? L3-L2 : 0)
    ///
    ///     temp= cu+cd==0 ? -1 : cu+cd
    ///     LaRSI=temp==-1 ? 0 : cu/temp
    ///
    ///     Color = colorchange ? (LaRSI > LaRSI[1] ? green : red) : blue
    ///     plot(100*LaRSI, title="LaRSI", linewidth=2, color=Color, transp=0)
    ///     plot(20,linewidth=1, color=maroon, transp=0)
    ///     plot(80,linewidth=1, color=maroon, transp=0)
    /// </summary>
    public class LaRSI
    {
    }
}
