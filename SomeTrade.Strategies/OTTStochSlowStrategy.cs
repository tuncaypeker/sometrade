using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using SomeTrade.TA;
using SomeTrade.TA.Indicators.KivancOzbilgic;
using SomeTrade.TA.Indicators.KivancOzbilgic.OTT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    /// <summary>
    /// AL: OTT Support Line üstünde
    ///     Stochastic Slow K Line D Line üstünde
    ///     STOP: SELL sinyali ile satar
    /// SAT: OTT Support Line altında
    ///     Stochastic Slow K Line D Line altında
    ///     STOP: BUY sinyali ile pozisyonu kapatir
    /// </summary>
    public class OTTStochSlowStrategy : StrategyBase
    {
        //parameters
        private int pOTTLength;
        private double pOTTOptimization;
        private int pStochasticSlowSmoothK;
        private int pStochasticSlowPeriod;

        public OTTStochSlowStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "OTTStochSlowStrategy", 11)
        {
            pOTTLength = GetParameterAsInt("OTTLength");
            pOTTOptimization = GetParameterAsDouble("OTTOptimization");
            pStochasticSlowSmoothK = GetParameterAsInt("StochasticSlowSmoothK");
            pStochasticSlowPeriod = GetParameterAsInt("StochasticSlowPeriod");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var OTTResult = OTT.Calculate(closeArray,pOTTLength, pOTTOptimization, "VAR");
            var StochSlowResult = StochasticSlow.Calculate(closeArray, highArray, lowArray, pStochasticSlowPeriod, pStochasticSlowSmoothK, pStochasticSlowSmoothK);

            //# Variables
            var ottSupportLine = OTTResult.SupportLine.Last();
            var ottLine = OTTResult.OTTLine.Last();
            var stochSlowKLine = StochSlowResult.K.Last();
            var stochSlowDLine = StochSlowResult.D.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "ottSupportLine", ottSupportLine },
                    { "ottLine", ottLine },
                    { "stochSlowKLine", stochSlowKLine },
                    { "stochSlowDLine", stochSlowKLine },
                },
                Result = false
            };

            //# Decision
            checkResult.Result = ottSupportLine > ottLine && stochSlowKLine > stochSlowDLine;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
            //# Technical Analysis
            var OTTResult = OTT.Calculate(closeArray,pOTTLength, pOTTOptimization, "VAR");
            var StochSlowResult = StochasticSlow.Calculate(closeArray, highArray, lowArray, pStochasticSlowPeriod, pStochasticSlowSmoothK, pStochasticSlowSmoothK);

            //# Variables
            var ottSupportLine = OTTResult.SupportLine.Last();
            var ottLine = OTTResult.OTTLine.Last();
            var stochSlowKLine = StochSlowResult.K.Last();
            var stochSlowDLine = StochSlowResult.D.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                    { "ottSupportLine", ottSupportLine },
                    { "ottLine", ottLine },
                    { "stochSlowKLine", stochSlowKLine },
                    { "stochSlowDLine", stochSlowKLine },
                },
                Result = false
            };

            //# Decision
            checkResult.Result = ottSupportLine < ottLine && stochSlowKLine < stochSlowDLine;

            return checkResult;
        }

        public override CheckResult ShouldCloseLong()
        {
            //eger short sinyali geldi ise long'u kapatabilirsin
            return ShouldOpenShort();
        }

        public override CheckResult ShouldCloseShort()
        {
            //eger long sinyali geldi ise long'u kapatabilirsin
            return ShouldOpenShort();
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
