using System;
using System.Collections.Generic;

namespace SomeTrade.TA.Dto
{
	public class SwingResultDto
	{
		public SwingResultDto()
		{
			SwingHistory = new List<SwingHistory>();
		}

		/// <summary>
		/// -1: Short
		///  0: Unknown
		///  1: Long
		/// </summary>
		public int CurrentTrend { get; set; }
		public SwingPoint SwingHigh { get; set; }
		public SwingPoint SwingLow { get; set; }
		public PBSwingPoint PullBack { get; set; }

		public List<SwingHistory> SwingHistory { get; set; }

		public DateTime LastCandleOpenTime { get; set; }
		public double LastCandleClosePrice { get; set; }

		public bool Succeed { get; set; }
		public string Message { get; set; }
	}

	public class SwingPoint
	{
		public SwingPoint(DateTime date, double price)
		{
			this.Date = date;
			this.Price = price;
		}

		public SwingPoint(DateTime date, decimal price)
		{
			this.Date = date;
			this.Price = Convert.ToDouble(price);
		}

		public DateTime Date { get; set; }
		public double Price { get; set; }
	}

	public class PBSwingPoint : SwingPoint
	{
		public PBSwingPoint(DateTime date, double price, double percentage, int trend = 0)
			: base(date, price)
		{
			this.Percentage = percentage;
			this.Trend = trend;
		}

		public PBSwingPoint(DateTime date, decimal price, decimal percentage, int trend = 0)
			: base(date, Convert.ToDouble(price))
		{
			this.Percentage = Convert.ToDouble(percentage);
			this.Trend = trend;
		}

		public double Percentage { get; set; }

		/// <summary>
		/// Harmonic Pullback yonu pespese ayni olamaz
		/// bunun kontrolu icin fraktaldan aliyoruz bu bilgiyi
		/// pespese ayni dip ya da tepe fraktallari nasil geliyor
		/// esit high ya da low'lari fraktal saymadigimizdan bazen denk geliyor
		/// 
		/// -1: Tepe fraktali, yani bu noktadan dusuyor
		///  1: Dip fraktali, yani bu noktadan cikiyor
		/// </summary>
		public int Trend { get; set; }
	}

	public class SwingHistory
	{
		public SwingHistory(SwingPoint swingPoint, string description)
		{
			Description = description;
			SwingPoint = swingPoint;
		}

		public string Description { get; set; }
		public SwingPoint SwingPoint { get; set; }
	}
}
