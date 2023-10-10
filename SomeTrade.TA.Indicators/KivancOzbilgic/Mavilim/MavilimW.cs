using CuttingEdge.Conditions;
using SomeTrade.TA.Indicators.Dto;

namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    /// <summary>
    /// pinescript:
    ///     //@version=4
    ///     //inspired by: @mavilim0732 on twitter
    ///     //creator&author: KIVANÇ ÖZBİLGİÇ
    ///     study("MavilimW", overlay=true)
    ///     mavilimold = input(false, title="Show Previous Version of MavilimW?")
    ///     fmal=input(3,"First Moving Average length")
    ///     smal=input(5,"Second Moving Average length")
    ///     tmal=fmal+smal
    ///     Fmal=smal+tmal
    ///     Ftmal=tmal+Fmal
    ///     Smal=Fmal+Ftmal
    ///
    ///     M1= wma(close, fmal)
    ///     M2= wma(M1, smal)
    ///     M3= wma(M2, tmal)
    ///     M4= wma(M3, Fmal)
    ///     M5= wma(M4, Ftmal)
    ///     MAVW= wma(M5, Smal)
    ///     col1= MAVW>MAVW[1]
    ///     col3= MAVW<MAVW[1]
    ///     colorM = col1 ? color.blue : col3 ? color.red : color.yellow
    ///
    ///     plot(MAVW, color=colorM, linewidth=2, title="MAVW")
    ///
    ///     M12= wma(close, 3)
    ///     M22= wma(M12, 5)
    ///     M32= wma(M22, 8)
    ///     M42= wma(M32, 13)
    ///     M52= wma(M42, 21)
    ///     MAVW2= wma(M52, 34)
    ///
    ///     plot(mavilimold and MAVW2 ? MAVW2 : na, color=color.blue, linewidth=2, title="MavWOld")
    ///
    ///     alertcondition(crossover(MAVW,MAVW[1]), title="MAVW BUY", message="MAVW BUY!")
    ///     alertcondition(crossunder(MAVW,MAVW[1]), title="MAVW SELL", message="MAVW SELL!")
    ///
    ///     alertcondition(cross(MAVW,MAVW[1]), title="Color ALARM", message="MavilimW has changed color!")
    /// </summary>
    public class MavilimW
    {
        public static MavilimWResultDto Calculate(double[] price, int firstLength, int secondLength)
        {
            var fmal = firstLength;
            var smal = secondLength;
            var tmal = fmal + smal;
            var Fmal = smal + tmal;
            var Ftmal = tmal + Fmal;
            var Smal = Fmal + Ftmal;

            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(firstLength, "firstLength")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);
            Condition.Requires(secondLength, "secondLength")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            var M1 = SomeTrade.TA.WMA.Calculate(price, fmal);
            var M2 = SomeTrade.TA.WMA.Calculate(M1, smal);
            var M3 = SomeTrade.TA.WMA.Calculate(M2, tmal);
            var M4 = SomeTrade.TA.WMA.Calculate(M3, Fmal);
            var M5 = SomeTrade.TA.WMA.Calculate(M4, Ftmal);
            var MAVW = SomeTrade.TA.WMA.Calculate(M5, Smal);

            var result = new MavilimWResultDto()
            {
                Color = new string[price.Length],
                Result = new double[price.Length]
            };

            for (int i = 1; i < price.Length; i++)
            {
                result.Result[i] = MAVW[i];

                var col1 = MAVW[i] > MAVW[i - 1];
                var col3 = MAVW[i] < MAVW[i - 1];
                result.Color[i] = col1 
                    ? "blue"
                    : col3
                        ? "red"
                        : "yellow";
            }

            return result;
        }
    }
}
