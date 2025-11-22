using System;
using System.Collections.Generic;

namespace SomeTrade.Model
{
    /// <summary>
    /// Bir pozisyon ile ilgili girilen stoplari loglar
    /// Ornek olarak iz suren stop guncellendi
    /// Ornek olarak manuel stop girildi
    /// </summary>
    public class RobotPositionStopLog : Core.ModelBase
    {
        public int RobotPositionId { get; set; }
        public decimal StopPrice { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public string PercentageLevel { get; set; }
    }
}