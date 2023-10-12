using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class RSITests
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
            var actualResult = RSI.Calculate(close, 14);

             // Assert
            var i3 = dateTimes.FindIndex(x=>x.Year == 2015 && x.Month == 3); //45.81
            Assert.True(actualResult[i3] > 45 && actualResult[i3] < 46);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2017 && x.Month == 12); //96.01
            Assert.True(actualResult[i1] > 96 && actualResult[i1] < 97);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2019 && x.Month == 8);//61.30
            Assert.True(actualResult[i2] > 61 && actualResult[i2] < 62);
        }
    }
}
