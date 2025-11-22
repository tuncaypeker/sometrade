namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.Infrastructure.Extensions;
    using SomeTrade.TA.Tests.Utilities;
    using System.Collections.Generic;
    using System;
    using System.Linq;
    using Xunit;

    public class DEMATests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = DEMA.Calculate(close, 9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 12); //8339.25
            Assert.True(actualResult[i3] > 8339 && actualResult[i3] < 8340);

            var i1 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 4); //51269.25
            Assert.True(actualResult[i1] > 51269 && actualResult[i1] < 51270);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 10);//53616.48
            Assert.True(actualResult[i2] > 53616 && actualResult[i2] < 53617);
        }

        
        [Fact]
        public void Execute_DEMA9_1D_Csv_Data_Export()
        {
            // Arrange
            var close = new List<double>();
            var dateTimes = new List<DateTime>();
            var demas = new List<double>();

            var csvLines = System.IO.File.ReadAllLines("_data/_indicator_data/DEMA_9_CLOSE_BINANCE_BTCUSDT, 1D.csv");
            //time,open,high,low,close,DEMA
            foreach (var line in csvLines.Skip(1))
            {
                var lineArr = line.Split(',');

                dateTimes.Add(DateExtensions.ToJsonTimeToDateTime(lineArr[0]));
                close.Add(HistoricalDataCsvExtensions._convertToDouble(lineArr[4]));
                demas.Add(HistoricalDataCsvExtensions._convertToDouble(lineArr[5]));
            }

            // Act
            var actualResult = DEMA.Calculate(close.ToArray(), 9);

            Assert.Equal(0, actualResult[5]);
            Assert.Equal(actualResult[100], demas[100]);
            Assert.Equal(actualResult[200], demas[200]);
            Assert.Equal(actualResult[500], demas[500]);
            Assert.Equal(actualResult[1000], demas[1000]);
        }
    }
}
