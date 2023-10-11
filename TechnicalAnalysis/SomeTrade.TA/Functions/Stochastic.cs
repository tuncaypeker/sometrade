namespace SomeTrade.TA
{
    using CuttingEdge.Conditions;
	using SomeTrade.TA.Dto;
	using System.Linq;

    /// <summary>
    /// Stochastic Oscillator Indicator.
    /// name:
    ///     
    /// description:
    ///     Serkan bunu cok kullaniyor. 20-80 kanali kullaniliyor. 
    ///         Al sinyali için stochastic dip bölgesinde olmali.
    /// link:
    ///     
    /// formula:
    ///     k% : (C-14/H14-L14)/*100
    ///     d% : SMA(k%,smoothD) 
    /// pinescript:
    /*
    
    //@version=5
    indicator(title="Stochastic", shorttitle="Stoch", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
    periodK = input.int(14, title="%K Length", minval=1)
    smoothK = input.int(1, title="%K Smoothing", minval=1)
    periodD = input.int(3, title="%D Smoothing", minval=1)
    k = ta.sma(ta.stoch(close, high, low, periodK), smoothK)
    d = ta.sma(k, periodD)
    plot(k, title="%K", color=#2962FF)
    plot(d, title="%D", color=#FF6D00)
    h0 = hline(80, "Upper Band", color=#787B86)
    hline(50, "Middle Band", color=color.new(#787B86, 50))
    h1 = hline(20, "Lower Band", color=#787B86)
    fill(h0, h1, color=color.rgb(33, 150, 243, 90), title="Background")

     */ 
    /// </summary>
    public class Stochastic
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="smoothD">%D Smoothing.</param>
        /// <returns>Calculated indicator series.</returns>
        public static StochasticResultDto Calculate(double[] price, double[] high, double[] low, int period, int smoothK = 3)
        {
            Condition.Requires(price, "price")
                .IsNotEmpty();
            Condition.Requires(period, "period")
                .IsGreaterThan(0)
                .IsLessOrEqual(price.Length);

            //ta.stoch(close, high, low, periodK)
            var taStoch = new double[price.Length];
            for (int i = period - 1; i < price.Length; i++)
            {
                var skip = i - period + 1;

                var h14 = high.ToList().Skip(skip).Take(period).Max();
                var l14 = low.ToList().Skip(skip).Take(period).Min();

                taStoch[i] = (price[i] - l14) / (h14 - l14) * 100;
            }

            var result = new StochasticResultDto();

            result.K = taStoch;
            result.D = TA.SMA.Calculate(result.K, smoothK);

            return result;
        }       
    }
}
