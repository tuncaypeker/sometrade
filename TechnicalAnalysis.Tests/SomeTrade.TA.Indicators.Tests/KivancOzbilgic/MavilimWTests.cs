namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class MavilimWTests
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
            var actualResult = MavilimW.Calculate(close, 3, 5);

            // Assert
            var i2 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 2);
            Assert.True(actualResult.Result[i2] > 606 && actualResult.Result[i2] < 607);

            var i3 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 3);
            Assert.True(actualResult.Result[i3] > 677 && actualResult.Result[i3] < 678);

            var i4 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 1);
            Assert.True(actualResult.Result[i4] > 6871 && actualResult.Result[i4] < 6872);
        }
    }
}
