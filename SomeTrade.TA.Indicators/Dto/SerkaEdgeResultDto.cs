namespace SomeTrade.TA.Indicators.Dto
{
    public class SerkaEdgeResultDto
    {
        public double[] Hband { get; set; }
        public double[] Lband { get; set; }
        public double[] Filt { get; set; }
        public bool[] LongCondition { get; set; }
        public bool[] ShortCondition { get; set; }
    }
}
