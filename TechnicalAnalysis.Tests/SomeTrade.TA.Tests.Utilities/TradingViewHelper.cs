using SomeTrade.Infrastructure.Extensions;
using System.Collections.Generic;

namespace SomeTrade.TA.Tests.Utilities
{
	/// <summary>
	/// Trading View > Layout > Export Chart Data ile alınmış indikator verilerini
	/// Mum verisi olarak verir
	/// Bu da ornek olarak binance ve Trading View uzerinde binance verileri arasındaki tutarsızlığın onune gecmemizi saglar
	/// 
	/// time,      open, high,    low,     close,   Plot,             Plot,Plot
	/// 1694653200,26522,26541.77,26126.77,26173.37,25906.32361211407,NaN, NaN
	/// </summary>
	public static class TradingViewHelper
    {
        public static List<DataCandle> GetCandles(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            var candles = new List<DataCandle>();

            foreach (var line in lines)
            {
                if (line.StartsWith("time"))
                    continue;

                var splitArr = line.Split(',');

                candles.Add(new DataCandle()
                {
                    Time = splitArr[0].ToUnixTimeToDateTime(),
                    Open = double.Parse(splitArr[1].Replace(".",",")),
                    High = double.Parse(splitArr[2].Replace(".",",")),
                    Low = double.Parse(splitArr[3].Replace(".",",")),
                    Close = double.Parse(splitArr[4].Replace(".",","))
                });
            }
            
            return candles;
        }
      
    }
}
