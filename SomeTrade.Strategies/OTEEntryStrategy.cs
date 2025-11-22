using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using SomeTrade.TA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
	public class OTEEntryStrategy : StrategyBase
	{
		//parameters
		private int pJumpPercentage;

		public OTEEntryStrategy(List<Candle> candles, Dictionary<string, object> parameters)
			: base(candles, parameters, "OTEEntryStrategy", 3)
		{
			pJumpPercentage = GetParameterAsInt("OTEEntryStrategy");
		}

		public override CheckResult ShouldOpenLong()
		{
			var swing = new Swing();
			var checkResult = new CheckResult(false);
			var swingResult = swing.CalculateSwings(_candles.Select(x => x.ToCandleDto()).ToList());

			if (swingResult.PullBack == null)
				return checkResult;

			if (swingResult.CurrentTrend == -1)
				return checkResult;

			//1- Swing bolgeleri cizdir
			//2- Son fiyatin fibo degerine bakalim
			//3- Fiyatin extreme noktadan donmesi muhtemeldir diye dusunerek eger 79 ustunde ise islem almaliyiz
			//son fiyatin fibosuna bakalim
			var fibo = swing.CalculateFib(swingResult.SwingLow.Price, swingResult.SwingHigh.Price, closeArray.Last());

			//aslinda sikinti sı OTE'yi de gecmis fiyat, OTE dedik ama extreme entry gibi oldu
			checkResult.Result = fibo > 0.78;
			checkResult.PositionLogs.Add("lastPriceFibo", fibo);
			checkResult.PositionLogs.Add("swingLow", swingResult.SwingLow.Price);
			checkResult.PositionLogs.Add("swingHigh", swingResult.SwingHigh.Price);

			return checkResult;
		}

		public override CheckResult ShouldOpenShort()
		{
			var swing = new Swing();
			var checkResult = new CheckResult(false);
			var swingResult = swing.CalculateSwings(_candles.Select(x => x.ToCandleDto()).ToList());

			if (swingResult.PullBack == null)
				return checkResult;

			if (swingResult.CurrentTrend == 1)
				return checkResult;

			//1- Swing bolgeleri cizdir
			//2- Son fiyatin fibo degerine bakalim
			//3- Fiyatin extreme noktadan donmesi muhtemeldir diye dusunerek eger 79 ustunde ise islem almaliyiz
			//son fiyatin fibosuna bakalim
			var fibo = swing.CalculateFib(swingResult.SwingLow.Price, swingResult.SwingHigh.Price, closeArray.Last());

			//aslinda sikinti sı OTE'yi de gecmis fiyat, OTE dedik ama extreme entry gibi oldu
			checkResult.Result = fibo > 0.78;
			checkResult.PositionLogs.Add("lastPriceFibo", fibo);
			checkResult.PositionLogs.Add("swingLow", swingResult.SwingLow.Price);
			checkResult.PositionLogs.Add("swingHigh", swingResult.SwingHigh.Price);

			return checkResult;
		}

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
			return new Dictionary<string, string>();
		}
	}
}
