namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class BearsPowerTests
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
            var actualResult = BearsPower.Calculate(close, 13, low);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //20.29
            Assert.True(actualResult[i3] > 20 && actualResult[i3] < 21);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //333.25
            Assert.True(actualResult[i1] > 333 && actualResult[i1] < 334);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//18.99
            Assert.True(actualResult[i2] > 18 && actualResult[i2] < 19);
        }
    }
}
