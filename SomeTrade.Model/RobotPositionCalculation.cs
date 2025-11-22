using System;

namespace SomeTrade.Model
{
    public class RobotPositionCalculation : Core.ModelBase
    {
        public int RobotPositionId { get; set; }
        public DateTime Date { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
