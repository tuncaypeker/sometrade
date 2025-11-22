using SomeTrade.TA.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.TA
{
	public class Swing
	{
		readonly double _validSwingPoint;
		readonly bool _useWicksAsBOS;

		public Swing(double validSwingPoint = 0.500, bool useWicksAsBOS = false)
		{
			_validSwingPoint = validSwingPoint;
			_useWicksAsBOS = useWicksAsBOS;
		}

		public SwingResultDto CalculateSwings(List<CandleDto> candles)
		{
			var candleCount = candles.Count;
			var result = new SwingResultDto()
			{
				SwingHistory = new List<SwingHistory>(),
				LastCandleOpenTime = candles.Last().OpenTime,
				LastCandleClosePrice = candles.Last().ClosePrice,
				SwingHigh = new SwingPoint(DateTime.MinValue, 0m),
				SwingLow = new SwingPoint(DateTime.MinValue, 0m)
			};

			bool hasValidPullback = false;

			//son 2 muma farkli davran, fraktal bakıyoruz
			for (int i = 0; i < candles.Count - 1; i++)
			{
				Console.Clear();

				var currentCandle = candles[i];
				
				//ilk mum geldi su an icin elimizdeki tek set 
				if (i == 0 || i == 1)
				{
					if (currentCandle.HighPrice > result.SwingHigh.Price) result.SwingHigh = new SwingPoint(currentCandle.OpenTime, currentCandle.HighPrice);

					if (result.SwingLow.Price == 0) result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);
					if (currentCandle.LowPrice < result.SwingLow.Price) result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);

					continue;
				}

				//son iki mum'a farkli davranalim cunku fraktal olup olmadigni anlayamayiz
				//bu gelen iki son mum bizim için ne ifade eder
				//swing high veya low'un altına üstüne giderek trendi değiştirebilir, yeni dip tepe yapabilir
				if (i == candleCount - 2 || i == candleCount - 3)
					continue;

				//bundan sonra gelen mumlara tek tek bakcaz, farkli senaryolar soz konusu

				//1 bu yeni gelen mum yeni bir yuksek yapmıs olabilir, wick or close bana farketmez icerde bakcam
				if (currentCandle.HighPrice > result.SwingHigh.Price || currentCandle.ClosePrice > result.SwingHigh.Price)
				{
					//a. hicbir trend yoktur ya da bilmiyoruzdur ve bunun anlami bizim icin yukari yonlu bir hareket var anlamina gelir
					if (result.CurrentTrend == 0)
					{
						result.CurrentTrend = 1;
						result.SwingHigh = new SwingPoint(currentCandle.OpenTime, currentCandle.HighPrice);

						continue;
					}

					//b. trendimiz short'tu, artik long'a dondu, pullback vermeden direk kirdi
					//MSB oldugu icin wick ya da kapanis farketmez, ustunde kapattigi an MSB kabul ederiz
					if (result.CurrentTrend == -1)
					{
						result.CurrentTrend = 1;

						//burada trend degisi ve bu durumu swing history'e yazmamiz lazim
						result.SwingHistory.Add(new SwingHistory(result.SwingLow, "Swing Low"));
						result.SwingHigh = new SwingPoint(currentCandle.OpenTime, currentCandle.HighPrice);

						hasValidPullback = false;
						result.PullBack = null;

						continue;
					}

					//burada pullback henuz gelmemis ve trend yonunde hareket devam ediyor olabilir, bu durumda 
					//wick kontrol etmeden yeni yuksek olarak isaretlemem gerekiyor
					if (!hasValidPullback && currentCandle.HighPrice > result.SwingHigh.Price) {
						result.SwingHigh = new SwingPoint(currentCandle.OpenTime, currentCandle.HighPrice);

						continue;
					}


					//bu noktada artik zaten yukselen trend icerisindeyiz demektir
					//WickBos olarak kabul edilecek mi edilmeyecek mi bu duruma uygun sekilde duzenleyelim
					var currentCandleHighPrice = _useWicksAsBOS ? currentCandle.HighPrice : currentCandle.ClosePrice;

					//c. zaten trendimiz yukselendir ve bu fiyat ile birlikte yeni bir yukarı yonlu kırılım gelmiş olabilir
					if (currentCandleHighPrice > result.SwingHigh.Price)
					{
						var prevSwingHigh = new SwingPoint(result.SwingHigh.Date, result.SwingHigh.Price);
						result.SwingHigh = new SwingPoint(currentCandle.OpenTime, currentCandle.HighPrice);

						//bu durumda pullback varsa duzenlemeleri yapalim
						if (hasValidPullback)
						{
							//swing low'dan swing high'a gittik sonra pull back'e geldik ve yeni swing high oldu
							result.SwingHistory.Add(new SwingHistory(result.SwingLow, "Swing Low"));
							result.SwingHistory.Add(new SwingHistory(prevSwingHigh, "Swing High"));
							result.SwingHistory.Add(new SwingHistory(result.PullBack, "PullBack"));

							//yeni yuksek oldugu icin valid pullback'im henuz yok
							hasValidPullback = false;

							//yeni swinglow'um onceki pullback'im
							result.SwingLow = new SwingPoint(result.PullBack.Date, result.PullBack.Price);
							result.PullBack = null;
						}
					}

					continue;
				}

				//2 bu yeni gelen mum yeni bir dip yapmis olabilir, wick or close bana farketmez icerde bakcam
				if (currentCandle.LowPrice < result.SwingLow.Price || currentCandle.ClosePrice < result.SwingLow.Price)
				{
					//a. hicbir trend yoktur ya da bilmiyoruzdur ve bunun anlami bizim icin asagi yonlu bir hareket var anlamina gelir
					if (result.CurrentTrend == 0)
					{
						result.CurrentTrend = -1;
						result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);

						continue;
					}

					//b. trendimiz long'tu, artik short'a dondu, pullback vermeden direk kirdi
					//MSB oldugu icin wick ya da kapanis farketmez, altinda kapattigi an MSB kabul ederiz
					if (result.CurrentTrend == 1)
					{
						result.CurrentTrend = -1;

						//burada trend degisi ve bu durumu swing history'e yazmamiz lazim
						result.SwingHistory.Add(new SwingHistory(result.SwingHigh, "Swing High"));
						result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);

						hasValidPullback = false;
						result.PullBack = null;

						continue;
					}

					//burada pullback henuz gelmemis ve trend yonunde hareket devam ediyor olabilir, bu durumda 
					//wick kontrol etmeden yeni dusuk olarak isaretlemem gerekiyor
					if (!hasValidPullback && currentCandle.LowPrice < result.SwingLow.Price) {
						result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);

						continue;
					}

					//bu noktada artik zaten dusen trend icerisindeyiz demektir
					//WickBos olarak kabul edilecek mi edilmeyecek mi bu duruma uygun sekilde duzenleyelim
					var currentCandleLowPrice = _useWicksAsBOS ? currentCandle.LowPrice : currentCandle.ClosePrice;

					//c. zaten trendimiz yukselendir ve bu fiyat ile birlikte yeni bir asagi yonlu kırılım gelmiş olabilir
					if (currentCandleLowPrice < result.SwingLow.Price)
					{
						var prevSwingLow = new SwingPoint(result.SwingLow.Date, result.SwingLow.Price);
						result.SwingLow = new SwingPoint(currentCandle.OpenTime, currentCandle.LowPrice);

						//bu durumda pullback varsa duzenlemeleri yapalim
						if (hasValidPullback)
						{
							//swing high'dan swing low'a gittik sonra pull back'e geldik ve yeni swing low oldu
							result.SwingHistory.Add(new SwingHistory(result.SwingHigh, "Swing High"));
							result.SwingHistory.Add(new SwingHistory(prevSwingLow, "Swing Low"));
							result.SwingHistory.Add(new SwingHistory(result.PullBack, "PullBack"));

							//yeni dusuk oldugu icin valid pullback'im henuz yok
							hasValidPullback = false;

							//yeni swinghigh'ım onceki pullback'im
							result.SwingHigh = new SwingPoint(result.PullBack.Date, result.PullBack.Price);
							result.PullBack = null;
						}
					}

					continue;
				}

				//3 bu mum tepe ve dip icerisinde bir mumdur ve muhtemel bir donus noktasi mi degil mi ona bakariz
				if (currentCandle.LowPrice > result.SwingLow.Price && currentCandle.HighPrice < result.SwingHigh.Price)
				{
					//trend belli degilse yapacak biseyimiz yok
					if (result.CurrentTrend == 0)
						continue;

					var prev1 = candles[i - 1];
					var prev2 = candles[i - 2];
					var next1 = candles[i + 1];
					var next2 = candles[i + 2];

					if (result.CurrentTrend == 1)
					{
						//1- bu mum bir Dip fraktali mi
						var isFraktal = IsThisCandleAFraktal(currentCandle, prev1, prev2, next1, next2);
						if (!isFraktal.IsFractal || isFraktal.Trend != -1)
							continue;

						//2- bu bizim icin valid bir donus noktasi mi
						var fibo = CalculateFib(result.SwingLow.Price, result.SwingHigh.Price, currentCandle.LowPrice);
						if (fibo > _validSwingPoint)
						{
							//ztn pullbcak point varsa ve ben bunun daha ustunde bir pullback buldysam bunu ignore etcem
							if (hasValidPullback && currentCandle.LowPrice > result.PullBack.Price)
							{
								//($"\tFibo ile valid bir pull back ama ignore: {fibo} / {result.PullBack.Price}", writeConsole);
							}
							else
							{
								hasValidPullback = true;
								result.PullBack = new PBSwingPoint(currentCandle.OpenTime, currentCandle.LowPrice, fibo, isFraktal.Trend);

								//($"\tFibo ile valid bir pull back: {fibo} / {result.PullBack.Price}", writeConsole);
							}

							continue;
						}
						else
						{
							//($"\tFibo ile valid olmayan bir pull back: {fibo}", writeConsole);
						}
					}

					if (result.CurrentTrend == -1)
					{
						//1- bu mum bir tepe fraktali mi
						var isFraktal = IsThisCandleAFraktal(currentCandle, prev1, prev2, next1, next2);
						if (!isFraktal.IsFractal || isFraktal.Trend != 1)
							continue;

						//2- bu bizim icin valid bir donus noktasi mi
						var fibo = CalculateFib(result.SwingHigh.Price, result.SwingLow.Price, currentCandle.HighPrice);

						if (fibo > _validSwingPoint)
						{
							//ztn pullbcak point varsa ve ben bunun daha altinda bir pullback buldysam bunu ignore etcem
							if (hasValidPullback && currentCandle.HighPrice < result.PullBack.Price)
							{
								//($"\tFibo ile valid bir pull back ama ignore: {fibo} / {result.PullBack.Price}", writeConsole);
							}
							else
							{
								hasValidPullback = true;
								result.PullBack = new PBSwingPoint(currentCandle.OpenTime, currentCandle.HighPrice, fibo, isFraktal.Trend);

								//($"\tFibo ile valid bir pull back: {fibo} / {result.PullBack.Price}", writeConsole);
							}

							continue;
						}
						else
						{
							//($"\tFibo ile valid olmayan bir pull back: {fibo}", writeConsole);
						}
					}
				}
			}

			//son olarak en son gelen swing low ya da high'i eklemek lazim
			if (result.CurrentTrend == 1)
				result.SwingHistory.Add(new SwingHistory(result.SwingHigh, "Swing High"));
			else if (result.CurrentTrend == -1)
				result.SwingHistory.Add(new SwingHistory(result.SwingLow, "Swing Low"));

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="candle"></param>
		/// <param name="prevCandle1"></param>
		/// <param name="prevCandle2"></param>
		/// <param name="nextCandle1"></param>
		/// <param name="nextCandle2"></param>
		/// <param name="trend">
		///  1: Yukselen trend ise, dip fraktali bakar
		/// -1: Dusen trend ise, tepe fraktali bakar
		/// 
		/// EQ high ve lowlari ignore eder,
		/// </param>
		/// <returns></returns>
		public FractalResultDto IsThisCandleAFraktal(CandleDto current, CandleDto prev1, CandleDto prev2, CandleDto next1, CandleDto next2)
		{
			//acaba bu bir dip fraktali mi yani en dusuk seviye, onceki ve sonraki dusuk seviyelerin altinda mi
			if (current.LowPrice < prev1.LowPrice && current.LowPrice < prev2.LowPrice &&
				current.LowPrice < next1.LowPrice && current.LowPrice < next2.LowPrice)
			{
				return new FractalResultDto()
				{
					IsFractal = true,
					Price = Convert.ToDecimal(current.LowPrice),
					Trend = -1
				};
			}

			//acaba bu bir tepe fraktali mi yani en yuksek seviye, onceki ve sonraki yuksek seviyelerin ustunde mi
			if (current.HighPrice > prev1.HighPrice && current.HighPrice > prev2.HighPrice &&
				current.HighPrice > next1.HighPrice && current.HighPrice > next2.HighPrice)
			{
				return new FractalResultDto()
				{
					IsFractal = true,
					Price = Convert.ToDecimal(current.HighPrice),
					Trend = 1
				};
			}

			return new Dto.FractalResultDto() { IsFractal = false };
		}

		public double CalculateFib(double X, double A, double price)
		{
			if (X == A)
				return 0;

			double start = X > A ? A : X;
			double end = X > A ? X : A;

			double diffBetweenPriceWithStart = price - start;
			double diffBetweenEndWithStart = end - start;

			var fib = System.Math.Truncate((diffBetweenPriceWithStart / diffBetweenEndWithStart) * 1000) / 1000;

			return X > A ? fib : 1 - fib;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="X"></param>
		/// <param name="A"></param>
		/// <param name="fib">Bu deger 0.786, 0.382 gibi gonderilmelidir</param>
		/// <returns></returns>
		public double CalculateFibValue(double X, double A, double fib)
		{
			if (X == A)
				return 0;

			double start = X > A ? A : X;
			double end = X > A ? X : A;

			double diffBetweenEndWithStart = end - start;
			fib = X > A ? fib : 1 - fib;

			return diffBetweenEndWithStart * fib;
		}

		public bool IsThisCandleOutOfBorders(double price1, double price2, CandleDto candle)
		{
			var up  = price1 >= price2 ? price1 : price2;
			var down = price1 < price2 ? price1 : price2;

			return candle.HighPrice > up || candle.LowPrice < down;
		}
	}
}
