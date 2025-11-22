using System;

namespace SomeTrade.Model
{
    public class RobotExecutionCalculation : Core.ModelBase
    {
        public int RobotId { get; set; }
        public int RobotExecutionId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
