namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class TRIXTests
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
            var actualResult = TRIX.Calculate(close, 18);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //1.07
            Assert.True(actualResult[i3] > 1 && actualResult[i3] < 2);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //6.23
            Assert.True(actualResult[i1] > 6 && actualResult[i1] < 7);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//2.42
            Assert.True(actualResult[i2] > 2 && actualResult[i2] < 3);
        }
    }
}
