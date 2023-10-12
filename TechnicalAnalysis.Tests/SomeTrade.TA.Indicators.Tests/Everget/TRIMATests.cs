namespace SomeTrade.TA.Indicators.Tests.Everget
{
    using SomeTrade.TA.Indicators.Everget;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class TRIMATests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = TRIMA.Calculate(close, 10);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 10); //409.58
            Assert.True(actualResult[i3] > 409 && actualResult[i3] < 410);

            var i1 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 10); //7867.18
            Assert.True(actualResult[i1] > 7867 && actualResult[i1] < 7868);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 1);//6681.36
            Assert.True(actualResult[i2] > 6681 && actualResult[i2] < 6682);
        }
    }
}
