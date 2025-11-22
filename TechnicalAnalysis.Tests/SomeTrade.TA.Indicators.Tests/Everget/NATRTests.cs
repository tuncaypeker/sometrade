namespace SomeTrade.TA.Indicators.Tests.Everget
{
    using SomeTrade.TA.Indicators.Everget;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class NATRTests
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
            var actualResult = NATR.Calculate(close, high, low, 14, "rma");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //26.75
            Assert.True(actualResult[i3] > 26 && actualResult[i3] < 27);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //13.02
            Assert.True(actualResult[i1] > 13 && actualResult[i1] < 14);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//29.03
            Assert.True(actualResult[i2] > 29 && actualResult[i2] < 30);
        }
    }
}
