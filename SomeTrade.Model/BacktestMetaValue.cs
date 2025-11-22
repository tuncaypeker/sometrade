namespace SomeTrade.Model
{
    public class BacktestMetaValue : Core.ModelBase
    {
        public int BacktestId { get; set; }
        public int StrategyMetaId { get; set; }
        public string Value { get; set; }
    }
}
