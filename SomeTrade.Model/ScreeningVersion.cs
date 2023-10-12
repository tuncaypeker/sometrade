using System;

namespace SomeTrade.Model
{
    public class ScreeningVersion : Core.ModelBase
    {
        public int ScreeningId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string CurrentVersion { get; set; }
        public string Description { get; set; }
    }
}
