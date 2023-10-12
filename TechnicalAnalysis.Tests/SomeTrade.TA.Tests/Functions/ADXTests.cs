namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ADXTests
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
            var actualResult = ADX.Calculate(close, 14, high, low);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //29.79
            Assert.True(actualResult[i3] > 29 && actualResult[i3] < 30);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //69.18
            Assert.True(actualResult[i1] > 69 && actualResult[i1] < 70);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//28.35
            Assert.True(actualResult[i2] > 28 && actualResult[i2] < 29);
        }
    }
}
