namespace SomeTrade.Model
{
    public class RobotMetaValue : Core.ModelBase
    {
        public int RobotId { get; set; }
        public int StrategyMetaId { get; set; }
        public string Value { get; set; }
    }
}
