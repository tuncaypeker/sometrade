using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
	/// <summary>
	/// AL: Kapanis 100 Periyot Highest uzerinde ve bullish ise long
	///     STOP: Trend Bearish olursa ya da fiyat 50 periyot Lowest altında düşerse
	/// SAT: Kapanis 100 Periyot Lowest altinda ve bearish ise short
	///      STOP: Trend Bullish olursa ya da fiyat 50 periyot highest üstüne çıkarsa
	///      
	///
	/*
	 
	ETH/USDT 4H'de delilik 2.252%
	BTC/USDT 4H'de güzel 277%
	DOGE/USDT 4H'de güzel 700%
	DOGE/USDT Gunluk'te delilik 1.900%
	BNB/USDT 4H'da guzel 665%
	AVAX/USDT 4H'da guzel 784%

	// This source code is subject to the terms of the Mozilla Public License 2.0 at https://mozilla.org/MPL/2.0/
	// © tuncaypeker_

	//@version=5
	strategy("My strategy", overlay=true, margin_long=100, margin_short=100)

	shortMA = input(50, "Short EMA Value")
	longMA = input(100, "Long EMA Value")

	EMA50 = ta.ema(close,shortMA)
	EMA100 = ta.ema(close,longMA)
	   
	isBearish = EMA50 < EMA100
	isBullish = EMA50 > EMA100

	isHigherThanHighestLong = close > ta.highest(close,longMA)[1]
	isLowerThanLowestLong = close < ta.lowest(close,longMA)[1]

	isHigherThanHighestShort = close > ta.highest(close,shortMA)[1]
	isLowerThanLowestShort = close < ta.lowest(close,shortMA)[1]

	longCondition = isBullish and isHigherThanHighestLong
	if (longCondition)
		strategy.entry("LongPoz", strategy.long)

	shortCondition = isBearish and isLowerThanLowestLong
	if (shortCondition)
		strategy.entry("ShortPoz", strategy.short)

	if((isBullish[1] and isBearish) or isLowerThanLowestShort)
		strategy.close("LongPoz")

	if((isBearish[1] and isBullish) or isHigherThanHighestShort)
		strategy.close("ShortPoz")

	 */ 

	///TODO/Gelişim Noktaları
	///1: Mum kapanışı beklemeden çıkabilir miyiz
	///2: İz süren stop olabilir mi?
	///3: Genelde 4H likte çalışıyor major coinlerde, bunu 30dklara indirebilir miyiz
	/// </summary>
	public class TrendFollowerStrategy : StrategyBase
	{
		//parameters
		private int pShortMALength;
		private int pLongMALength;

		public TrendFollowerStrategy(List<Candle> candles, Dictionary<string, object> parameters)
			: base(candles, parameters, "TrendFollowerStrategy", 10)
		{
			pShortMALength = GetParameterAsInt("ShortMALength");
			pLongMALength = GetParameterAsInt("LongMALength");
		}

		public override CheckResult ShouldOpenLong()
		{
			//# Technical Analysis
			var EMAShort = TA.EMA.Calculate(closeArray, pShortMALength);
			var EMALong = TA.EMA.Calculate(closeArray, pLongMALength);
			
			//# Variables
			var isBullish = EMAShort.Last() > EMALong.Last();
			var isHigherThan = closeArray.Last() > TA.Highest.Calculate(highArray, pLongMALength).Last();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"IsBullish",isBullish },
				   { $"IsHigherThan",isHigherThan },
				},
				Result = false
			};

			//# Decision
			if (isBullish && isHigherThan)
				checkResult.Result = true;

			return checkResult;
		}

		public override CheckResult ShouldOpenShort()
		{
			//# Technical Analysis
			var EMAShort = TA.EMA.Calculate(closeArray, pShortMALength);
			var EMALong = TA.EMA.Calculate(closeArray, pLongMALength);
			
			//# Variables
			var isBearish = EMALong.Last() > EMAShort.Last();
			var isLowerThan = closeArray.Last() < TA.Lowest.Calculate(lowArray, pLongMALength).Last();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"IsBearish",isBearish },
				   { $"IsLowerThan",isLowerThan },
				},
				Result = false
			};

			//# Decision
			if (isBearish && isLowerThan)
				checkResult.Result = true;

			return checkResult;
		}

		//STOP: Trend Bearish olursa ya da fiyat 50 periyot Lowest altında düşerse
		public override CheckResult ShouldCloseLong()
		{
			//# Technical Analysis
			var EMAShort = TA.EMA.Calculate(closeArray, pShortMALength);
			var EMALong = TA.EMA.Calculate(closeArray, pLongMALength);
			
			//# Variables
			var isBearish = EMALong.Last() > EMAShort.Last();
			var isLowerThan = closeArray.Last() < TA.Lowest.Calculate(lowArray, pShortMALength).Last();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"IsBearish",isBearish },
				   { $"IsLowerThan",isLowerThan },
				},
				Result = false
			};

			//# Decision
			if (isBearish && isLowerThan)
				checkResult.Result = true;

			return checkResult;
		}

		//STOP: Trend Bullish olursa ya da fiyat 50 periyot highest üstüne çıkarsa
		public override CheckResult ShouldCloseShort()
		{
			//# Technical Analysis
			var EMAShort = TA.EMA.Calculate(closeArray, pShortMALength);
			var EMALong = TA.EMA.Calculate(closeArray, pLongMALength);
			
			//# Variables
			var isBullish = EMAShort.Last() > EMALong.Last();
			var isHigherThan = closeArray.Last() > TA.Highest.Calculate(highArray, pShortMALength).Last();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"IsBullish",isBullish },
				   { $"IsHigherThan",isHigherThan },
				},
				Result = false
			};

			//# Decision
			if (isBullish && isHigherThan)
				checkResult.Result = true;

			return checkResult;
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
