using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using SomeTrade.TA;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs.Helpers
{
    public class BinanceHelper
    {
        BinanceClient binanceClient;
        public BinanceHelper(BinanceClient binanceClient)
        {
            this.binanceClient = binanceClient;
        }

        public JobBinanceCandlesDto GetCandles(string symbol, KlineInterval interval, int limit)
        {
            //Canli fiyat verisi
            var binanceCandlesResult = binanceClient.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, interval: interval, limit: limit).Result;

            if (!binanceCandlesResult.Success)
                return null;

            var candles = binanceCandlesResult.Data.ToList();

            return new JobBinanceCandlesDto()
            {
                //Son mum henüz kapanmadigi icin, dikkate almiyoruz. Tamamen kapanmis mumlar uzerinde islem yapiyoruz
                //TODO: bu dogru mu sence, Yani gecici sinyal ile islem yapmak isteyen bir bot icinde oldugu mum verisini alamazsa patlamaz mi?
                Candles = candles.Take(limit - 1).ToList(),
            };
        }
    }

    public class JobBinanceCandlesDto
    {
        public List<IBinanceKline> Candles { get; set; }
        public IBinanceKline Last
        {
            get
            {
                return Candles.Last();
            }
        }

        public IBinanceKline PrevLast
        {
			get
			{
                return Candles[Candles.Count-2];
            }
        }

        public DateTime LastCandleCloseTime
        {
            get
            {
                return Last.CloseTime;
            }
        }

        public decimal ClosePrice
        {
            get
            {
                return Last.ClosePrice;
            }
        }

        public void WriteToLogFile(string logFilePath)
        {
            if (System.IO.File.Exists(logFilePath))
                System.IO.File.Delete(logFilePath);

            //simdi'de log dosyasina yazalim
            var csvLines = new List<string>();
            foreach (var cfe in Candles)
            {
                var csvLine = $"{cfe.OpenTime};{cfe.OpenPrice};{cfe.HighPrice};{cfe.LowPrice};{cfe.ClosePrice};{cfe.Volume};{cfe.CloseTime};{cfe.QuoteVolume};{cfe.TradeCount};{cfe.TakerBuyBaseVolume};{cfe.TakerBuyQuoteVolume}";
                csvLines.Add(csvLine);
            }

            System.IO.File.WriteAllLines(logFilePath, csvLines);
        }

        public void AppendLastCandleToLogFile(string logFilePath)
        {
            var cfe = Last;
            var csvLine = $"{cfe.OpenTime};{cfe.OpenPrice};{cfe.HighPrice};{cfe.LowPrice};{cfe.ClosePrice};{cfe.Volume};{cfe.CloseTime};{cfe.QuoteVolume};{cfe.TradeCount};{cfe.TakerBuyBaseVolume};{cfe.TakerBuyQuoteVolume}";

            System.IO.File.AppendAllLines(logFilePath, new List<string>() { csvLine });
        }

        public bool FillFromLogFile(string logFilePath)
        {
            if (!System.IO.File.Exists(logFilePath))
                return false;

            Candles = new List<IBinanceKline>();
            var lines = System.IO.File.ReadAllLines(logFilePath);

            foreach (var line in lines)
            {
                var stringArr = line.Split(';');

                var kline = new BinanceKline()
                {
                    OpenTime = DateTime.Parse(stringArr[0]),
                    OpenPrice = decimal.Parse(stringArr[1]),
                    HighPrice = decimal.Parse(stringArr[2]),
                    LowPrice = decimal.Parse(stringArr[3]),
                    ClosePrice = decimal.Parse(stringArr[4]),
                    Volume = decimal.Parse(stringArr[5]),
                    CloseTime = DateTime.Parse(stringArr[6]),
                    QuoteVolume = decimal.Parse(stringArr[7]),
                    TradeCount = int.Parse(stringArr[8]),
                    TakerBuyBaseVolume = decimal.Parse(stringArr[9]),
                    TakerBuyQuoteVolume = decimal.Parse(stringArr[10]),
                };

                Candles.Add(kline);
            }

            return true;
        }
    }

    public class BinanceKline : IBinanceKline
    {
        public DateTime OpenTime { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Volume { get; set; }
        public DateTime CloseTime { get; set; }
        public decimal QuoteVolume { get; set; }
        public int TradeCount { get; set; }
        public decimal TakerBuyBaseVolume { get; set; }
        public decimal TakerBuyQuoteVolume { get; set; }
    }
}
