namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class DmiPlusTests
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
            var actualResult = DmiPlus.Calculate(close, 14, high, low);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //28.31
            Assert.True(actualResult[i3] > 28 && actualResult[i3] < 29);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //40.07
            Assert.True(actualResult[i1] > 40 && actualResult[i1] < 41);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//20.92
            Assert.True(actualResult[i2] > 20 && actualResult[i2] < 21);
        }
    }
}
