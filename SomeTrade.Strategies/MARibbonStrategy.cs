using Google.Protobuf.WellKnownTypes;
using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using SomeTrade.TA;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SomeTrade.Strategies
{
	/// <summary>
	/// AL: 3 EMA küçükten büyüge sıralanırsa AL MA50 > MA100 > MA150
	/// SAT: 3 EMA büyükten küçüğe sıralanırsa SAT MA150 > MA100 > MA50
	/// </summary>
	public class MARibbonStrategy : StrategyBase
	{
		//parameters
		private int pShortMaLength;
		private int pMiddleMaLength;
		private int pLongMaLength;
		private string pMA;

		private int pStochLength;
		private int pStochSmoothK;
		private int pStochSmoothD;

		/// <summary>
		/// Toplamda olabilecek 6 kondüsyon var Mavi: M / Kırmızı: K / Yeşil: Y
		/// M > K > Y / M > Y > K //mavi yukarida ise not ordered
		/// Y > M > K / Y > K > M //mavi en altta ise not ordered
		/// K > M > Y / K > Y > M //mavi en altta ise not ordered
		/// Kisacasi mavi mutlaka ortada olacak
		/// Bizim kosulumuz nedir 
		///     onceki barda mavi ortada olmayacak
		///     mevcut barda ortada olacak, K ve Ynin durumu long ya da shortu belirleyecek
		/// </summary>
		/// <param name="candles"></param>
		/// <param name="parameters"></param>
		public MARibbonStrategy(List<Candle> candles, Dictionary<string, object> parameters)
			: base(candles, parameters, "MARibbonStrategy", 2)
		{
			pShortMaLength = GetParameterAsInt("ShortMaLength");
			pMiddleMaLength = GetParameterAsInt("MiddleMaLength");
			pLongMaLength = GetParameterAsInt("LongMaLength");
			pMA = GetParameter("MA");

			pStochLength = GetParameterAsInt("StochLength");
			pStochSmoothK = GetParameterAsInt("StochSmoothK");
			pStochSmoothD = GetParameterAsInt("StochSmoothD");
		}

		public override CheckResult ShouldOpenLong()
		{
			//# Technical Analysis
			var maShort = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pShortMaLength)
				: TA.SMA.Calculate(closeArray, pShortMaLength);
			var maMiddle = pMA.ToLower() == "ema"
			   ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
			   : TA.SMA.Calculate(closeArray, pMiddleMaLength);
			var maLong = pMA.ToLower() == "ema"
				 ? TA.EMA.Calculate(closeArray, pLongMaLength)
				 : TA.SMA.Calculate(closeArray, pLongMaLength);

			var stochasticSlow = StochasticSlow.Calculate(closeArray, highArray, lowArray, pStochLength, pStochSmoothK, pStochSmoothD);

			//# Variables
			var maShortLast = maShort.Last();
			var maShortPrev = maShort.TakePrev();
			var maMiddleLast = maMiddle.Last();
			var maMiddlePrev = maMiddle.TakePrev();
			var maLongLast = maLong.Last();
			var maLongPrev = maLong.TakePrev();

			var stochasticSlowK = stochasticSlow.K.Last();
			var stochasticSlowD = stochasticSlow.D.Last();

			//# Decision
			//Daha once sirali degillerdi bu mumda siralandilar
			//short > middle > long olmamali oncesinde
			var areHOsSorting = !isOrderedForPosition(maShortPrev, maMiddlePrev, maLongPrev) //onceki barda mavi en ustte ya da en altta olmali
				&& (maShortLast >= maMiddleLast && maMiddleLast >= maLongLast); //mevcut barda da short > middle > long olarak dizilmeli

			//K cizgisinin D cizgisinden buyuk olmasini beklerim
			var stochasticSlowSorting = stochasticSlowK > stochasticSlowD;

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				  { "Long Last", $"EMA[{pShortMaLength}]:{maShortLast.RoundTo(5)} > EMA[{pMiddleMaLength}]:{maMiddleLast.RoundTo(5)} > EMA[{pLongMaLength}]:{maLongLast.RoundTo(5)} [{maShortLast >= maMiddleLast && maMiddleLast >= maLongLast}]" },
				  { "Long Prev", $"EMA[{pShortMaLength}]:{maShortPrev.RoundTo(5)} > EMA[{pMiddleMaLength}]:{maMiddlePrev.RoundTo(5)} > EMA[{pLongMaLength}]:{maLongPrev.RoundTo(5)} [{maShortPrev >= maMiddlePrev && maMiddlePrev >= maLongPrev}]" },
				  { "Result", $"{areHOsSorting && stochasticSlowSorting}" },
				},
				Result = areHOsSorting && stochasticSlowSorting
			};

			return checkResult;
		}

		public override CheckResult ShouldOpenShort()
		{
			//# Technical Analysis
			var maShort = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pShortMaLength)
				: TA.SMA.Calculate(closeArray, pShortMaLength);
			var maMiddle = pMA.ToLower() == "ema"
			   ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
			   : TA.SMA.Calculate(closeArray, pMiddleMaLength);
			var maLong = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pLongMaLength)
				: TA.SMA.Calculate(closeArray, pLongMaLength);

			var stochasticSlow = StochasticSlow.Calculate(closeArray, highArray, lowArray, pStochLength, pStochSmoothK, pStochSmoothD);

			//# Variables
			var maShortLast = maShort.Last();
			var maShortPrev = maShort.TakePrev();
			var maMiddleLast = maMiddle.Last();
			var maMiddlePrev = maMiddle.TakePrev();
			var maLongLast = maLong.Last();
			var maLongPrev = maLong.TakePrev();

			var stochasticSlowK = stochasticSlow.K.Last();
			var stochasticSlowD = stochasticSlow.D.Last();

			//# Decision
			//Daha once sirali degillerdi bu mumda siralandilar
			//short < middle < long olmamali oncesinde
			var areHOsSorting = !isOrderedForPosition(maShortPrev, maMiddlePrev, maLongPrev) //onceki barda mavi en ustte ya da en altta olmali
				&& (maLongLast >= maMiddleLast && maMiddleLast >= maShortLast); //mevcut barda da long > middle > short  olarak dizilmeli

			//D cizgisinin K cizgisinden buyuk olmasini beklerim
			var stochasticSlowSorting = stochasticSlowK < stochasticSlowD;

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				  { "Short Last", $"EMA[{pShortMaLength}]:{maShortLast.RoundTo(5)} < EMA[{pMiddleMaLength}]:{maMiddleLast.RoundTo(5)} < EMA[{pLongMaLength}]:{maLongLast.RoundTo(5)} [{maShortLast < maMiddleLast &&  maMiddleLast < maLongLast}]" },
				  { "Short Prev", $"EMA[{pShortMaLength}]:{maShortPrev.RoundTo(5)} < EMA[{pMiddleMaLength}]:{maMiddlePrev.RoundTo(5)} < EMA[{pLongMaLength}]:{maLongPrev.RoundTo(5)} [{maShortPrev < maMiddlePrev && maMiddlePrev < maLongPrev}]" },
				  { "Result", $"{areHOsSorting && stochasticSlowSorting}" },
				},
				Result = areHOsSorting && stochasticSlowSorting
			};

			return checkResult;
		}

		/// <summary>
		/// Kisa HO orta HO keserse take profit almalisin
		/// TODO: burada kademeli satış? yarısını orta HO, kalanını uzun HO
		/// </summary>
		/// <returns></returns>
		public override CheckResult ShouldCloseLong()
		{
			//# Technical Analysis
			var maShort = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pShortMaLength)
				: TA.SMA.Calculate(closeArray, pShortMaLength);
			var maMiddle = pMA.ToLower() == "ema"
			   ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
			   : TA.SMA.Calculate(closeArray, pMiddleMaLength);
			var maLong = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pLongMaLength)
				: TA.SMA.Calculate(closeArray, pLongMaLength);

			//# Variables
			var maShortLast = maShort.Last();
			var maMiddleLast = maMiddle.Last();
			var maLongLast = maLong.Last();

			//# Decision
			//Daha once sirali degillerdi bu mumda siralandilar
			//short < middle'ın altina duserse trend bozuluyor, cikmalisin
			var areHOsSorting = maShortLast < maMiddleLast;
			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"MaShort[{pShortMaLength}]",$"[{maShortLast.RoundTo(5)}]" },
				   { $"MaMiddle[{pMiddleMaLength}]",$"[{maMiddleLast.RoundTo(5)}]" },
				   { $"MaLong[{pLongMaLength}]",$"[{maLongLast.RoundTo(5)}]" },
				   { "ClosePrice",closeArray.Last() },
				},
				Result = areHOsSorting
			};

			return checkResult;
		}

		/// <summary>
		/// Kisa bar hem orta hem uzun u yukari keserse alip cikmalisin
		/// Siraya girmesini beklememelisin
		/// </summary>
		/// <returns></returns>
		public override CheckResult ShouldCloseShort()
		{
			//# Technical Analysis
			var maShort = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pShortMaLength)
				: TA.SMA.Calculate(closeArray, pShortMaLength);
			var maMiddle = pMA.ToLower() == "ema"
			   ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
			   : TA.SMA.Calculate(closeArray, pMiddleMaLength);
			var maLong = pMA.ToLower() == "ema"
				? TA.EMA.Calculate(closeArray, pLongMaLength)
				: TA.SMA.Calculate(closeArray, pLongMaLength);

			//# Variables
			var maShortLast = maShort.Last();
			var maMiddleLast = maMiddle.Last();
			var maLongLast = maLong.Last();

			//# Decision
			//Daha once sirali degillerdi bu mumda siralandilar
			//short > middle'ın altina duserse trend bozuluyor, cikmalisin
			var areHOsSorting = maShortLast > maMiddleLast;
			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"MaShort[{pShortMaLength}]",$"[{maShortLast.RoundTo(5)}]" },
				   { $"MaMiddle[{pMiddleMaLength}]",$"[{maMiddleLast.RoundTo(5)}]" },
				   { $"MaLong[{pLongMaLength}]",$"[{maLongLast.RoundTo(5)}]" },
				   { "ClosePrice",closeArray.Last() },
				},
				Result = areHOsSorting
			};

			return checkResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryPrice"></param>
		/// <param name="side"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
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
			var pMALower = pMA.ToLower();

            //# Technical Analysis
			var maShort = pMALower == "ema" ? TA.EMA.Calculate(closeArray, pShortMaLength) : TA.SMA.Calculate(closeArray, pShortMaLength);
			var maMiddle = pMALower == "ema" ? TA.EMA.Calculate(closeArray, pMiddleMaLength) : TA.SMA.Calculate(closeArray, pMiddleMaLength);
			var maLong = pMALower == "ema" ? TA.EMA.Calculate(closeArray, pLongMaLength) : TA.SMA.Calculate(closeArray, pLongMaLength);

			var stochasticSlow = StochasticSlow.Calculate(closeArray, highArray, lowArray, pStochLength, pStochSmoothK, pStochSmoothD);

			return new Dictionary<string, string>() {
				{ "MA",pMALower },
				{ $"{pMALower}[{pShortMaLength}]",maShort.Last().ToString() },
				{ $"{pMALower}[{pMiddleMaLength}]",maMiddle.Last().ToString() },
				{ $"{pMALower}[{pLongMaLength}]",maLong.Last().ToString() },
				{ $"STOCHSLOW", "Length:" + pStochLength },
				{ $"STOCHSLOW.K[{pStochSmoothK}]",stochasticSlow.K.Last().ToString() },
				{ $"STOCHSLOW.D[{pStochSmoothD}]",stochasticSlow.D.Last().ToString() },
			};
        }

        /// <summary>
        /// Toplamda olabilecek 6 kondüsyon var Mavi: M / Kırmızı: K / Yeşil: Y
        /// M > K > Y / M > Y > K //mavi yukarida ise not ordered
        /// Y > M > K / Y > K > M //mavi en altta ise not ordered
        /// K > M > Y / K > Y > M //mavi en altta ise not ordered
        /// Kisacasi mavi mutlaka ortada olacak
        /// Bizim kosulumuz nedir 
        ///     onceki barda mavi ortada olmayacak
        ///     mevcut barda ortada olacak, K ve Ynin durumu long ya da shortu belirleyecek
        /// </summary>
        private bool isOrderedForPosition(double green, double blue, double red)
		{
			//mavi en ustte ise
			if (blue >= green && blue >= red)
				return false;

			//mavi en altta ise
			if (green >= blue && red >= blue)
				return false;

			return true;
		}
	}
}
