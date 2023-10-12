using SomeTrade.Indicators;
using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies.Screening
{
    public class MACrossMAScreening : ScreeningBase
    {
        //parameters
        private int pShortMaLength;
        private int pLongMaLength;
        private string pMA;

        public MACrossMAScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "MACrossMAScreening",6)
        {
            pShortMaLength = GetParameterAsInt("ShortMaLength");
            pLongMaLength = GetParameterAsInt("LongMaLength");
            pMA = GetParameter("MA");
        }

        public override ScreeningResultDto Execute()
        {
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            var maShortLast = maShort.Last();
            var maShortPrev = maShort.TakePrev();
            var maLongLast = maLong.Last();
            var maLongPrev = maLong.TakePrev();

            var values = new Dictionary<string, object> {
                { $"MaShortLast[{pShortMaLength}]",maShortLast },
                { $"MaShortPrev[{pShortMaLength}]",maShortPrev },
                { $"MaLongLast[{pLongMaLength}]",maLongLast },
                { $"MaLongPrev[{pLongMaLength}]",maLongPrev },
                { "ClosePrice",closeArray.Last() },
            };

            var priceLast = closeArray.Last();
            var pricePrev = closeArray.TakePrev();

            //oncekinde short long'un altinda, su an ustune cikti
            if (maShort.TakePrev() < maLong.TakePrev() && maShort.Last() > maLong.Last())
                return new ScreeningResultDto(true, $"EMA{pShortMaLength}/EMA{pLongMaLength} ⬆️ \n\n Fiyat: {priceLast} | Onceki: {pricePrev} \n\n", values);

            if (maShort.TakePrev() > maLong.TakePrev() && maShort.Last() < maLong.Last())
                return new ScreeningResultDto(true, $"EMA{pShortMaLength}/EMA{pLongMaLength} ⬇️ \n\n Fiyat: {priceLast} | Onceki: {pricePrev} \n\n", values);

            return new ScreeningResultDto(false, "", values);
        }
    }
}
