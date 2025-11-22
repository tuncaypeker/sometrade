namespace SomeTrade.TA.Dto
{
	public class FractalResultDto
	{
        public FractalResultDto()
        {
			
        }

        public bool IsFractal { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        ///  1: Tepe Fraktali yani en yuksek fiyati onceki ve sonraki iki mumun ustunde
        /// -1: Dip Fraktali yani en dusuk fiyati onceki ve sonraki iki mumun altinda 
        /// </summary>
        public int Trend { get; set; }
    }
}
