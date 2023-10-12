using System;

namespace SomeTrade.Model
{
    public class ManuelPositionHistory : Core.ModelBase
    {
        public string Pair { get; set; }
        public int Direction { get; set; }
        public decimal Amount { get; set; }
        public int Leverage { get; set; }
        public decimal EntryPrice { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal ExitPrice { get; set; }
        public DateTime ExitDate { get; set; }
        public decimal StopPrice { get; set; }
        public string EntryDescription { get; set; }
        public string RiskDescription { get; set; }
        public string GeneralDescription { get; set; }
        public bool HasClosed { get; set; }
    }
}
