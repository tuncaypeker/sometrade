namespace SomeTrade.Model
{
    public class ScreeningMeta : Core.ModelBase
    {
        public int ScreeningId { get; set; }

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
