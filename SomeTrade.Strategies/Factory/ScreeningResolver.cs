using SomeTrade.Strategies.Interfaces;
using SomeTrade.Strategies.Screening;
using System.Collections.Generic;

namespace SomeTrade.Strategies
{
    public interface IScreeningResolver
    {
        IScreening GetScreeningByName(string name, List<Dto.Candle> candles, Dictionary<string, object> parameters);
        IScreening GetScreeningById(int id, List<Dto.Candle> candles, Dictionary<string, object> parameters);
    }

    public class ScreeningResolver : IScreeningResolver
    {
        public IScreening GetScreeningByName(string name, List<Dto.Candle> candles, Dictionary<string, object> parameters)
        {
            switch (name)
            {
                case "EMACrossEMAScreening":
                    return new MACrossMAScreening(candles, parameters);
                case "AlphaTrendBuySellScreening":
                    return new AlphaTrendBuySellScreening(candles, parameters);
                case "MACrossPriceScreening":
                    return new MACrossPriceScreening(candles, parameters);
                case "EMACross2EMAScreening":
                    return new EMACross2EMAScreening(candles, parameters);
                case "BetweenTwoSmaScreening":
                    return new BetweenTwoSmaScreening(candles, parameters);
                case "OneCandleJumpScreening":
                    return new OneCandleJumpScreening(candles, parameters);
                case "PriceAlertScreening":
                    return new PriceAlertScreening(candles, parameters);
                default: return null;
            }
        }

        public IScreening GetScreeningById(int id, List<Dto.Candle> candles, Dictionary<string, object> parameters)
        {
            switch (id)
            {
                case 6: return new MACrossMAScreening(candles, parameters);
                case 7: return new AlphaTrendBuySellScreening(candles, parameters);
                case 8: return new MACrossPriceScreening(candles, parameters);
                case 9: return new EMACross2EMAScreening(candles, parameters);
                case 10: return new BetweenTwoSmaScreening(candles, parameters);
                case 11: return new OneCandleJumpScreening(candles, parameters);
                case 12: return new PriceAlertScreening(candles, parameters);
                default: return null;
            }
        }
    }
}
