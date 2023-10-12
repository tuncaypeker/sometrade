using System;

namespace SomeTrade.Model
{
    public class Robot : Core.ModelBase
    {
        public Robot()
        {
            CandleLimit = 100;
            HeartBeat = DateTime.Now;
        }

        public int ExchangeId { get; set; }
        public int StrategyId { get; set; }

        public string Name { get; set; }
        public TimeIntervalEnum Interval { get; set; }

        //In Seconds
        public int IntervalInSeconds { get; set; }

        public string Description { get; set; }

        public int MaxPosition { get; set; }
        public decimal StartBudget { get; set; }
        public decimal CurrentBudget { get; set; }
        public int PercentagePerPosition { get; set; }

        public int Leverage { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// Gecici sinyalde de pozisyon acip kapatmasina izin verelim mi
        /// </summary>
        public bool AllowOnTemporarySignal { get; set; }

        /// <summary>
        /// Gecici sinyalle isleme girmiyor ise opsiyonel olarak kac sn'e kadar islem yapabilir iznini belirler
        /// Bu 0 ya da -1 girilirse, icerde belirledigimzi default sureleri kullnair ornek olarak 1dk lık mum'da 10sn gibi
        /// </summary>
        public int AllowingTolerateSeconds { get; set; }

        /// <summary>
        /// Meta degerlerine gore exchange'den symbol'e ait kac tane mum verisi 
        /// almamiz gerektigini belirtir, varsayilan olarak 100 kabul ediyorum
        /// ama ema200 kullancaksak bunu robot'ta ayarlamalıyız
        /// </summary>
        public int CandleLimit { get; set; }

        public System.DateTime HeartBeat { get; set; }

        /// <summary>
        /// Bu true olarak isaretlenirse, RobotTickLog tablosuna strateji taragindan gelen 
        /// hesaplamalari veritabanina kaydetmeye calisir
        /// </summary>
        public bool ShouldLog { get; set; }
    }
}
