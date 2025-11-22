namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class TEMATests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = TEMA.Calculate(close, 9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 5); //506.90
            Assert.True(actualResult[i3] > 506 && actualResult[i3] < 507);

            var i1 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 8); //10021.72
            Assert.True(actualResult[i1] > 10021 && actualResult[i1] < 10022);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 3);//51439.17
            Assert.True(actualResult[i2] > 51439 && actualResult[i2] < 51440);
        }
    }
}
