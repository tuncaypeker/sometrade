namespace SomeTrade.TA.Indicators.Tests.Everget
{
    using SomeTrade.TA.Indicators.Everget;
    using SomeTrade.TA.Tests.Utilities._DataProviders;
    using System.Linq;
    using Xunit;

    public class AHMATests
    {
        [Fact]
        public void Execute_MainMonthlData_Length9()
        {
            // Arrange
            var candles = _Data_BINANCE_BTC_USDT_4H.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = AHMA.Calculate(close, 9);

            // Assert
            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 3); //7870.40
            Assert.True(actualResult[i1] > 7870 && actualResult[i1] < 7871);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 7);//46978.87
            Assert.True(actualResult[i2] > 46978 && actualResult[i2] < 46979);

            var i3 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 1 && x.Day == 1); //6702.75
            Assert.True(actualResult[i3] > 6702 && actualResult[i3] < 6703);
        }
    }
}
