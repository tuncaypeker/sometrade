namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class WPRTests
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
            var actualResult = WPR.Calculate(close, 14, high, low, close);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //-19.10
            Assert.True(actualResult[i3] > -20 && actualResult[i3] < -19);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //-4.94
            Assert.True(actualResult[i1] > -5 && actualResult[i1] < -4);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//-44.21
            Assert.True(actualResult[i2] > -45 && actualResult[i2] < -44);
        }
    }
}
