namespace SomeTrade.Model
{
    public class SymbolPair : Core.ModelBase
    {
        public int ExchangeId { get; set; }
        public int ToSymbolId { get; set; }
        public int FromSymbolId { get; set; }
    }
}
