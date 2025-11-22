namespace SomeTrade.TA.Indicators.Tests
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ADLTests
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

            // Act
            var actualResult = ADL.Calculate(high, low, close, volume);

            // Assert
            Assert.Equal(actualResult[0], -7);
            Assert.True(actualResult[1] < -1119 && actualResult[1] > -1120);
            Assert.True(actualResult[10] > 67595 && actualResult[10] < 67596);
            Assert.True(actualResult[17] > 308697 && actualResult[17] < 308698);
        }
    }
}
