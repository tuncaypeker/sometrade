namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    /// <summary>
    /// name:
    ///     
    /// description:
    /// 
    /// pinescript:
    ///       //@version=3
    ///       // Kıvanç Özbilgiç
    ///       study("Laguerre Filter", shorttitle="LagF", overlay=true)
    ///       src = input(title="Source", defval=hl2)
    ///       alpha = input(title="Alpha", type=float, minval=0, maxval=1, step=0.1, defval=0.2)
    ///       colorchange = input(title="Change Color ?", type=bool, defval=true)
    ///       gamma=1-alpha
    ///       L0 = 0.0
    ///       L0 := (1-gamma) * src + gamma * nz(L0[1])
    ///       L1 = 0.0
    ///       L1 := -gamma * L0 + nz(L0[1]) + gamma * nz(L1[1])
    ///
    ///       L2 = 0.0
    ///       L2 := -gamma * L1 + nz(L1[1]) + gamma * nz(L2[1])
    ///
    ///       L3 = 0.0
    ///       L3 := -gamma * L2 + nz(L2[1]) + gamma * nz(L3[1])
    ///
    ///       LagF = (L0 + 2 * L1 + 2 * L2 + L3) / 6
    ///
    ///       Color = colorchange ? (LagF > LagF[1] ? green : red) : yellow
    ///       plot(LagF, title="LagF", linewidth=2, color=Color, transp=0)
    ///
    /// </summary>
    public class LagF
    {
    }
}
