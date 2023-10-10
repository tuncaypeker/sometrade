namespace SomeTrade.TA.Indicators.Everget
{
    using CuttingEdge.Conditions;
    

    /// <summary>
    /// name:
    ///     Triangular Moving Average (TRIMA)
    ///     Bazı yerlerde TMA olarak da geçiyor haberin olsun, hatta emin olduysam bunu Functions altına direk TMA olarak eklmeiş olabilrim
    /// description:
    ///     
    /// link:
    ///     https://www.tradingtechnologies.com/xtrader-help/x-study/technical-indicator-definitions/triangular-moving-average-trima/
    /// formula:
    ///    MA = ( SMA ( SMAm, Nm ) ) / Nm
    /// kivanc- Hareketli Ortalamalarla Algo Trade:
    ///     Ortadaki verilerin daha agırlıklı olması
    ///     C1 * 1 + C2 * 2 + C3 * 3 + C4 * 2 + C5 *1 / 1+2+3+2+1
    ///     50 üçgensel, desteksel ve direnç. konusunda başarılı
    /// </summary>
    public class TRIMA
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <returns>Calculated indicator series.</returns>
        public static double[] Calculate(double[] price, int period)
        {
            Condition.Requires(price, "price").IsNotEmpty();
            Condition.Requires(period, "period").IsGreaterThan(0).IsLessOrEqual(price.Length);

            var sma1 = TA.SMA.Calculate(price, period);
            var trima = TA.SMA.Calculate(sma1, period);

            return trima;
        }       
    }
}
