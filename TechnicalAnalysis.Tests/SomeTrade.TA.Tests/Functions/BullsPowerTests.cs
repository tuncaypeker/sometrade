namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class BullsPowerTests
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
            var actualResult = BullsPower.Calculate(close, 13, high);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //103.28
            Assert.True(actualResult[i3] > 103 && actualResult[i3] < 104);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1441.25
            Assert.True(actualResult[i1] > 1441 && actualResult[i1] < 1442);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//1983.99
            Assert.True(actualResult[i2] > 1983 && actualResult[i2] < 1984);
        }
    }
}
