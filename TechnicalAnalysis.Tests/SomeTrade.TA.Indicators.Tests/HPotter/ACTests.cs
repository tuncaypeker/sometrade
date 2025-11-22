namespace SomeTrade.TA.Indicators.Tests.HPotter
{
    using SomeTrade.TA.Indicators.HPotter;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ACTests
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
            var actualResult = AC.Calculate(high, low, 5, 34);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //44.73
            Assert.True(actualResult[i3] > 44 && actualResult[i3] < 45);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //416.14
            Assert.True(actualResult[i1] > 416 && actualResult[i1] < 417);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//-239.67
            Assert.True(actualResult[i2] > -240 && actualResult[i2] < -239);
        }
    }
}
