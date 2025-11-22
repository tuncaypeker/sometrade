namespace SomeTrade.Model
{
    public class Strategy : Core.ModelBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CurrentVersion { get; set; }

        /// <summary>
        /// Trading view üzerinde pinescript ile backtest yapmak için kullanılacak
        /// </summary>
        public string PsBacktestCode { get; set; }
    }
}
