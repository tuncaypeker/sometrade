using SomeTrade.TA.Dto;
using System.Collections.Generic;

namespace SomeTrade.TA.Harmonics
{
	public class Initial
	{
		//TODO: 1- Sadece fraktallari dikkate aliyoruz, bur bir sorun mu, ozellikle d noktası icin fraktal olmasi sart mi
		//		Kesinlikle degil, burada buyuk bir hatamiz var, mum kapansinin bekledigimi D noktasinda olmasi bizim icin yeterli
		//		Bunu da C noktasi oturduktan sonraki mum kapanislarina bakarak yapcaz
		//		Ilk etapta 10 mum baksak bizim icin yeterli olur, sonrasinda atiyorum AB arasindaki mum kadar bakariz falan
		//TODO: 2- Sadece sonraki swingleri dikkate aliyoruz burada kac swing bakılabilir belki sonraki swing, ya da sonraki 2 swing bakilabilir
		public static List<PBSwingPoint> GetFraktals(List<CandleDto> candles)
		{
			//once tum fraktallari bulalim tepe? dip?
			var swing = new Swing();
			var fraktals = new List<PBSwingPoint>();
			for (int i = 2; i < candles.Count - 3; i++)
			{
				var currentCandle = candles[i];
				var prev1 = candles[i - 1];
				var prev2 = candles[i - 2];
				var next1 = candles[i + 1];
				var next2 = candles[i + 2];

				var isThisAFraktal = swing.IsThisCandleAFraktal(currentCandle, prev1, prev2, next1, next2);
				if (isThisAFraktal.IsFractal)
					fraktals.Add(new PBSwingPoint(currentCandle.OpenTime, isThisAFraktal.Price, -1, isThisAFraktal.Trend));
			}

			return fraktals;
		}
	}
}
