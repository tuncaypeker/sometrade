namespace SomeTrade.Candles.Common
{
    public class CandleResult
    {
        public CandleResult(RetCode retCode, int begIdx, int nbElement, int[] integer)
        {
            Integer = integer;
            RetCode = retCode;
            BegIdx = begIdx;
            NBElement = nbElement;
        }

        public RetCode RetCode { get; set; }
        public int BegIdx { get; set; }
        public int NBElement { get; set; }

        public int[] Integer { get; }
    }
}
