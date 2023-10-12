using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class SMATests
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
            var actualResult = SMA.Calculate(close, 7);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //341.08
            Assert.True(actualResult[i3] > 341 && actualResult[i3] < 342);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1742.15
            Assert.True(actualResult[i1] > 1742 && actualResult[i1] < 1743);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//8152.75
            Assert.True(actualResult[i2] > 8152 && actualResult[i2] < 8153);
        }
    }
}
