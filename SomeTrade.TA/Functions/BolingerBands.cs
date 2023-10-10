namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Dto;

    /// <summary>
    /// name:
    ///     
    /// description:
    ///     
    /// link:
    ///     
    /// formula:
    ///     
    /// pinescript:
    ///     
    /*
     
    //@version=5
    indicator(shorttitle="BB", title="Bollinger Bands", overlay=true, timeframe="", timeframe_gaps=true)
    length = input.int(20, minval=1)
    src = input(close, title="Source")
    mult = input.float(2.0, minval=0.001, maxval=50, title="StdDev")
    basis = ta.sma(src, length)
    dev = mult * ta.stdev(src, length)
    upper = basis + dev
    lower = basis - dev
    offset = input.int(0, "Offset", minval = -500, maxval = 500)
    plot(basis, "Basis", color=#FF6D00, offset = offset)
    p1 = plot(upper, "Upper", color=#2962FF, offset = offset)
    p2 = plot(lower, "Lower", color=#2962FF, offset = offset)
    fill(p1, p2, title = "Background", color=color.rgb(33, 150, 243, 95))

     */
    /// </summary>
    public class BolingerBands
    {
        public static BolingerBandsResultDto Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotNull().IsNotEmpty();

            var result = new Dto.BolingerBandsResultDto()
            {
                LowerBand = new double[price.Length],
                MiddleBand = new double[price.Length],
                UpperBand = new double[price.Length]
            };

            result.MiddleBand = TA.SMA.Calculate(price, period);
            var stdDev = TA.STDDev.Calculate(price, period);

            for (int i = period; i < price.Length; i++)
            {
                var dev = 2.0 * stdDev[i];

                result.UpperBand[i] = result.MiddleBand[i] + dev;
                result.LowerBand[i] = result.MiddleBand[i] - dev;
            }

            return result;
        }
    }
}
