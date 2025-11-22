using SomeTrade.Indicators;
using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies.Screening
{
    public class MACrossPriceScreening : ScreeningBase
    {
        //parameters
        private int pMaLength;
        private string pMA;

        public MACrossPriceScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "MACrossPriceScreening",8)
        {
            pMaLength = GetParameterAsInt("MaLength");
            pMA = GetParameter("MA");
        }

        public override ScreeningResultDto Execute()
        {
            var maResult = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMaLength)
                : TA.SMA.Calculate(closeArray, pMaLength);

            var maLast = maResult.Last();
            var maPrev = maResult.TakePrev();

            var values = new Dictionary<string, object> {
                { "MaLast",maLast },
                { "MaPrev",maPrev },
            };

            //oncekinde fiyatin altindaymis, sonuncuda ustune cikmis
            if (maLast > closeArray.Last() && maPrev < closeArray.TakePrev())
                return new ScreeningResultDto(true, $"{pMA}-{pMaLength} Fiyatı [{closeArray.Last()}] yukarı kırdı", values);

            if (maLast < closeArray.Last() && maPrev > closeArray.TakePrev())
                return new ScreeningResultDto(true, $"{pMA}-{pMaLength} Fiyatı [{closeArray.Last()}] aşağı kırdı", values);

            return new ScreeningResultDto(false, "", values);
        }
    }
}
