namespace SomeTrade.Strategies.Screening
{
    using SomeTrade.Strategies.Dto;
    using SomeTrade.Strategies.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Aynı mum içerisinde açılış fiyatı ile anlık fiyat arasında parametre kadar sıçrama varsa 
    /// bildirir
    /// </summary>
    public class OneCandleJumpScreening : ScreeningBase
    {
         //parameters
        private int pJumpValue;

        public OneCandleJumpScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "OneCandleJumpScreening", 11)
        {
            pJumpValue = GetParameterAsInt("pJumpValue");
        }

        public override ScreeningResultDto Execute()
        {
            double openPriceForThisCandle = openArray.Last();
            double currentPriceForThisCandle = closeArray.Last();

            var values = new Dictionary<string, object> {
                { "open",openPriceForThisCandle },
                { "current",currentPriceForThisCandle },
            };

            //yuzde orani bulalim
            if (openPriceForThisCandle > currentPriceForThisCandle)//fiyat dusmus
            {
                var diff = openPriceForThisCandle - currentPriceForThisCandle;
                var percentage = diff * 100 / openPriceForThisCandle;

                if(percentage > pJumpValue)
                    return new ScreeningResultDto(true, $"Aynı mum içinde %{pJumpValue} düşüş oldu", values);    
            }
            else { //fiyat artmis ya da sabit
                var diff = currentPriceForThisCandle - openPriceForThisCandle;
                var percentage = diff * 100 / openPriceForThisCandle;

                if(percentage > pJumpValue)
                    return new ScreeningResultDto(true, $"Aynı mum içinde %{pJumpValue} artış oldu", values);    
            }

            return new ScreeningResultDto(false, "", values);
        }
    }
}
