using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class HighestTests
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
            var actualResult = Highest.Calculate(close, 40);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //1119.80
            Assert.True(actualResult[i3] > 1119 && actualResult[i3] < 1120);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //2855.81
            Assert.True(actualResult[i1] > 2855 && actualResult[i1] < 2856);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//13880.00
            Assert.True(actualResult[i2] >= 13880 && actualResult[i2] < 13881);
        }
    }
}
