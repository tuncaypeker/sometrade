using System;
using System.Collections.Generic;

namespace SomeTrade.Model
{
    public class RobotPosition : Core.ModelBase
    {
        public RobotPosition()
        {
            Notes = "";
            IsClosed = false;
            EntryDate = DateTime.Now;
            ExitPrice = 0;
            ExitDate = new DateTime(1970, 1, 1);
            ExitBudget = 0;
            SyncedDate = new DateTime(1970, 1, 1);
            SyncedExchange = false;
        }

        public int RobotId { get; set; }

        public string Symbol { get; set; }

        public DateTime EntryDate { get; set; }
        public decimal EntryPrice { get; set; }
        public decimal EntryBudget { get; set; }
        public decimal Quantity { get; set; }
        public int Side { get; set; }
        public bool IsClosed { get; set; }
        public decimal ExitPrice { get; set; }
        public DateTime ExitDate { get; set; }
        public decimal ExitBudget { get; set; }
        public string Notes { get; set; }

        public int Leverage { get; set; }

        //Exchange' ile ilgili alanlar
        public bool SyncedExchange { get; set; }
        public DateTime SyncedDate { get; set; }
        public long ExchangeEntryOrderId { get; set; }
        public long ExchangeExitOrderId { get; set; }
        public decimal ExchangeEntryPrice { get; set; }
        public decimal LiquidationPrice { get; set; }
        public decimal ExchangeQuantity { get; set; }

        public virtual ICollection<RobotPositionLog> RobotPositionLogs { get; set; }
        public virtual ICollection<RobotPositionCandle> RobotPositionCandles { get; set; }
        public virtual ICollection<RobotPositionPnl> RobotPositionPnls { get; set; }
    }
}