namespace SomeTrade.Model
{
    public class TvChartDemo : Core.ModelBase
    {
        /*
            SupportedResolutions = new[] { "1D" },
            SupportGroupRequest = false,
            SupportMarks = true,
            SupportSearch = true,
            SupportTimeScaleMarks = false
         */
        public string SupportedResolutions { get; set; }
        public bool SupportGroupRequest { get; set; }
        public bool SupportMarks { get; set; }
        public bool SupportSearch { get; set; }
        public bool SupportTimeScaleMarks { get; set; }

        public string SymbolName { get; set; } //BTCUSDT
        public string SymbolTicker { get; set; } //BTCUSDT
        public string SymbolDescription { get; set; }//BTCUSDT
        public string SymbolType { get; set; }//crypto
        public string SymbolExchangeTraded { get; set; }//Crypto
        public string SymbolExchangeListed { get; set; }//Crypto
        public string SymbolTimezone { get; set; }//Europe/Istanbul
        public string SymbolSession { get; set; }//24x7
        public string SymbolSupportedResolutions { get; set; }//"60","120","1D"
        public string SymbolCurrencyCode { get; set; }//BTC
        public string SymbolOriginalCurrencyCode { get; set; }//BTC
        public string SymbolIntradayMultipliers { get; set; }//"1D"

        public bool SymbolHasNoVolume { get; set; }//false
        public bool SymbolHasDaily { get; set; }//true
        public bool SymbolHasIntraday { get; set; }//false

        public int SymbolVolumePrecision { get; set; }//2
        public decimal SymbolMinMov { get; set; }//1
        public decimal SymbolMinMov2 { get; set; }//0
        public int SymbolPriceScale { get; set; }//100
        public int SymbolPointValue { get; set; }//1
    }
}
