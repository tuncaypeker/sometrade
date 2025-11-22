using SomeTrade.TA.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SomeTrade.TA.Harmonics
{
	public class HarmonicGartley : IHarmonic
	{
		/// <summary>
		/*
		   AB dönüşü
				Gartley: B noktası XA hareketinin 0.618
						 B noktası XA hareketinin 0.618ine mutlaka degmeli, 786yı mutlaka gecmemelidir[Kaynak 4]

			C noktası
				Gartley: C noktası AB hareketinin 0.382 - 0.886 arasında olmalıdır
						 C noktası AB hareketinin 0.618ine fitil dahil olsa degmek zorundadır. Perfect Gartley[Kaynak 4]
	
			D noktası
				Gartley: D noktası XA hareketinin 0.786
						 D noktası AB hareketinin 1.272 - 1.618 arasında olmalıdır
		*/
		/// </summary>
		/// <param name="fraktals"></param>
		public HarmonicResult Pick(List<CandleDto> candles)
		{
			var fraks = Initial.GetFraktals(candles);
			var harmonicResult = new HarmonicResult();
			var swing = new TA.Swing();
			var resetXLoop = false;

			//tum fraktallar da gezin
			for (int forX = 0; forX < fraks.Count - 1; forX++)
			{
				//yeni eklenirse bu true oluyor ve A B C D dongulerinden cikiyor
				resetXLoop = false;

				//X'dan bir sonraki fraktaldan itibaren 3 fraktal bak
				for (int forA = forX + 1; forA < forX + 4 && forA < fraks.Count; forA++)
				{
					var trend = fraks[forX].Price > fraks[forA].Price ? -1 : 1;

					//A ve X fraktallari her zaman ters olmalidir
					if (fraks[forA].Trend == fraks[forX].Trend) continue;

					//X ile A arasında XA sınırını asan bir mum var midir
					var indexOfX = candles.FindIndex(x => x.OpenTime == fraks[forX].Date);
					var indexOfA = candles.FindIndex(x => x.OpenTime == fraks[forA].Date);
					bool outOfBorderXA = false;
					for (int candleIndex = indexOfX; candleIndex < indexOfA; candleIndex++)
					{
						bool isOutOfBorder = swing.IsThisCandleOutOfBorders(fraks[forX].Price, fraks[forA].Price, candles[candleIndex]);
						if (isOutOfBorder)
						{
							outOfBorderXA = true;
							break;
						}
					}
					if (outOfBorderXA) continue;

					//A'den bir sonraki fraktaldan itibaren 3 fraktal bak
					for (int forB = forA + 1; forB < forA + 4 && forB < fraks.Count; forB++)
					{
						//A ve B fraktallari her zaman ters olmalidir
						if (fraks[forB].Trend == fraks[forA].Trend) continue;

						//bu B valid bir B noktasi mi nasil anlarsin
						var fiboXAForB = swing.CalculateFib(fraks[forX].Price, fraks[forA].Price, fraks[forB].Price);
						if (fiboXAForB < 0.618 || fiboXAForB > 0.780)
							continue;

						//B ile C arasında XA sınırını asan bir mum var midir
						var indexOfB = candles.FindIndex(x => x.OpenTime == fraks[forB].Date);
						bool outOfBorderAB = false;
						for (int candleIndex = indexOfA; candleIndex < indexOfB; candleIndex++)
						{
							bool isOutOfBorder = swing.IsThisCandleOutOfBorders(fraks[forX].Price, fraks[forA].Price, candles[candleIndex]);
							if (isOutOfBorder)
							{
								outOfBorderAB = true;
								break;
							}
						}
						if (outOfBorderAB) continue;

						//B'den bir sonraki fraktaldan itibaren 3 fraktal bak
						for (int forC = forB + 1; forC < forB + 4 && forC < fraks.Count; forC++)
						{
							//C nin fraktal turu B'nin tersi yonunde olmalidir
							if (fraks[forC].Trend == fraks[forB].Trend)
								continue;

							//bu C valid bir C noktasi mi nasil anlarsin
							var fiboABForC = swing.CalculateFib(fraks[forA].Price, fraks[forB].Price, fraks[forC].Price);
							if ((fiboABForC < 0.618 || fiboABForC > 0.886))
								continue;

							//B ile C arasında XA sınırını asan bir mum var midir
							var indexOfC = candles.FindIndex(x => x.OpenTime == fraks[forC].Date);
							bool outOfBorderBC = false;
							for (int candleIndex = indexOfB; candleIndex < indexOfC; candleIndex++)
							{
								bool isOutOfBorder = swing.IsThisCandleOutOfBorders(fraks[forX].Price, fraks[forA].Price, candles[candleIndex]);
								if (isOutOfBorder)
								{
									outOfBorderBC = true;
									break;
								}
							}
							if (outOfBorderBC) continue;

							//bu noktada artik valid bir Cpoint var, sonraki 10 mum icinde kapanısına gore uygun D noktasi bakiyorum
							for (int j = indexOfC + 1; j < indexOfC + 10 && j < candles.Count - 1; j++)
							{
								var candleForDCheck = candles[j];
								//acaba bu mum ile trende gore X ya da A kirildi mi, bu durumda iptal olur
								//eger trend dusen ise low price Anin altına inemez, high price Xin ustune cikamaz
								//eger trend artan ise high price Anin ustune cikamaz, low price Xin altına inemez
								if ((trend == -1 && (candleForDCheck.LowPrice < fraks[forA].Price || candleForDCheck.HighPrice > fraks[forX].Price)) ||
									(trend == 1 && (candleForDCheck.HighPrice > fraks[forA].Price || candleForDCheck.LowPrice < fraks[forX].Price)))
									break;

								//TODO: burada kapanis fiyati dikkate aldim, fitilli alttan ustten gittigi yeri onemsemedim
								var DPotential = new SwingPoint(candleForDCheck.OpenTime, candleForDCheck.ClosePrice);

								var fiboXAForD = swing.CalculateFib(fraks[forX].Price, fraks[forA].Price, DPotential.Price);
								if (fiboXAForD > 0.786 && fiboXAForD < 0.850)//burada 850un altinda kalsin diye bir sart yok ama cok uzaklasmasin
								{
									var harmonicItem = new HarmonicItem();

									harmonicItem.A = fraks[forA];
									harmonicItem.X = fraks[forX];
									harmonicItem.B = new PBSwingPoint(fraks[forB].Date, fraks[forB].Price, fiboXAForB, fraks[forB].Trend);
									harmonicItem.C = new PBSwingPoint(fraks[forC].Date, fraks[forC].Price, fiboABForC, fraks[forC].Trend);
									harmonicItem.D = new PBSwingPoint(DPotential.Date, DPotential.Price, fiboXAForD, 0); //
									harmonicItem.Trend = trend;

									harmonicResult.Detections.Add(harmonicItem);
									resetXLoop = true; ;

									break;
								}
							}

							if (resetXLoop) break;
						}

						if (resetXLoop) break;
					}

					if (resetXLoop) break;
				}
			}

			return harmonicResult;
		}

		public string GetName()
		{
			return "Gartley";
		}
	}
}
