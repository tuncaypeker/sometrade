using CuttingEdge.Conditions;

namespace SomeTrade.TA.Indicators
{
    /// <summary>
    /// name:
    ///     Accumulation Distribution Indicator or ADL (Accumulation Distribution Line)
    /// description:
    ///     Volume based indicator which was essentially designed to measure underlying supply and demand.
    /// link:
    ///    https://www.tradingview.com/support/solutions/43000501770-accumulation-distribution-adl/
    /// formula:
    ///     Accumulation/Distribution = ((Close – Low) – (High – Close)) / (High – Low) * Period Volume 
    /// pinescript:
    ///     
    /// </summary>
    public class ADL
    {
        public static double[] Calculate(double[] highArray, double[] lowArray, double[] closeArray, double[] volumeArray)
        {
            Condition.Requires(highArray, "highArray").IsNotEmpty();
            Condition.Requires(lowArray, "lowArray").IsNotEmpty();
            Condition.Requires(closeArray, "closeArray").IsNotEmpty();
            Condition.Requires(volumeArray, "volumeArray").IsNotEmpty();

            //validation
            var result = new double[highArray.Length];
            double ad = 0.0;

            for(int i=0;i<highArray.Length;i++)
            {
                double high = highArray[i];
                double low = lowArray[i];
                double close = closeArray[i];

                var highLowDifference = high - low;
                if (highLowDifference > 0.0)
                    ad += (close - low - (high - close)) / highLowDifference * volumeArray[i];

                result[i] = ad;
            }

            return result;
        }
    }
}
