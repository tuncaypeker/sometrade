using SomeTrade.Model.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace SomeTrade.Model
{
	public class Robot : Core.ModelBase
	{
		public Robot()
		{
			CandleLimit = 100;
			HeartBeat = DateTime.UtcNow;
		}

		public int ExchangeId { get; set; }
		public int StrategyId { get; set; }

		public string Name { get; set; }
		public TimeIntervalEnum Interval { get; set; }

		//In Seconds
		public int IntervalInSeconds { get; set; }

		public string Description { get; set; }

		public int MaxPosition { get; set; }
		public decimal StartBudget { get; set; }
		public decimal CurrentBudget { get; set; }
		public int PercentagePerPosition { get; set; }

		public int Leverage { get; set; }

		public bool IsActive { get; set; }

		/// <summary>
		/// Gecici sinyalde de pozisyon acip kapatmasina izin verelim mi
		/// </summary>
		public bool AllowOnTemporarySignal { get; set; }

		/// <summary>
		/// Gecici sinyalle isleme girmiyor ise opsiyonel olarak kac sn'e kadar islem yapabilir iznini belirler
		/// Bu 0 ya da -1 girilirse, icerde belirledigimzi default sureleri kullnair ornek olarak 1dk lık mum'da 10sn gibi
		/// </summary>
		public int AllowingTolerateSeconds { get; set; }

		/// <summary>
		/// Meta degerlerine gore exchange'den symbol'e ait kac tane mum verisi 
		/// almamiz gerektigini belirtir, varsayilan olarak 100 kabul ediyorum
		/// ama ema200 kullancaksak bunu robot'ta ayarlamalıyız
		/// </summary>
		public int CandleLimit { get; set; }

		public System.DateTime HeartBeat { get; set; }

		/// <summary>
		/// Bu true olarak isaretlenirse, RobotTickLog tablosuna strateji tarafindan gelen 
		/// hesaplamalari veritabanina kaydetmeye calisir
		/// </summary>
		public bool ShouldLog { get; set; }

		/// <summary>
		/// Yüzdesel iz süren stop var mi 1.5% de 1%ye stop koy gibi
		/// </summary>
		public bool HasPercentageTrailingStop { get; set; }

		/// <summary>
		/// 1.5/1;2/1.5;2.5/2 formatinda girmelisin
		/// </summary>
		public string TrailingStopPercentageLevels { get; set; }

		[Ignored]
		public Dictionary<decimal, decimal> GetTrailingStopPercentageLevels
		{
			get
			{
				Dictionary<decimal, decimal> levels = new Dictionary<decimal, decimal>();

				if (!HasPercentageTrailingStop || string.IsNullOrEmpty(TrailingStopPercentageLevels))
					return levels;

				var levelSplits = TrailingStopPercentageLevels.Split(";");
				var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

				foreach (var levelSplit in levelSplits)
				{
					var levelSplitParts = levelSplit.Split("/");

					decimal hitPercentage = Decimal.Parse(levelSplitParts[0].Replace(",", "."), numberFormatInfo);
					decimal stopPercentage = Decimal.Parse(levelSplitParts[1].Replace(",", "."), numberFormatInfo);

					levels.Add(hitPercentage, stopPercentage);
				}

				return levels;
			}
		}

		public int HardStopPercentage { get; set; }
	}
}
