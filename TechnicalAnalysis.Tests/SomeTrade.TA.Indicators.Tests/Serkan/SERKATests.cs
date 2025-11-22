namespace SomeTrade.TA.Indicators.Tests.Serkan
{
    using System.Linq;
    using SomeTrade.TA.Indicators.Serkan;
    using SomeTrade.TA.Tests.Utilities;
    using Xunit;

    public class SERKATests
    {
        [Fact]
        public void Execute_Length14_fast2_slow30()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = SERKA.Calculate(close, length: 14, fastLength: 2, slowLength: 30);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //339,
            Assert.True(actualResult.Result[i3] > 339 && actualResult.Result[i3] < 340);
            Assert.Equal("green", actualResult.Color[i3]);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //2083,84
            Assert.True(actualResult.Result[i1] > 2083 && actualResult.Result[i1] < 2084);
            Assert.Equal("green", actualResult.Color[i1]);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//7876,
            Assert.True(actualResult.Result[i2] > 7876 && actualResult.Result[i2] < 7877);
            Assert.Equal("green", actualResult.Color[i2]);
        }

        [Fact]
        public void Execute_Length20_fast2_slow30()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = SERKA.Calculate(close, length: 20, fastLength: 2, slowLength: 30);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //432,66
            Assert.True(actualResult.Result[i3] > 432 && actualResult.Result[i3] < 433);
            Assert.Equal("green", actualResult.Color[i3]);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //2010,10
            Assert.True(actualResult.Result[i1] > 2010 && actualResult.Result[i1] < 2011);
            Assert.Equal("green", actualResult.Color[i1]);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//8605,28
            Assert.True(actualResult.Result[i2] > 8605 && actualResult.Result[i2] < 8606);
            Assert.Equal("green", actualResult.Color[i2]);
        }
    }
}
