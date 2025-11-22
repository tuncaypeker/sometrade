namespace SomeTrade.Strategies.Dto
{
	public class TrailingStopResult
	{
        public TrailingStopResult(bool result, decimal newStopPrice)
        {
            Result = result;
            NewStopPrice = newStopPrice;
        }

        /// <summary>
        /// Update Trailing Stop or Not
        /// </summary>
        public bool Result { get; set; }

		/// <summary>
		/// New trailing stop price, if Result is true
		/// </summary>
        public decimal NewStopPrice { get; set; }
    }
}
