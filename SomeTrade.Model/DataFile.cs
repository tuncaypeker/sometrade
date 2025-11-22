using Microsoft.VisualBasic.FileIO;
using System;

namespace SomeTrade.Model
{
    /// <summary>
    /// Mum verilerini barindiran dosyalar
    /// Binance, TradingView ya da başka bir veri kaynagindan gelebilir
    /// </summary>
    public class DataFile : Core.ModelBase
    {
        public DataFile()
        {
        }

        public string Symbol { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// <option value="1">binance.com/en/landing/data</option>
        /// <option value="2">trading view export</option>
        /// </summary>
        public int DataFileType { get; set; }
        public string FilePath { get; set; }

        public int CandleCount { get; set; }
        public DateTime FirstCandle { get; set; }
        public DateTime LastCandle { get; set; }

        /// <summary>
        /// 3 5 15 30 1H 4H  1D
        /// 3 5 15 30 60 240 1440
        /// </summary>
        public int TimeFrame { get; set; }
    }
}
