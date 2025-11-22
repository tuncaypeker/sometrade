using System;

namespace SomeTrade.TA.Tests.Utilities
{
    public class DataCandle
    {
        public DataCandle()
        {
            
        }

        public DataCandle(decimal open, decimal high, decimal low, decimal close, DateTime time, decimal volume)
        {
            Close = (double)close;
            High = (double)high;
            Low = (double)low;
            Open = (double)open;
            Time = time;
            Volume = (double)volume;
        }

        public DataCandle(double open, double high, double low, double close, DateTime time, double volume)
        {
            Close = close;
            High = high;
            Low = low;
            Open = open;
            Time = time;
            Volume = volume;
        }

         public DataCandle(double open, double high, double low, double close, string time, double volume)
        {
            Close = close;
            High = high;
            Low = low;
            Open = open;
            Time = DateTime.Parse(time);
            Volume = volume;
        }

        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Open { get; set; }
        public DateTime Time { get; set; }
        public double Volume { get; set; }
    }
}
