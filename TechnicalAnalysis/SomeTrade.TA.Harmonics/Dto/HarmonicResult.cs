using System.Collections.Generic;

namespace SomeTrade.TA.Harmonics
{
	public class HarmonicResult
	{
        public HarmonicResult()
        {
			Detections = new List<HarmonicItem>();
        }

        public HarmonicItem Potential { get; set; }
        public List<HarmonicItem> Detections { get; set; }
    }
}
