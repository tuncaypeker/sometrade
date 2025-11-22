namespace SomeTrade.TA.Indicators.Tests
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class LWMATests
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
            var actualResult = LWMA.Calculate(close, 10);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2015 && x.Month == 3); //349.99
            Assert.True(actualResult[i3] > 349 && actualResult[i3] < 350);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 12); //4930.36
            Assert.True(actualResult[i1] > 4930 && actualResult[i1] < 4931);

            var i2 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 8);//7008.47
            Assert.True(actualResult[i2] > 7008 && actualResult[i2] < 7009);
        }
    }
}
