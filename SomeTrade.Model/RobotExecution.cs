using System;

namespace SomeTrade.Model
{
    public class RobotExecution : Core.ModelBase
    {
        public RobotExecution()
        {
        }

        public int RobotSymbolPairId { get; set; }
        public int RobotId { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// Burada decisions ve diger notlar olmalidir
        /// </summary>
        public string Notes { get; set; }
    }
}
