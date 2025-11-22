namespace SomeTrade.TA.Indicators.Dto
{
    public class AlphaTrendResultDto
    {
        public double[] AlphaTrend { get; set; }
        public double[] AlphaTrendK2 { get; set; }
        public int[] BuySignal { get; set; }
        public int[] SellSignal { get; set; }
        public string[] Color { get; set; }
    }
}
