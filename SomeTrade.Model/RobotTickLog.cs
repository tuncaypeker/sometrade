using System;

namespace SomeTrade.Model
{
    /// <summary>
    /// RobotPositionLog' ile ayni yapida ancak bir fark var
    /// Bu da robot eger log yapilsin olarak isaretlenmişse loglar
    /// ve position'a bagli degil direk robot'a bagli olarak ilerler
    /// </summary>
    public class RobotTickLog : Core.ModelBase
    {
        public int RobotId { get; set; }
        public DateTime Date { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
