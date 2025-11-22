using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class SARTests
    {
        [Fact]
        public void Execute_020202()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = SAR.Calculate(close, high, low, start: 0.02, inc: 0.02, max: 0.2);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //716.2
            Assert.True(actualResult[i3] > 86 && actualResult[i3] < 87);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1222.37
            Assert.True(actualResult[i1] > 418 && actualResult[i1] < 419);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//13482.81
            Assert.True(actualResult[i2] > 3228 && actualResult[i2] < 3229);
        }
    }
}
