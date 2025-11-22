namespace SomeTrade.TA.Indicators.Serkan
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;
    using System.Linq;

    /// <summary>
    /// name:
    ///    Serkan average
    /// description:
    ///     
    /// link:
    ///    
    /// formula:
    ///     
    /// pinescript:
    ///    //@version=3
    ///     study("Serkan Average", shorttitle="SERKA", overlay=true)
    ///
    ///     length = input(title="Length", type=integer, defval=14)
    ///     fastLength = input(title="Fast EMA Length", type=integer, defval=2)
    ///     slowLength = input(title="Slow EMA Length", type=integer, defval=30)
    ///     highlightMovements = input(title="Highlight Movements ?", type=bool, defval=true)
    ///     src = input(title="Source", type=source, defval=close)
    ///
    ///     mom = abs(change(src, length))
    ///     volatility = sum(abs(change(src)), length)
    ///
    ///     // Efficiency Ratio
    ///     er = volatility != 0 ? mom / volatility : 0
    ///
    ///     fastAlpha = 2 / (fastLength + 1)
    ///     slowAlpha = 2 / (slowLength + 1)
    ///
    ///     // KAMA Alpha
    ///     sc = pow((er * (fastAlpha - slowAlpha)) + slowAlpha, 2)
    ///
    ///     kama = 0.0
    ///     kama := sc * src + (1 - sc) * nz(kama[1])
    ///
    ///     kamaColor = highlightMovements ? (kama > kama[1] ? green : red) : #6d1e7f
    ///     plot(kama, title="KAMA", linewidth=2, color=kamaColor, transp=0)
    ///
    /// </summary>
    public class SERKA
    {
        public static SerkaResultDto Calculate(double[] closeArray, int length, int fastLength, int slowLength)
        {
            Condition.Requires(closeArray, "closeArray").IsNotEmpty();

            var momArray = new double[closeArray.Length];
            for (int i = length; i < closeArray.Length - 1; i++)
                momArray[i] = System.Math.Abs(closeArray[i] - closeArray[i - length]);

            //math.abs(ta.change(src))
            var diffArray = new double[closeArray.Length];
            for (int i = 1; i < closeArray.Length; i++)
                diffArray[i] = System.Math.Abs(closeArray[i] - closeArray[i - 1]);

            var volatilityArray = new double[closeArray.Length];
            var erArray = new double[closeArray.Length];
            for (int i = length; i < closeArray.Length; i++) {

                volatilityArray[i - 1] = diffArray.Skip(i - length).Take(length).Sum();
                erArray[i-1] = volatilityArray[i-1] != 0
                    ? momArray[i-1] / volatilityArray[i-1]
                    : 0;
            }

            double fastAlpha = 2.0 / (fastLength + 1);
            double slowAlpha = 2.0 / (slowLength + 1);

            var scArray = new double[closeArray.Length];
            for (int i = length; i < closeArray.Length; i++)
                scArray[i] = System.Math.Pow((erArray[i] * (fastAlpha - slowAlpha)) + slowAlpha, 2);

            var serkaResult = new SerkaResultDto()
            {
                Color = new string[closeArray.Length],
                Result = new double[closeArray.Length]
            };

            for (int i = 1; i < closeArray.Length; i++) { 
                serkaResult.Result[i] = scArray[i] * closeArray[i] + (1 - scArray[i]) * serkaResult.Result[i - 1];
                serkaResult.Color[i] = serkaResult.Result[i] > serkaResult.Result[i - 1] ? "green" : "red";
            }

            return serkaResult;
        }
    }
}
