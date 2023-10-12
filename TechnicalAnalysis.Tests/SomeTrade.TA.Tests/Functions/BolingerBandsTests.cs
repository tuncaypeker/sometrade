namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class BolingerBandsTests
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
            var actualResult = BolingerBands.Calculate(close, 20);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //
            Assert.True(actualResult.LowerBand[i3] > 136 && actualResult.LowerBand[i3] < 137);
            Assert.True(actualResult.MiddleBand[i3] > 330 && actualResult.MiddleBand[i3] < 331);
            Assert.True(actualResult.UpperBand[i3] > 523 && actualResult.UpperBand[i3] < 524);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //
            Assert.True(actualResult.LowerBand[i1] > -436 && actualResult.LowerBand[i1] < -435);
            Assert.True(actualResult.MiddleBand[i1] > 985 && actualResult.MiddleBand[i1] < 986);
            Assert.True(actualResult.UpperBand[i1] > 2405 && actualResult.UpperBand[i1] < 2406);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//1983.99
            Assert.True(actualResult.LowerBand[i2] > 2500 && actualResult.LowerBand[i2] < 2501);
            Assert.True(actualResult.MiddleBand[i2] > 7202 && actualResult.MiddleBand[i2] < 7203);
            Assert.True(actualResult.UpperBand[i2] > 11903 && actualResult.UpperBand[i2] < 11904);
        }
    }
}
