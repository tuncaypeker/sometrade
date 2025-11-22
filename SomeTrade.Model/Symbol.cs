namespace SomeTrade.Model
{
    public class Symbol : Core.ModelBase
    {
        public string Name { get; set; }
        public string Short { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// precision-is-over-the-maximum-defined-for-this-asset-binance
        /// hatasinin cozumu icin kullanilacak
        /// , den sonra kac basamak gelebilir
        /// </summary>
        public int Precision { get; set; }
    }
}
