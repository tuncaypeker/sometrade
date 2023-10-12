namespace SomeTrade.Strategies.Screening
{
    using SomeTrade.Indicators;
    using SomeTrade.Strategies.Dto;
    using SomeTrade.Strategies.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Gun icinde al-sat, gir-cik stratejileri icin kullanilabilir.
    /// iki MA arasında kalınca kırılımın gercekleştği yöne dogru pozisyon, Kivanc Ozbilgic zamaninda cok kullandim ise yariyor diyor
    /// sma20 sma50 denenbilir
    /// stopda yine sma'nın kendisi olur 
    /// dakikalıkda sma200-sma500
    /// </summary>
    public class BetweenTwoSmaScreening : ScreeningBase
    {
         //parameters
        private int pSmaShortLength;
        private int pSmaLongLength;

        public BetweenTwoSmaScreening(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "BetweenTwoSmaScreening", 7)
        {
            pSmaShortLength = GetParameterAsInt("SmaShortLength");
            pSmaLongLength = GetParameterAsInt("SmaLongLength");
        }

        public override ScreeningResultDto Execute()
        {
            var smaShort = TA.SMA.Calculate(closeArray, pSmaShortLength);
            var smaLong = TA.SMA.Calculate(closeArray, pSmaLongLength);

            var smaShortPrev = smaShort.TakePrev();
            var smaShortLast = smaShort.Last();
            
            var smaLongPrev = smaLong.TakePrev();
            var smaLongLast = smaLong.Last();

            var priceLast = closeArray.Last();
            var pricePrev = closeArray.TakePrev();

            var values = new Dictionary<string, object> {
                { "smaShortPrev",smaShortPrev },
                { "smaShortLast",smaShortLast },
                { "pricePrev",pricePrev },
                { "priceLast",priceLast },
                { "smaLongPrev",smaLongPrev },
                { "smaLongLast",smaLongLast }
            };

            //fiyat oncekinde kanal icinde olmali, sonrakinde asagi ya da yukari kirmali

            //onceki bar'da uzun bar ustte, kisa bar fiyat altinda ve 
            if ((smaLongPrev > pricePrev && pricePrev > smaShortPrev)) {
                //o zmn son bar'da bi yone kirilma var mi bakalim
                if (priceLast > smaLongLast)//fiyat uzun bar'i uste kirdi
                   return new ScreeningResultDto(true, $"İki SMA Arasi Kirildi: [SMA{pSmaShortLength}:SMA{pSmaLongLength} {smaShortLast}/{priceLast}/{smaLongLast}]", values);    
                else if (priceLast < smaShortLast)//fiyat kisa bar'i alta kirdi
                   return new ScreeningResultDto(true, $"İki SMA Arasi Kirildi: [SMA{pSmaShortLength}:SMA{pSmaLongLength} {smaShortLast}/{priceLast}/{smaLongLast}]", values);    
            }
            
            //onceki bar'da kisa bar altta, uzun bar fiyat ustunde ve 
            if ((smaShortPrev > pricePrev && pricePrev > smaLongPrev)) {
                //o zmn son bar'da bi yone kirilma var mi bakalim
                if (priceLast > smaShortLast)//fiyat kisa bar'i uste kirdi
                   return new ScreeningResultDto(true, $"İki SMA Arasi Kirildi: [SMA{pSmaShortLength}:SMA{pSmaLongLength} {smaShortLast}/{priceLast}/{smaLongLast}]", values);    
                else if (priceLast < smaLongLast)//fiyat uzun bar'i alta kirdi
                    return new ScreeningResultDto(true, $"İki SMA Arasi Kirildi: [SMA{pSmaShortLength}:SMA{pSmaLongLength} {smaShortLast}/{priceLast}/{smaLongLast}]", values);    
            }

            return new ScreeningResultDto(false, "", values);
        }
    }
}
