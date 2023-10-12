namespace SomeTrade.Model
{
    public class StrategyMeta : Core.ModelBase
    {
        public int StrategyId { get; set; }

        public string Name { get; set; }
        public string PlaceHolder { get; set; }
        public string DefaultValue { get; set; }

        /// <summary>
        /// 1- Textbox
        /// 2- Checkbox
        /// </summary>
        public int InputType { get; set; }

        public int Order { get; set; }
    }
}
