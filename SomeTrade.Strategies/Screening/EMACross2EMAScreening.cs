using SomeTrade.Indicators;
using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies.Screening
{
    public class EMACross2EMAScreening : ScreeningBase
    {
        //parameters
        private int pShortEmaLength;
        private int pMiddleEmaLength;
        private int pLongEmaLength;

        public EMACross2EMAScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "EMACross2EMAScreening", 9)
        {
            pShortEmaLength = GetParameterAsInt("ShortEmaLength");
            pMiddleEmaLength = GetParameterAsInt("MiddleEmaLength");
            pLongEmaLength = GetParameterAsInt("LongEmaLength");
        }

        public override ScreeningResultDto Execute()
        {
            var emaShort = TA.EMA.Calculate(closeArray, pShortEmaLength);
            var emaMiddle = TA.EMA.Calculate(closeArray, pMiddleEmaLength);
            var emaLong = TA.EMA.Calculate(closeArray, pLongEmaLength);

            var emaShortLast = emaShort.Last();
            var emaShortPrev = emaShort.TakePrev();
            var emaMiddleLast = emaMiddle.Last();
            var emaMiddlePrev = emaMiddle.TakePrev();
            var emaLongLast = emaLong.Last();
            var emaLongPrev = emaLong.TakePrev();

            var values = new Dictionary<string, object> {
                { "EmaShortLast",emaShortLast },
                { "EmaShortPrev",emaShortPrev },
                { "EmaMiddleLast",emaMiddleLast },
                { "EmaMiddlePrev",emaMiddlePrev },
                { "EmaLongLast",emaLongLast },
                { "EmaLongPrev",emaLongPrev }
            };

            //oncekinde short middle ve long'un altinda, su an ustune cikti
            if (emaShort.TakePrev() < emaMiddle.TakePrev() && emaShort.TakePrev() < emaLong.TakePrev()
                && emaShort.Last() > emaMiddle.Last() && emaShort.Last() > emaLong.Last())
                return new ScreeningResultDto(true, $"EMA{pShortEmaLength}, EMA{pMiddleEmaLength} ve EMA{pLongEmaLength}'i yukarı kırdı", values);

            if (emaShort.TakePrev() > emaMiddle.TakePrev() && emaShort.TakePrev() > emaLong.TakePrev()
                && emaShort.Last() < emaMiddle.Last() && emaShort.Last() < emaLong.Last())
                return new ScreeningResultDto(true, $"EMA{pShortEmaLength}, EMA{pMiddleEmaLength} ve EMA{pLongEmaLength}'i aşağı kırdı", values);

            return new ScreeningResultDto(false, "", values);
        }
    }
}
