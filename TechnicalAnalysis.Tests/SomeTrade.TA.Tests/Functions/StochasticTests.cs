namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class StochasticTests
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
            var actualResult = Stochastic.Calculate(close, high, low, 14);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //80.90
            Assert.True(actualResult.K[i3] > 80 && actualResult.K[i3] < 81);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //95.06
            Assert.True(actualResult.K[i1] > 95 && actualResult.K[i1] < 96);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//55.79
            Assert.True(actualResult.K[i2] > 55 && actualResult.K[i2] < 56);

            var i4 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //74.12
            Assert.True(actualResult.D[i3] > 74 && actualResult.D[i3] < 75);

            var i5 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //85.15
            Assert.True(actualResult.D[i1] > 85 && actualResult.D[i1] < 86);

            var i6 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//44.55
            Assert.True(actualResult.D[i2] > 44 && actualResult.D[i2] < 45);
        }
    }
}
