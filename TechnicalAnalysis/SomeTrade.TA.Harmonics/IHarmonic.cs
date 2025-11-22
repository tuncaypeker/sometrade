using SomeTrade.TA.Dto;
using System.Collections.Generic;

namespace SomeTrade.TA.Harmonics
{
	public interface IHarmonic
	{
		HarmonicResult Pick(List<CandleDto> candles);
		string GetName();
	}
}