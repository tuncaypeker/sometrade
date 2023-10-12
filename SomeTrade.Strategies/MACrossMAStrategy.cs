using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    /// <summary>
    /// AL: MA20-MA50 yi yukari kirarsa AL
    ///     STOP: MA20-MA50 yi asagi kirarsa SAT
    /// SAT: SELL sinyalinin gelmesi ile short girer
    ///     STOP: BUY sinyali ile pozisyonu kapatir
    /// </summary>
    public class MACrossMAStrategy : StrategyBase
    {
        //parameters
        private int pShortMaLength;
        private int pLongMaLength;
        private string pMA;

        public MACrossMAStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "MACrossMAStrategy", 6)
        {
            pShortMaLength = GetParameterAsInt("ShortMaLength");
            pLongMaLength = GetParameterAsInt("LongMaLength");
            pMA = GetParameter("MA");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            //# Variables
            var maShortLast = maShort.Last();
            var maShortPrev = maShort.TakePrev();
            var maLongLast = maLong.Last();
            var maLongPrev = maLong.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                  { $"MA", pMA },
                  { $"MaShort[{pShortMaLength}]1", $"{maShortPrev.RoundTo(5)}" },
                  { $"MaShort[{pShortMaLength}]2", $"{maShortLast.RoundTo(5)}" },
                  { $"MaLong[{pLongMaLength}]1", $"{maLongPrev.RoundTo(5)}" },
                  { $"MaLong[{pLongMaLength}]2", $"{maLongLast.RoundTo(5)}" },
                  { "ClosePrice",closeArray.Last() },
                },
                Result = false
            };

            //# Decision
            if (maShort.TakePrev() < maLong.TakePrev() && maShort.Last() > maLong.Last())
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
            //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            //# Variables
            var maShortLast = maShort.Last();
            var maShortPrev = maShort.TakePrev();
            var maLongLast = maLong.Last();
            var maLongPrev = maLong.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                  { $"MA", pMA },
                  { $"MaShort[{pShortMaLength}]1", $"{maShortPrev.RoundTo(5)}" },
                  { $"MaShort[{pShortMaLength}]2", $"{maShortLast.RoundTo(5)}" },
                  { $"MaLong[{pLongMaLength}]1", $"{maLongPrev.RoundTo(5)}" },
                  { $"MaLong[{pLongMaLength}]2", $"{maLongLast.RoundTo(5)}" },
                  { "ClosePrice",closeArray.Last() },
                },
                Result = false
            };

            //# Decision
            if (maShort.TakePrev() > maLong.TakePrev() && maShort.Last() < maLong.Last())
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldTakeProfitLong()
        {
            return ShouldOpenShort();
        }

        public override CheckResult ShouldTakeProfitShort()
        {
             return ShouldOpenLong();
        }

        public override bool CheckAlarm()
        {
            return false;
        }

        public override bool UpdateTrailingStop()
        {
            return false;
        }

        public override bool ShouldStop(double entryPrice, int side)
        {
            return false;
        }
    }
}
