using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    public class PriceJumpStrategy : StrategyBase
    {
        //parameters
        private int pJumpPercentage;

        public PriceJumpStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "PriceJumpStrategy", 8)
        {
            pJumpPercentage = GetParameterAsInt("JumpPercentage");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var isJumped = _isPriceJumped(closeArray.Last(), closeArray.TakePrev(), pJumpPercentage);

            //# Variables
            var priceLast = closeArray.Last();
            var pricePrev = closeArray.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"PriceLast",priceLast },
                   { $"PricePrev",pricePrev },
                },
                Result = false
            };

            //# Decision
            if (isJumped)
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
            return new CheckResult()
            {
                Result = false
            };
        }

        /// <summary>
        /// Terse kirmizi mum atarsa ve belirtilen degerin yarisi kadar sicradi ise
        /// </summary>
        /// <returns></returns>
        public override CheckResult ShouldCloseLong()
        {
            return new CheckResult()
            {
                Result = false
            };
        }

        public override CheckResult ShouldCloseShort()
        {
            return new CheckResult()
            {
                Result = false
            };
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

        private bool _isPriceJumped(double _closeLast, double _closePrev, int jumpPercentage)
        {
            if (_closeLast < _closePrev)
            {
                var _swipeLast = _closeLast;
                _closeLast = _closePrev;
                _closePrev = _swipeLast;
            }

            var _diff = _closeLast - _closePrev;
            var percentage = 100 * _diff / _closeLast;

            return percentage > jumpPercentage;
        }
    }
}
