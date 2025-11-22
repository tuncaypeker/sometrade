using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using SomeTrade.TA.Indicators.KivancOzbilgic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    /// <summary>
    /// AL: BUY sinyalinin gelmesi ile satin alir
    ///     STOP: SELL sinyali ile satar
    /// SAT: SELL sinyalinin gelmesi ile short girer
    ///     STOP: BUY sinyali ile pozisyonu kapatir
    /// </summary>
    public class AlphaTrendStrategy : StrategyBase
    {
        //parameters
        private int pAlphaTrendLength;
        private double pAlphaTrendCoeff;
        private bool pAlphaTrendVolumeData;

        public AlphaTrendStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "AlphaTrendStrategy", 5)
        {
            pAlphaTrendLength = GetParameterAsInt("AlphaTrendLength");
            pAlphaTrendCoeff = GetParameterAsDouble("AlphaTrendCoeff");
            pAlphaTrendVolumeData = GetParameterAsBoolean("AlphaTrendVolumeData");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var alphaTrendResult = ALPHATREND.Execute(closeArray, highArray, lowArray, volumeArray
                , pAlphaTrendLength, pAlphaTrendCoeff, pAlphaTrendVolumeData);

            //# Variables
            var alphaTrendBuy = alphaTrendResult.BuySignal.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "alphaTrendBuy", alphaTrendBuy }
                },
                Result = false
            };

            //# Decision
            if (alphaTrendBuy == 0)
                return checkResult;

            checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
            //# Technical Analysis
            var alphaTrendResult = ALPHATREND.Execute(closeArray, highArray, lowArray, volumeArray, pAlphaTrendLength, pAlphaTrendCoeff, pAlphaTrendVolumeData);

            //# Variables
            var alphaTrendSell = alphaTrendResult.SellSignal.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "alphaTrendSell", alphaTrendSell }
                },
                Result = false
            };

            //# Decision
            if (alphaTrendSell == 0)
                return checkResult;

            checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldCloseLong()
        {
            //# Technical Analysis
            var alphaTrendResult = ALPHATREND.Execute(closeArray, highArray, lowArray, volumeArray, pAlphaTrendLength, pAlphaTrendCoeff, pAlphaTrendVolumeData);

            //# Variables
            var alphaTrendSell = alphaTrendResult.SellSignal.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "alphaTrendSell", alphaTrendSell }
                },
                Result = false
            };

            //# Decision
            //# pozisyon long olarak girilmis, eger sell sinyali gelmedi ise pozisyonu koru
            if (alphaTrendSell == 0)
                return checkResult;

            checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldCloseShort()
        {
           //# Technical Analysis
            var alphaTrendResult = ALPHATREND.Execute(closeArray, highArray, lowArray, volumeArray, pAlphaTrendLength, pAlphaTrendCoeff, pAlphaTrendVolumeData);

            //# Variables
            var alphaTrendBuy = alphaTrendResult.BuySignal.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "alphaTrendBuy", alphaTrendBuy }
                },
                Result = false
            };

            //# Decision
            //# pozisyon short olarak girilmis, eger buy sinyali gelmedi ise pozisyonu koru
            if (alphaTrendBuy == 0)
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
