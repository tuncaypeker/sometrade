using SomeTrade.TA.Dto;

namespace SomeTrade.TA.Harmonics
{
	public class HarmonicItem
	{ 
		public HarmonicItem()
		{

		}

		public HarmonicItem(int _trend, PBSwingPoint _x, PBSwingPoint _a, PBSwingPoint _b, PBSwingPoint _c, PBSwingPoint _d)
		{
			this.Trend = _trend;
			this.X = _x;
			this.A = _a;
			this.B = _b;
			this.C = _c;
			this.D = _d;
		}

		public SwingPoint X { get; set; }
		public SwingPoint A { get; set; }
		public PBSwingPoint B { get; set; }
		public PBSwingPoint C { get; set; }
		public PBSwingPoint D { get; set; }
		public int Trend { get; set; }
	}
}
