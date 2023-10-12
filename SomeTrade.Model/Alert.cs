namespace SomeTrade.Model
{
    public class Alert : Core.ModelBase
    {
        public Alert()
        {
            CandleLimit = 100;
        }

        public int ExchangeId { get; set; }
        public int ScreeningId { get; set; }

        public string Name { get; set; }
        public TimeIntervalEnum Interval { get; set; }

        public int IntervalInSeconds { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Gecici sinyalde de pozisyon acip kapatmasina izin verelim mi
        /// </summary>
        public bool AllowOnTemporarySignal { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Meta degerlerine gore exchange'den symbol'e ait kac tane mum verisi 
        /// almamiz gerektigini belirtir, varsayilan olarak 100 kabul ediyorum
        /// ama ema200 kullancaksak bunu robot'ta ayarlamalıyız
        /// </summary>
        public int CandleLimit { get; set; }
    }
}
