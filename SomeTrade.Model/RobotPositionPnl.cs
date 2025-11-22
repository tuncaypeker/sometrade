using System;

namespace SomeTrade.Model
{
    public class RobotPositionPnl : Core.ModelBase
    {
        public int RobotPositionId { get; set; }
        public DateTime Date { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal Percentage { get; set; }
        public decimal CurrentProfit { get; set; }
    }
}
