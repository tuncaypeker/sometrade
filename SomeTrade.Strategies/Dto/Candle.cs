using System;

namespace SomeTrade.Strategies.Dto
{
    public class Candle
    {
        public enum CandleColor
        {
            Red,
            Green
        }

        public Candle()
        {

        }

        public Candle(DateTime _date, decimal _open, decimal _close, decimal _low, decimal _high, decimal _volume)
        {
            Date = _date;
            Open = Convert.ToDouble(_open);
            Close = Convert.ToDouble(_close);
            Low = Convert.ToDouble(_low);
            High = Convert.ToDouble(_high);
            Volume = Convert.ToDouble(_volume);
        }

        public Candle(DateTime _date, double _open, double _close, double _low, double _high, double _volume)
        {
            Date = _date;
            Open = _open;
            Close = _close;
            Low = _low;
            High = _high;
            Volume = _volume;
        }

        public double Open { get; set; }
        public double Close { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Volume { get; set; }
        public DateTime Date { get; set; }

        public CandleColor Color
        {
            get { 
                return Close > Open 
                    ? CandleColor.Green
                    : CandleColor.Red;
            }
        }
    }
}
