namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;
    using System.Linq;

    /// <summary>
    /// name:
    ///     Darvas Box'ın Kıvanç tarafindan gelistirilen Versiyonu
    /// description:
    ///     Darvas box theory is a trading strategy developed by Nicolas Darvas that targets stocks using highs and volume as key indicators.
    /// link:
    ///    https://www.investopedia.com/terms/d/darvasboxtheory.asp
    /// pinescript:
    ///     //@version=3
    ///
    ///     //author KıvanÇ @fr3762 on twitter
    ///     //creator Nicholas Darvas
    ///
    ///     study("DARVAS BOX",overlay=true, shorttitle="DARVAS")
    ///     //iff(low=lowest(low,5),low,iff(low[1]=lowest(low,5),low[1],iff(low[2]=lowest(low,5),low[2],iff(low[3]=lowest(low,5),low[3],iff(low[4]=lowest(low,5),low[4],0)))))
    ///     //iff(low=lowest(low,5),low,iff(low[1]=lowest(low,5),low[1],iff(low[2]=lowest(low,5),low[2],iff(low[3]=lowest(low,5),low[3],iff(low[4]=lowest(low,5),low[4],0)))))
    ///     boxp=input(5, "BOX LENGTH")
    ///
    ///     LL = lowest(low,boxp)
    ///     k1=highest(high,boxp)
    ///     k2=highest(high,boxp-1)
    ///     k3=highest(high,boxp-2)
    ///
    ///     NH =  valuewhen(high>k1[1],high,0)
    ///     box1 =k3<k2
    ///     TopBox = valuewhen(barssince(high>k1[1])==boxp-2 and box1, NH, 0)
    ///     BottomBox = valuewhen(barssince(high>k1[1])==boxp-2 and box1, LL, 0)
    ///
    ///     plot(TopBox, linewidth=3, color=green, title="TBbox")
    ///     plot(BottomBox, linewidth=3, color=red, title="BBbox")
    /// </summary>
    public class DarvasBox
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <returns>Calculated indicator series.</returns>
        public static DarvasBoxKDto Calculate(double[] high, double[] low,  int period)
        {
            Condition.Requires(high, "high").IsNotEmpty();
            Condition.Requires(low, "low").IsNotEmpty();

            var result = new DarvasBoxKDto()
            {
                BottomBox = new double[high.Length],
                TopBox = new double[high.Length]
            };

            double K1_1 = 0;
            int barsSince = 0;
            double NewHigh = 0;
            double topBoxValue = 0;
            double bottomBoxValue = 0;

            for (int i = period; i < high.Length; i++)
            {
                var last5LowPrice = low.Skip(i - period + 1).Take(period);
                var last5HighPrice = high.Skip(i - period + 1).Take(period);
                var last4Price = high.Skip(i - period + 2).Take(period - 1);
                var last3Price = high.Skip(i - period + 3).Take(period - 2);

                var LowestLow = last5LowPrice.Min();


                var K1 = last5HighPrice.Max();
                var K2 = last4Price.Max();
                var K3 = last3Price.Max();

                //NH =  valuewhen(high>k1[1],high,0)
                //high degerinin k1'den ilk buyuk oldugu mum'un high degeri
                if (K1_1 != 0 && high[i] > K1_1)
                    NewHigh = high[i];
                
                var box1 = K3 < K2;

                if (K1_1 > high[i]) barsSince += 1;
                else barsSince = 0;
                
                K1_1 = K1;

                //TopBox = valuewhen(barssince(high>k1[1])==boxp-2 and box1, NH, 0)
                if (barsSince == period - 2 && box1)
                {
                    topBoxValue = NewHigh;
                    bottomBoxValue = LowestLow;
                }

                result.TopBox[i] = topBoxValue;
                result.BottomBox[i] = bottomBoxValue;
            }

            return result;
        }
    }
}
