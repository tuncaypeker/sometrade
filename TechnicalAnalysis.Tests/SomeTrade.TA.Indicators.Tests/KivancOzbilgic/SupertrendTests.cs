namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class SupertrendTests
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
            var actualResult = Supertrend.Calculate(close, close, high, low, 10);

            // Assert
            var i1 = dateTimes.FindIndex(x => x.Year == 2015 && x.Month == 1);
            Assert.True(actualResult[i1] == -1);

            var i2 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 6);
            Assert.True(actualResult[i2] == 1);

            var i3 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 3);
            Assert.True(actualResult[i3] == -1);

            var i4 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 6);
            Assert.True(actualResult[i4] == 1);
        }
    }
}
