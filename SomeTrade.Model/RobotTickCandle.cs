using System;

namespace SomeTrade.Model
{
    /// <summary>
    /// RobotPositionCandle' ile ayni yapida ancak bir fark var
    /// Bu da robot eger log yapilsin olarak isaretlenmişse loglar
    /// ve position'a bagli degil direk robot'a bagli olarak ilerler
    /// </summary>
    public class RobotTickCandle : Core.ModelBase
    {
        public int RobotId { get; set; }
        public DateTime CloseDate { get; set; }
        public DateTime OpenDate { get; set; }
        
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
    }
}
