using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    /// <summary>
    /// AL: MA20-Fiyati yukari kirarsa AL
    ///     STOP: MA20-Fiyati asagi kirarsa STOP
    /// SAT: SELL Fiyat-MA20 asagi kirarsa SAT
    ///     STOP: Fiyat-MA20 yukari kirarsa STOP
    /// </summary>
    public class MACrossPriceStrategy : StrategyBase
    {
        //parameters
        private int pMaLength;
        private string pMA;

        public MACrossPriceStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "MACrossPriceStrategy", 7)
        {
            pMaLength = GetParameterAsInt("MaLength");
            pMA = GetParameter("MA");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var maResult = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMaLength)
                : TA.SMA.Calculate(closeArray, pMaLength);

            //# Variables
            var maLast = maResult.Last();
            var maPrev = maResult.TakePrev();
            var closePriceLast = closeArray.Last();
            var closePricePrev = closeArray.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"MatLast[{pMaLength}]",maLast },
                    { $"MaPrev[{pMaLength}]",maPrev },
                    { "ClosePriceLast",closePriceLast },
                    { "ClosePricePrev",closePricePrev }
                },
                Result = false
            };

            //# Decision
            if (closePricePrev < maPrev && closePriceLast > maLast)
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
           //# Technical Analysis
            var maResult = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMaLength)
                : TA.SMA.Calculate(closeArray, pMaLength);

            //# Variables
            var maLast = maResult.Last();
            var maPrev = maResult.TakePrev();
            var closePriceLast = closeArray.Last();
            var closePricePrev = closeArray.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"MatLast[{pMaLength}]",maLast },
                    { $"MaPrev[{pMaLength}]",maPrev },
                    { "ClosePriceLast",closePriceLast },
                    { "ClosePricePrev",closePricePrev }
                },
                Result = false
            };

            //# Decision
            if (closePricePrev > maPrev && closePriceLast < maLast)
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldCloseLong()
        {
            //# Technical Analysis
            var maResult = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMaLength)
                : TA.SMA.Calculate(closeArray, pMaLength);

             //# Variables
            var maLast = maResult.Last();
            var closePriceLast = closeArray.Last();

            var checkResult = new CheckResult()
            {
                 PositionLogs = new Dictionary<string, object>() {
                   { $"MatLast[{pMaLength}]",maLast },
                    { "ClosePriceLast",closePriceLast },
                },
                Result = false
            };

            //# Decision
            //# pozisyon long olarak girilmis, eger fiyat MA ustunde ise pozisyonu koru
            if (closePriceLast > maLast)
                return checkResult;

            checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldCloseShort()
        {
           //# Technical Analysis
            var maResult = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMaLength)
                : TA.SMA.Calculate(closeArray, pMaLength);

             //# Variables
            var maLast = maResult.Last();
            var closePriceLast = closeArray.Last();

            var checkResult = new CheckResult()
            {
                 PositionLogs = new Dictionary<string, object>() {
                   { $"MatLast[{pMaLength}]",maLast },
                    { "ClosePriceLast",closePriceLast },
                },
                Result = false
            };

            //# Decision
            //# pozisyon short olarak girilmis, eger fiyat MA altında ise pozisyonu koru
            if (closePriceLast < maLast)
                return checkResult;

            checkResult.Result = true;

            return checkResult;
        }

         public override TrailingStopResult UpdateTrailingStop(decimal entryPrice, int side, decimal quantity)
        {
            return new TrailingStopResult(false, 0);
        }

        public override bool ShouldStop(decimal entryPrice, int side)
        {
            return false;
        }

        public override Dictionary<string, string> GetCalculations()
        { 
            throw new NotImplementedException();
        }
    }
}
