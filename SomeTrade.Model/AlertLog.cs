namespace SomeTrade.Model
{
    public class AlertLog : Core.ModelBase
    {
        public int AlertId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public bool IsSucceed { get; set; }
        public string Values { get; set; }
    }
}
