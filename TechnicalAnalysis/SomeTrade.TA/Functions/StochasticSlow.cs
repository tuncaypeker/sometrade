namespace SomeTrade.TA
{
    using SomeTrade.TA.Dto;

    /// <summary>
    /// Stochastic Slow Oscillator Indicator.
    /// name:
    ///     
    /// description:
    ///     Anıl bunu OTT ile birlikte cok kullanıyor
    ///     Normal Stochastic sonuclarında d% degeri k% olarak kabul edilir, yeni d% için ise yeni k% nin yine hareketli ortalaması kullanılır
    /// link:
    ///     
    /// formula:
    ///     k% : SMA((C-14/H14-L14)/*100,smoothK) 
    ///     d% : SMA(k%,smoothD) 
    ///     
    /// pinescript:
    /*
    
        study(title="Slow Stochastic", shorttitle="SlowStoch")
        smoothK = input(14, minval=1), smoothD = input(3, minval=1)
        k = sma(stoch(close, high, low, smoothK), 3)
        d = sma(k, smoothD)
        plot(k, color=black)
        plot(d, color=red)
        h0 = hline(80)
        h1 = hline(20)
        fill(h0, h1, color=purple, transp=95)

     */
    /// </summary>
    public class StochasticSlow
    {
        /// <summary>
        /// Calculates indicator.
        /// </summary>
        /// <param name="price">Price series.</param>
        /// <param name="period">Indicator period.</param>
        /// <param name="smoothK">%K Smoothing.</param>
        /// <param name="smoothD">%D Smoothing.</param>
        /// <returns>Calculated indicator series.</returns>
        public static StochasticResultDto Calculate(double[] price, double[] high, double[] low, int period, int smoothK = 3, int smoothD = 3)
        {
            var stochastic = TA.Stochastic.Calculate(price, high, low, period, smoothK);

            var result = new StochasticResultDto();

            result.K = stochastic.D; //K degerimiz artik stochastik'e ait D Degeri
            result.D = TA.SMA.Calculate(result.K, smoothD);

            return result;
        }
    }
}
