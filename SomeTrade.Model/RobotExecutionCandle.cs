using System;

namespace SomeTrade.Model
{
    public class RobotExecutionCandle : Core.ModelBase
    {
        public int RobotId { get; set; }
        public int RobotExecutionId { get; set; }

        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Volume { get; set; }
        public DateTime StartTime { get; set; }
        public string Symbol { get; set; }
    }
}
