using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class COGTests
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
            var actualResult = COG.Calculate(close,9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //-4.49
            Assert.True(actualResult[i3] > -5 && actualResult[i3] < -4);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //-3.87
            Assert.True(actualResult[i1] > -4 && actualResult[i1] < -3);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//-4.97
            Assert.True(actualResult[i2] > -5 && actualResult[i2] < -4);
        }
    }
}
