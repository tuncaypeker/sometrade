using System;

namespace SomeTrade.TA.Dto
{
	public class CandleDto
	{
        public double LowPrice { get; set; }
		public double HighPrice { get; set; }
        public double ClosePrice { get; set; }
        public double OpenPrice { get; set; }
        public DateTime OpenTime { get; set; }

        public CandleDto()
        {
            
        }

        public CandleDto(double _lowPrice, double _highPrice, double _closePrice, double _openPrice, DateTime _openTime)
        {
            LowPrice = _lowPrice; 
            HighPrice = _highPrice; 
            ClosePrice = _closePrice; 
            OpenPrice = _openPrice;
            OpenTime = _openTime;
        }

        public CandleDto(decimal _lowPrice, decimal _highPrice, decimal _closePrice, decimal _openPrice, DateTime _openTime)
        {
            LowPrice = Convert.ToDouble(_lowPrice); 
            HighPrice = Convert.ToDouble(_highPrice); 
            ClosePrice = Convert.ToDouble(_closePrice); 
            OpenPrice = Convert.ToDouble(_openPrice);
            OpenTime = _openTime;
        }
    }
}
