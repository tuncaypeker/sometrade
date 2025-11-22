using System;

namespace SomeTrade.Model
{
    public class RobotPositionCandle : Core.ModelBase
    {
        public int RobotPositionId { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Volume { get; set; }
        public DateTime OpenTime { get; set; }
    }
}
