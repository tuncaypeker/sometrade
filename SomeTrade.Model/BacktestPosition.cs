using System;

namespace SomeTrade.Model
{
    public class BacktestPosition : Core.ModelBase
    {
        public BacktestPosition()
        {
        }

        public int BacktestId { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal EntryBudget { get; set; }
        public decimal Quantity { get; set; }
        public int Side { get; set; }
        public DateTime ExitDate { get; set; }
        public decimal ExitPrice { get; set; }
        public decimal ExitBudget { get; set; }
        public decimal Fee { get; set; }
    }
}
