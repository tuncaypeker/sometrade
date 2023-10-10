namespace SomeTrade.TA.Indicators.KivancOzbilgic
{
    using CuttingEdge.Conditions;
    using SomeTrade.TA.Indicators.Dto;

    /// <summary>
    /// name:
    /// description:
    /// </summary>
    public class ALPHATREND
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
        public static AlphaTrendResultDto Execute(double[] close, double[] high, double[] low, double[] volume
            , int length = 14, double coeff = 1.0, bool novolumedata = false)
        {
            Condition.Requires(close, "close").IsNotNull().IsNotEmpty();
            Condition.Requires(high, "high").IsNotNull().IsNotEmpty();
            Condition.Requires(low, "low").IsNotNull().IsNotEmpty();
            Condition.Requires(volume, "volume").IsNotNull().IsNotEmpty();
            Condition.Requires(coeff, "vFactor").IsGreaterThan(0.0);

            var TRArray = TA.TR.Execute(high, low, close);
            var ATRArray = TA.SMA.Calculate(TRArray, length);
            var RSIArray = TA.RSI.Calculate(close, length);
            var MFIArray = TA.MFI.Execute(high, low, close, volume, length);

            var uptArray = new double[close.Length];
            var downTArray = new double[close.Length];

            for (int i = 0; i < close.Length; i++)
            {
                uptArray[i] = low[i] - ATRArray[i] * coeff;
                downTArray[i] = high[i] + ATRArray[i] * coeff;
            }

            /* 
                 AlphaTrend := (novolumedata 
                                    ? ta.rsi(src, AP) >= 50
                                    : ta.mfi(hlc3, AP) >= 50
                                ) 
                            ? upT < nz(AlphaTrend[1]) 
                                ? nz(AlphaTrend[1]) 
                                : upT 
                            : downT > nz(AlphaTrend[1]) 
                                ? nz(AlphaTrend[1]) 
                                : downT
            */
            var alphaTrend = new double[close.Length];
            var alphaTrendK2 = new double[close.Length];
            var color = new string[close.Length];
            var buySignal = new int[close.Length];
            var sellSignal = new int[close.Length];

            for (int i = 3; i < close.Length; i++)
            {
                var isUp = novolumedata ? RSIArray[i] >= 50 : MFIArray[i] >= 50;

                if (isUp)
                {
                    alphaTrend[i] = uptArray[i] < alphaTrend[i - 1]
                        ? alphaTrend[i - 1]
                        : uptArray[i];
                }
                else
                {
                    alphaTrend[i] = downTArray[i] > alphaTrend[i - 1]
                        ? alphaTrend[i - 1]
                        : downTArray[i];
                }

                color[i] = alphaTrend[i] > alphaTrend[i - 2]
                    ? "#00E60F"
                    : alphaTrend[i] < alphaTrend[i - 2]
                        ? "#80000B"
                        : alphaTrend[i - 1] < alphaTrend[i - 3] ? "#00E60F" : "#80000B";

                alphaTrendK2[i] = alphaTrend[i - 2];

                //cross over
                if (alphaTrend[i - 1] <= alphaTrendK2[i - 1] && alphaTrend[i] > alphaTrendK2[i])
                    buySignal[i] = 1;

                if (alphaTrend[i - 1] >= alphaTrendK2[i - 1] && alphaTrend[i] < alphaTrendK2[i])
                    sellSignal[i] = 1;
            }

            return new AlphaTrendResultDto()
            {
                AlphaTrend = alphaTrend,
                AlphaTrendK2 = alphaTrendK2,
                BuySignal = buySignal,
                Color = color,
                SellSignal = sellSignal
            };
        }
    }
}
