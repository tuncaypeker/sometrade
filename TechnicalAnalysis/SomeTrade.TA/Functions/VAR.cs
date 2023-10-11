namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using System.Linq;

    /// <summary>
    /// Variable Moving Average VAR Or VMA
    /// Mesela Bu Matrisk'te falan VAR
    /// 
    /// !!!!! KÖTÜ HABER şu ki bu indikatör kendinden önceki değerine göre hesaplanıyor 
    /// bu ne demek 500 mum verirsen değer farklı oluyor, 1000 mum verirsen değer farklı oluyor
    /// Kısacası yazdıgım formu ldogru ancak hesaplamalara dahil edemezsin
    /// 
    /// </summary>
    /// <summary>
    /// name:
    ///     Variable Moving Average
    ///     The Variable Moving Average is also known as the VIDYA Indicator.
    ///     
    ///     İlginc sekilde Kıvanc 'in VIDYA'si
    ///     Everget'in de VIDYA'si var ama tahmin edersin ki farklı sonuclar veriyor, formulde ne degisiklikler var bilemiyorum
    ///     Bu kavram karmaşası cok fazla oluyor ama anladıgım kadaryıla VIDYA farklı olarak daha dinamik bir yapi var
    ///     Variable Index Dynamic Average
    /// description:
    ///     A VMA is calculated by taking the average of the most recent price data, with an emphasis on the most recent price data.
    /// link:
    ///     
    /// formula:
    ///     VMA = [P + (a*b)P1 + (a*b)2P2 + … + (a*b)(n-1)P(n-1)]/ [1 + (a*b) + (a*b)2   + …   + (a*b)(n-1)]
    ///
    ///     Where:
    ///     P = current price
    ///     P1 = price 1 period ago
    ///     P2 = price 2 periods ago
    ///     a = smoothing constant 2/(n+1)
    ///     b = absolute value (F(P)/100)
    ///     n = user-defined number of periods for the average
    ///  pinescript:
    /*
    Kıvanc'in Trading View OTT icerisinden aldim
    

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
    plot(VAR)

    Ilginc bir sekilde LazyBear'in yazdıgı VMA_LB indikatörü ile eşleşmiyor. LazyBear'inki bizim sistemde var yazılmamış olabilir
    ama ben burada kivanc'in ki ile ilerliycem

    //
    // @author LazyBear 
    // List of all my indicators: 
    // https://docs.google.com/document/d/15AGCufJZ8CIUvwFJ9W-IKns88gkWOKBCvByMEvm5MLo/edit?usp=sharing
    // 
    study(title="Variable Moving Average [LazyBear]", shorttitle="VMA_LB", overlay=true)
    src=close
    l =input(6, title="VMA Length") 
    std=input(false, title="Show Trend Direction")
    bc=input(false, title="Color bars based on Trend")
    k = 1.0/l
    pdm = max((src - src[1]), 0)
    mdm = max((src[1] - src), 0)
    pdmS = ((1 - k)*nz(pdmS[1]) + k*pdm)
    mdmS = ((1 - k)*nz(mdmS[1]) + k*mdm)
    s = pdmS + mdmS
    pdi = pdmS/s
    mdi = mdmS/s
    pdiS = ((1 - k)*nz(pdiS[1]) + k*pdi)
    mdiS = ((1 - k)*nz(mdiS[1]) + k*mdi)
    d = abs(pdiS - mdiS)
    s1 = pdiS + mdiS
    iS = ((1 - k)*nz(iS[1]) + k*d/s1)
    hhv = highest(iS, l) 
    llv = lowest(iS, l) 
    d1 = hhv - llv
    vI = (iS - llv)/d1
    vma = (1 - k*vI)*nz(vma[1]) + k*vI*src
    vmaC=(vma > vma[1]) ? green : (vma<vma[1]) ? red : (vma==vma[1]) ? blue : black 
    plot(vma, color=std?vmaC:black, linewidth=3, title="VMA")
    barcolor(bc?vmaC:na)
    */
    /// </summary>
    public class VAR
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            double valpha = (2.0 / (period + 1));
            double[] vma = new double[price.Length];
            double[] vud1 = new double[price.Length];
            double[] vdd1 = new double[price.Length];

            for (int i = 1; i < price.Length; i++)
            {
                vud1[i] = price[i] > price[i - 1] ? price[i] - price[i - 1] : 0;
                vdd1[i] = price[i] < price[i - 1] ? price[i - 1] - price[i] : 0;

                var vUD = SumArrayLast(vud1, i, 9);
                var vDD = SumArrayLast(vdd1, i, 9);

                var vCMO = (vUD - vDD) / (vUD + vDD) == 0
                    ? 0
                    : (vUD - vDD) / (vUD + vDD);

                if (i < 8)
                    continue;

                vma[i] = (valpha * System.Math.Abs(vCMO) * price[i])+ (1 - valpha * System.Math.Abs(vCMO)) * vma[i - 1];
            }

            return vma;
        }

        /// <summary>
        /// 22 den basla geriye donuk olarak 9 tane topla
        /// array, 22, 9
        /// array[22], array[21], array[20].... ya 9 kere doner ya da start sifir olacagi icin durur
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static double SumArrayLast(double[] array, int start, int length)
        {
            double sum = 0;

            while (start != -1 && length != 0)
            {
                sum += array[start];
                start -= 1;
                length -= 1;
            }

            return sum;
        }
    }
}
