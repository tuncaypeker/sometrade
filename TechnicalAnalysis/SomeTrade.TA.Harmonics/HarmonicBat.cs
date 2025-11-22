using SomeTrade.TA.Dto;
using System;
using System.Collections.Generic;

namespace SomeTrade.TA.Harmonics
{
	public class HarmonicBat : IHarmonic
	{
		

		/// <summary>
		/*
		   AB dönüşü
				Bat: B noktası XA hareketinin 0.382 - 0.500 arasında olmalıdır

			C noktası
				Gartley: C noktası AB hareketinin 0.382 - 0.886 arasında olmalıdır
	
			D noktası
				Gartley: D noktası XA hareketinin 0.786
						 D noktası AB hareketinin 1.127 - 1.618 arasında olmalıdır
		*/
		/// </summary>
		/// <param name="candles"></param>
		/// <param name="fraktals"></param>
		public HarmonicResult Pick(List<CandleDto> candles)
		{
			var fraktals = Initial.GetFraktals(candles);
			var harmonicResult = new HarmonicResult();
			var swing = new TA.Swing();

			//simdi bu noktada elimizde fraktallar var, bu fraktallar arasinda gezinip kurallari ariycaz
			for (int i = 0; i < fraktals.Count - 4; i++)
			{
				var X = fraktals[i];
				var A = fraktals[i + 1];
				var BPotential = fraktals[i + 2];
				var CPotential = fraktals[i + 3];
				var trend = X.Price > A.Price ? -1 : 1;

				//eger trendim  1 ise B'nin dip  fraktali, C'nin tepe fraktali olmasi 
				//eger trendim -1 ise B'nin tepe fraktali, C'nin dip  fraktali olmasi gerekiyor
				//kisacasi B ve C trendi ayni olamaz, ztn olamaz deme esit dip ve tepeleri fraktal saymiyoruz
				if (BPotential.Trend == CPotential.Trend)
					continue;

				//	Bat: B noktası XA hareketinin 0.382 - 0.500 arasında olmalıdır
				var fiboXAForB = swing.CalculateFib(X.Price, A.Price, BPotential.Price);
				if (fiboXAForB < 0.382 || fiboXAForB > 0.500)
					continue;

				//  Bat: C noktası AB hareketinin 0.382 - 0.886 arasında olmalıdır
				var fiboABForC = swing.CalculateFib(A.Price, BPotential.Price, CPotential.Price);
				if ((fiboABForC < 0.382 || fiboABForC > 0.886))
					continue;

				//Bat:
				//	D noktası XA hareketinin 0.886
				//	D noktası AB hareketinin 1.618 - 2.618 arasında olmalıdır
				//	Buranin fraktal olma sarti yok bu yuzden C sonrasindaki 10 mumun kapanisina trende yonune gore bu alanda kapanis var mi bakalim
				var indexOfC = candles.FindIndex(x => x.OpenTime == CPotential.Date);
				for (int j = indexOfC; j < indexOfC + 10 && j < candles.Count - 1; j++)
				{
					var candleForDCheck = candles[j];
					//acaba bu mum ile trende gore X ya da A kirildi mi, bu durumda iptal olur
					//eger trend dusen ise low price Anin altına inemez, high price Xin ustune cikamaz
					//eger trend artan ise high price Anin ustune cikamaz, low price Xin altına inemez
					if ((trend == -1 && (candleForDCheck.LowPrice < A.Price || candleForDCheck.HighPrice > X.Price)) ||
						(trend == 1 && (candleForDCheck.HighPrice > A.Price || candleForDCheck.LowPrice < X.Price)))
						break;

					//TODO: burada kapanis fiyati dikkate aldim, fitilli alttan ustten gittigi yeri onemsemedim
					var DPotential = new SwingPoint(candleForDCheck.OpenTime, candleForDCheck.ClosePrice);

					var fiboXAForD = swing.CalculateFib(X.Price, A.Price, DPotential.Price);
					if (fiboXAForD > 0.886 && fiboXAForD < 1)//burada 1in altinda kalsin diye bir sart yok ama cok uzaklasmasin, gerci ztn 1i gecerse break olur
					{
						var harmonicItem = new HarmonicItem();

						harmonicItem.A = A;
						harmonicItem.X = X;
						harmonicItem.B = new PBSwingPoint(BPotential.Date, BPotential.Price, fiboXAForB, BPotential.Trend);
						harmonicItem.C = new PBSwingPoint(CPotential.Date, CPotential.Price, fiboABForC, CPotential.Trend);
						harmonicItem.D = new PBSwingPoint(DPotential.Date, DPotential.Price, fiboXAForD);

						harmonicResult.Detections.Add(harmonicItem);

						break;
					}
				}
			}

			return harmonicResult;
		}

		public string GetName()
		{
			return "Bat";
		}
	}
}
