using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class RMATests
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
            var actualResult = RMA.Calculate(close,15);

            // Assert
            var i3 = dateTimes.FindIndex(x=>x.Year == 2017 && x.Month == 5); //758.45
            Assert.True(actualResult[i3] > 758 && actualResult[i3] < 759);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 3); //4216.27
            Assert.True(actualResult[i1] > 4216 && actualResult[i1] < 4217);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2021 && x.Month == 4);//19870.84
            Assert.True(actualResult[i2] > 19870 && actualResult[i2] < 19871);
        }
    }
}
