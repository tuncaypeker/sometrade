using System;
using System.Collections.Generic;
using System.Globalization;
using SomeTrade.Infrastructure.Extensions;

namespace SomeTrade.Infrastructure.Extensions
{
    public static class HistoricalDataCsvExtensions
    {
        /// <summary>
        /// Uygun csv dosyasindan satirlari okuyarak list of RangeBarModel'e donusturur
        /// Csv file must has comma as seperator
        /// </summary>
        /// <param name="csvFilePath">Exact Csv File Path</param>
        /// <returns></returns>
        public static List<RangeBarModel> LoadCandles(this string csvFilePath)
        {
            var rows = System.IO.File.ReadAllLines(csvFilePath);
            var list = new List<RangeBarModel>();

            //parse rows
            foreach (var row in rows)
            {
                var cols = row.Split(',');

                var historicalData = new RangeBarModel();
                historicalData.Close = _convertToDouble(cols[4]);
                historicalData.High = _convertToDouble(cols[2]);
                historicalData.Low = _convertToDouble(cols[3]);
                historicalData.Open = _convertToDouble(cols[1]);
                historicalData.Date = cols[0].ToJsonTimeToDateTime();
                historicalData.Volume = _convertToDouble(cols[5]);

                list.Add(historicalData);
            }

            return list;
        }

        //TODO: birkac yerde daha bu metod'dan gordum, tel noktaya alinabilir
        public static double _convertToDouble(string text)
        {
            double value;
            text = text.Replace(',', '.');
            double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

            return value;
        }
    }

    public class RangeBarModel
    {
        public RangeBarModel() { }

        public RangeBarModel(double open, double high, double low, double close, DateTime time, double volume)
        {
            Close = close;
            High = high;
            Low = low;
            Open = open;
            Date = time;
            Volume = volume;
        }

        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
    }
}
