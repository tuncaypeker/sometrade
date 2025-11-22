using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;

namespace SomeTrade.Strategies
{
	public interface IStrategyResolver
	{
		IStrategy GetStrategyByName(string name, List<Dto.Candle> candles, Dictionary<string, object> parameters);
		IStrategy GetStrategyById(int id, List<Dto.Candle> candles, Dictionary<string, object> parameters);
	}

	public class StrategyResolver : IStrategyResolver
	{
		public IStrategy GetStrategyByName(string name, List<Dto.Candle> candles, Dictionary<string, object> parameters)
		{
			switch (name)
			{
				case "AlphaTrendStrategy":
					return new AlphaTrendStrategy(candles, parameters);
				case "MACrossMAStrategy":
					return new MACrossMAStrategy(candles, parameters);
				case "MACrossPriceStrategy":
					return new MACrossPriceStrategy(candles, parameters);
				case "VolumeJumpStrategy":
					return new VolumeJumpStrategy(candles, parameters);
				case "MARibbonStrategy":
					return new MARibbonStrategy(candles, parameters);
				case "TrendFollowerStrategy":
					return new TrendFollowerStrategy(candles, parameters);
				case "OTTStochSlowStrategy":
					return new OTTStochSlowStrategy(candles, parameters);
				case "OTEEntryStrategy":
					return new OTEEntryStrategy(candles, parameters);
				default: return null;
			}
		}

		public IStrategy GetStrategyById(int id, List<Dto.Candle> candles, Dictionary<string, object> parameters)
		{
			switch (id)
			{
				case 5: return new AlphaTrendStrategy(candles, parameters);
				case 6: return new MACrossMAStrategy(candles, parameters);
				case 7: return new MACrossPriceStrategy(candles, parameters);
				case 8: return new VolumeJumpStrategy(candles, parameters);
				case 2: return new MARibbonStrategy(candles, parameters);
				case 10: return new TrendFollowerStrategy(candles, parameters);
				case 11: return new OTTStochSlowStrategy(candles, parameters);
				case 3: return new OTEEntryStrategy(candles, parameters);
				default: return null;
			}
		}
	}
}
