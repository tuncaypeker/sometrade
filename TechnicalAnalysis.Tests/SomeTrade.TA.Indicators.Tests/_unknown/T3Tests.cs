namespace SomeTrade.TA.Indicators.Tests
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class T3Tests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = T3.Calculate(close, 7, 0.7);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 5); //9702.65
            Assert.True(actualResult.Result[i3] > 9702 && actualResult.Result[i3] < 9703);

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 8); //9213.20
            Assert.True(actualResult.Result[i1] > 9213 && actualResult.Result[i1] < 9214);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 12);//51701.96
            Assert.True(actualResult.Result[i2] > 51701 && actualResult.Result[i2] < 51702);

            var i4 = dateTimes.FindIndex(x => x.Year == 2015 && x.Month == 10);//51701.96
            Assert.True(actualResult.Color[i4] == "red");
            Assert.True(actualResult.Color[i4 + 1] == "green");

            var i5 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 5);//51701.96
            Assert.True(actualResult.Color[i5] == "green");
            Assert.True(actualResult.Color[i5 + 1] == "red");
        }
    }
}
