using System;

namespace SomeTrade.Model
{
    public class Backtest : Core.ModelBase
    {
        public Backtest()
        {
            Fee = 0.0500m;
        }

        public int DataFileId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StrategyId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool HasEnded { get; set; }
        public decimal StartBudget { get; set; }
        public decimal FinishBudget { get; set; }
        public int CandleCount { get; set; }
        public int PositionCount { get; set; }
        public decimal MaxProfit { get; set; }
        public decimal MaxLoss { get; set; }
        public DateTime CandleStart { get; set; }
        public DateTime CandleEnd { get; set; }
        public decimal Fee { get; set; }
    }
}
