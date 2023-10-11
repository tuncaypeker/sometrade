namespace SomeTrade.TA.Indicators.ahmetkasa
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;
    using System.Linq;

    /// <summary>
    /// name:
    /// description:
    /// pinescript:
    ///  study(title="FibFib(Ahmet Kasa)", shorttitle="AutoFib", overlay=true)
    ///  fiblength=input(265)
    ///  maxr = highest(close, fiblength)
    ///  minr = lowest(close, fiblength)
    ///  ranr = maxr - minr
    ///  
    ///  ON=plot( maxr , color=#000000,  title="1")
    ///  SS=plot( maxr - 0.236 * ranr, title="0.764", color=#3399FF )
    ///  SO=plot( maxr - 0.382 * ranr, title="0.618", color=#3399FF )
    ///  FI=plot( maxr - 0.50 * ranr, title="0.5", color=#FFFB05)
    ///  TE=plot( minr + 0.382 * ranr, title="0.382", color=color.rgb(5, 255, 176) )
    ///  TT=plot( minr + 0.236 * ranr, title="0.236", color=#FF0505)
    ///  ZZ=plot( minr , title="0", color=#000000 )
    /// </summary>
    public class AutoFib
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="close"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <param name="period">Common Period</param>
        /// <param name="coeff">Çarpan 0.1 olarak ilerlemeli</param>
        /// <param name="novolumedata">true gelirse hesaplamada hacim kullanilmaz</param>
        /// <returns></returns>
        public static AutoFibResult Execute(double[] price, int length = 265)
        {
            Condition.Requires(price, "close").IsNotNull().IsNotEmpty();
            Condition.Requires(length, "length").IsGreaterThan(0);

            double[] maxr = new double[price.Length];
            double[] minr = new double[price.Length];
            double[] ranr = new double[price.Length];
            for (int i = length; i < price.Length; i++)
            {
                //i = 265 265 tane al
                //i = 266, 1 tane atla, 265 tane al
                //i = 267, 2 tane atla, 265 tane al
                var currentPriceSet = price.Skip(i - length).Take(length).ToArray();

                var maxrCurrent = TA.Highest.Calculate(currentPriceSet, length);
                var minrCurrent = TA.Lowest.Calculate(currentPriceSet, length);

                maxr[i - 1] = maxrCurrent[length - 1];
                minr[i - 1] = minrCurrent[length - 1];
                ranr[i - 1] = maxrCurrent[length - 1] - minrCurrent[length - 1];
            }

            double[] ss = new double[price.Length];
            double[] so = new double[price.Length];
            double[] fi = new double[price.Length];
            double[] te = new double[price.Length];
            double[] tt = new double[price.Length];

            for (int i = 0; i < price.Length; i++) {
                ss[i] = maxr[i] - 0.236 * ranr[i]; 
                so[i] = maxr[i] - 0.382 * ranr[i]; 
                fi[i] = maxr[i] - 0.5 * ranr[i]; 

                te[i] = minr[i] + 0.382 * ranr[i]; 
                tt[i] = minr[i] + 0.236 * ranr[i]; 
            }

            var result = new AutoFibResult()
            {
                Fi = fi,
                Maxr = maxr,
                Minr = minr,
                So = so,
                Ss = ss,
                Te = te,
                Tt = tt
            };

            return result;
        }
    }
}
