using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies.Screening
{
    public class PriceAlertScreening : ScreeningBase
    {

        //parameters
        private double pPrice;

        public PriceAlertScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "PriceAlertScreening",8)
        {
            pPrice = GetParameterAsDouble("Price");
        }

        public override ScreeningResultDto Execute()
        {
            //1- Onceki kapanis fiyatın altında ya da eşittir, son kapanis fiyatın ustunde ya da esittir
            //2- Onceki kapanis fiyatin usunde  ya da eşittir, son kapanis fiyatın altında ya da esittir
            //3- Fiyat son mum icerisinde low pricedan dusuk ya da esit high pricedan buyuk ya da esit ise

            var prevClose = this.closeArray.TakePrev();
            var lastClose = this.closeArray.Last();
            var lastLow = this.lowArray.TakePrev();
            var lastHigh = this.highArray.TakePrev();

            var values = new Dictionary<string, object> {
                { "PrevClose",prevClose },
                { "LastClose",lastClose },
            };

            //oncekinde fiyatin altindaymis, sonuncuda ustune cikmis
            if (prevClose <= pPrice && lastClose >= pPrice)
                return new ScreeningResultDto(true, $"{pPrice} fiyata dokundu", values);

            if (prevClose >= pPrice && lastClose <= pPrice)
                return new ScreeningResultDto(true, $"{pPrice} fiyata dokundu", values);

            if (lastLow <= pPrice && lastHigh >= pPrice)
                return new ScreeningResultDto(true, $"{pPrice} fiyata dokundu", values);

            return new ScreeningResultDto(false, "", values);
        }
    }
}
