namespace SomeTrade.Model
{
    public class AlertMetaValue : Core.ModelBase
    {
        public int AlertId { get; set; }
        public int ScreeningMetaId { get; set; }
        public string Value { get; set; }
    }
}
