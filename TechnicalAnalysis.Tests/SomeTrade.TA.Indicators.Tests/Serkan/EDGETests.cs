namespace SomeTrade.TA.Indicators.Tests.Serkan
{
    using System.Linq;
    using Xunit;
    using SomeTrade.TA.Tests.Utilities;
    using SomeTrade.TA.Indicators.Serkan;

    public class EDGETests
    {
        [Fact]
        public void Execute_Length10()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EDGE.Calculate(close, length: 10, multiplier: 3.0);

            // Assert

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//10974,
            Assert.True(actualResult.Hband[i1] > 10974 && actualResult.Hband[i1] < 10975);
            Assert.True(actualResult.Lband[i1] > 3456 && actualResult.Lband[i1] < 3457);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 6);//10954,
            Assert.True(actualResult.Hband[i2] > 10954 && actualResult.Hband[i2] < 10955);
            Assert.True(actualResult.Lband[i2] > 3476 && actualResult.Lband[i2] < 3477);

            var i3 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 7);//11356,
            Assert.True(actualResult.Hband[i3] > 11356 && actualResult.Hband[i3] < 11357);
            Assert.True(actualResult.Lband[i3] > 3802 && actualResult.Lband[i3] < 3803);
        }

        [Fact]
        public void Execute_Length15()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EDGE.Calculate(close, length: 15, multiplier: 3.0);

            // Assert

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//11102,
            Assert.True(actualResult.Hband[i1] > 11102 && actualResult.Hband[i1] < 11103);
            Assert.True(actualResult.Lband[i1] > 4184 && actualResult.Lband[i1] < 4185);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 6);//11113.99,
            Assert.True(actualResult.Hband[i2] > 11113 && actualResult.Hband[i2] < 11114);
            Assert.True(actualResult.Lband[i2] > 4172 && actualResult.Lband[i2] < 4173);

            var i3 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 7);//11356.74,
            Assert.True(actualResult.Hband[i3] > 11356 && actualResult.Hband[i3] < 11357);
            Assert.True(actualResult.Lband[i3] > 4342 && actualResult.Lband[i3] < 4343);
        }
    }
}
