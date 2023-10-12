using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using SomeTrade.TA.Indicators.KivancOzbilgic;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies.Screening
{
    public class AlphaTrendBuySellScreening : ScreeningBase
    {
        //parameters
        private int pAlphaTrendLength;
        private double pAlphaTrendCoeff;

        public AlphaTrendBuySellScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "AlphaTrendBuySellScreening", 7)
        {
            pAlphaTrendLength = GetParameterAsInt("AlphaTrendLength");
            pAlphaTrendCoeff = GetParameterAsDouble("AlphaTrendCoeff");
        }

        public override ScreeningResultDto Execute()
        {
            var alphaTrend = ALPHATREND.Execute(closeArray, highArray, lowArray, volumeArray
                , pAlphaTrendLength, pAlphaTrendCoeff, novolumedata: false);

            var values = new Dictionary<string, object> {
                { "BuySignal",alphaTrend.BuySignal.Last() },
                { "SellSignal",alphaTrend.SellSignal.Last() },
                { "LastPrice",closeArray.Last() },
            };

            //oncekinde short long'un altinda, su an ustune cikti
            if (alphaTrend.BuySignal.Last() == 1)
                return new ScreeningResultDto(true, $"AlphaTrend Al Sinyali Verdi [{closeArray.Last()}]", values);

            if (alphaTrend.SellSignal.Last() == 1)
                return new ScreeningResultDto(true, $"AlphaTrend Sat Sinyali Verdi [{closeArray.Last()}]", values);

            return new ScreeningResultDto(false, "", values);
        }
    }
}
