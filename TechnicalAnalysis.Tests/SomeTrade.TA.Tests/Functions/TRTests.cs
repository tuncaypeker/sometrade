using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class TRTests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = TR.Execute(high, low, close);

             // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2014 && x.Month == 4); //208.21
            Assert.True(actualResult[i3] > 208 && actualResult[i3] < 209);

            var i1 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //82.99
            Assert.True(actualResult[i1] > 82 && actualResult[i1] < 83);

            var i2 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 5);//2922.08
            Assert.True(actualResult[i2] > 2922 && actualResult[i2] < 2923);
        }
    }
}
