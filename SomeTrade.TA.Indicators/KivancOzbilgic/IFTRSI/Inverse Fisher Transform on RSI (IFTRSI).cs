
namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    using CuttingEdge.Conditions;

    /// <summary>
    /// name:
    ///    INVERSE FISHER TRANSFORM
    /// description:
    ///     About INVERSE FISHER TRANSFORM:
    ///
    ///      The purpose of technical indicators is to help with your timing decisions to buy or
    ///     sell. Hopefully, the signals are clear and unequivocal. However, more often than
    ///     not your decision to pull the trigger is accompanied by crossing your fingers.
    ///     Even if you have placed only a few trades you know the drill.
    ///     In this article I will show you a way to make your oscillator-type indicators make
    ///     clear black-or-white indication of the time to buy or sell. I will do this by using the
    ///     Inverse Fisher Transform to alter the Probability Distribution Function ( PDF ) of
    ///     your indicators. In the past12 I have noted that the PDF of price and indicators do
    ///     not have a Gaussian, or Normal, probability distribution. A Gaussian PDF is the
    ///     familiar bell-shaped curve where the long “tails” mean that wide deviations from
    ///     the mean occur with relatively low probability. The Fisher Transform can be
    ///     applied to almost any normalized data set to make the resulting PDF nearly
    ///     Gaussian, with the result that the turning points are sharply peaked and easy to
    ///     identify. The Fisher Transform is defined by the equation
    ///     1)
    ///     Whereas the Fisher Transform is expansive, the Inverse Fisher Transform is
    ///     compressive. The Inverse Fisher Transform is found by solving equation 1 for x
    ///     in terms of y. The Inverse Fisher Transform is:
    ///     2)
    ///     The transfer response of the Inverse Fisher Transform is shown in Figure 1. If
    ///     the input falls between –0.5 and +0.5, the output is nearly the same as the input.
    ///     For larger absolute values (say, larger than 2), the output is compressed to be no
    ///     larger than unity . The result of using the Inverse Fisher Transform is that the
    ///     output has a very high probability of being either +1 or –1. This bipolar
    ///     probability distribution makes the Inverse Fisher Transform ideal for generating
    ///     an indicator that provides clear buy and sell signals.
    /// link:
    ///    https://tr.tradingview.com/script/LxiuxNm4/
    ///    https://www.youtube.com/watch?v=jK1bq4r6FQs => optimizasyon için cok onemli
    /// formula:
    ///     
    /// pinescript:
    ///     //@version=3
    ///     // author: KIVANC @fr3762 on twitter
    ///     // creator John EHLERS
    ///     //
    ///     study("Inverse Fisher Transform on RSI", shorttitle="IFTRSI")
    ///     rsilength=input(5, "RSI Length")
    ///     wmalength=input(9, title="Smoothing length")
    ///     v1=0.1*(rsi(close, rsilength)-50)
    ///     v2=wma(v1,wmalength)
    ///     INV=(exp(2*v2)-1)/(exp(2*v2)+1)
    ///     plot(INV, color=green, linewidth=2)
    ///     hline(0.5, color=red)
    ///     hline(-0.5, color=green)
    /// </summary>
    public class IFTRSI
    {
        public static double[] Calculate(double[] closeArray, int rsiLength, int wmaLength)
        {
            Condition.Requires(closeArray, "closeArray").IsNotEmpty();

            var rsiArray = TA.RSI.Calculate(closeArray, rsiLength);
            var v1 = new double[closeArray.Length];
            for (int i = 0; i < closeArray.Length; i++)
                 v1[i] = 0.1 * (rsiArray[i] - 50);

            var v2 = TA.WMA.Calculate(v1, wmaLength);

            //validation
            var INV = new double[closeArray.Length];

            for (int i = 0; i < closeArray.Length; i++)
            {
                INV[i] = (System.Math.Exp(2 * v2[i]) - 1) / (System.Math.Exp(2 * v2[i]) + 1);
            }

            return INV;
        }
    }
}
