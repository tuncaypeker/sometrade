using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
	/// <summary>
	/// AL: Hacim belirtilen deger kadar arttıysa ve mum yesil ise AL
	///     STOP: Hacim jump value'nun yarısı kadar terse sıçrarsa STOP OL
	/// SAT: Hacim belirtilen deger kadar arttıysa ve mum kirmisi ise SAT
	///      STOP: Hacim jump value'nun yarısı kadar terse sıçrarsa STOP OL
	/// </summary>
	public class VolumeJumpStrategy : StrategyBase
	{
		//parameters
		private int pJumpPercentage;

		public VolumeJumpStrategy(List<Candle> candles, Dictionary<string, object> parameters)
			: base(candles, parameters, "VolumeJumpStrategy", 8)
		{
			pJumpPercentage = GetParameterAsInt("JumpPercentage");
		}

		public override CheckResult ShouldOpenLong()
		{
			//# Technical Analysis
			var isJumped = _isVolumeJumped(volumeArray.Last(), volumeArray.TakePrev(), pJumpPercentage);
			var isGreen = _candles.Last().Color == Candle.CandleColor.Green;

			//# Variables
			var volumePriceLast = volumeArray.Last();
			var volumePricePrev = volumeArray.TakePrev();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"VolumeLast",volumePriceLast },
				   { $"VolumePrev",volumePricePrev },
				},
				Result = false
			};

			//# Decision
			if (isJumped && isGreen)
				checkResult.Result = true;

			return checkResult;
		}

		public override CheckResult ShouldOpenShort()
		{
			//# Technical Analysis
			var isJumped = _isVolumeJumped(volumeArray.Last(), volumeArray.TakePrev(), pJumpPercentage);
			var isRed = _candles.Last().Color == Candle.CandleColor.Red;

			//# Variables
			var volumePriceLast = volumeArray.Last();
			var volumePricePrev = volumeArray.TakePrev();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"VolumeLast",volumePriceLast },
				   { $"VolumePrev",volumePricePrev },
				},
				Result = false
			};

			//# Decision
			if (isJumped && isRed)
				checkResult.Result = true;

			return checkResult;
		}

		/// <summary>
		/// Terse kirmizi mum atarsa ve belirtilen degerin yarisi kadar sicradi ise
		/// </summary>
		/// <returns></returns>
		public override CheckResult ShouldTakeProfitLong()
		{
			//# Technical Analysis
			var isJumped = _isVolumeJumped(volumeArray.Last(), volumeArray.TakePrev(), pJumpPercentage/2);
			var isRed = _candles.Last().Color == Candle.CandleColor.Red;

			//# Variables
			var volumePriceLast = volumeArray.Last();
			var volumePricePrev = volumeArray.TakePrev();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"VolumeLast",volumePriceLast },
				   { $"VolumePrev",volumePricePrev },
				},
				Result = false
			};

			//# Decision
			if (isJumped && isRed)
				checkResult.Result = true;

			return checkResult;
		}

		public override CheckResult ShouldTakeProfitShort()
		{
			//# Technical Analysis
			var isJumped = _isVolumeJumped(volumeArray.Last(), volumeArray.TakePrev(), pJumpPercentage/2);
			var isRed = _candles.Last().Color == Candle.CandleColor.Green;

			//# Variables
			var volumePriceLast = volumeArray.Last();
			var volumePricePrev = volumeArray.TakePrev();

			var checkResult = new CheckResult()
			{
				PositionLogs = new Dictionary<string, object>() {
				   { $"VolumeLast",volumePriceLast },
				   { $"VolumePrev",volumePricePrev },
				},
				Result = false
			};

			//# Decision
			if (isJumped && isRed)
				checkResult.Result = true;

			return checkResult;
		}

		public override bool CheckAlarm()
		{
			return false;
		}

		public override bool UpdateTrailingStop()
		{
			return false;
		}

		public override bool ShouldStop(double entryPrice, int side)
		{
			return false;
		}

		public bool _isVolumeJumped(double _volumeLast, double _volumePrev, int jumpPercentage)
		{
			if (_volumeLast < _volumePrev)
				return false;

			var _diff = _volumeLast - _volumePrev;
			var percentage = 100 * _diff / _volumeLast;

			return percentage > jumpPercentage;
		}
	}
}
