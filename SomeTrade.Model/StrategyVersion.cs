using System;

namespace SomeTrade.Model
{
    public class StrategyVersion : Core.ModelBase
    {
        public int StrategyId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string CurrentVersion { get; set; }
        public string Description { get; set; }
    }
}
