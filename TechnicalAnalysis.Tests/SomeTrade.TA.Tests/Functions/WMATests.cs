using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class WMATests
    {
        [Fact]
        public void Execute_close10()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = WMA.Calculate(close,10);

            // Assert
            var i3 = dateTimes.FindIndex(x=>x.Year == 2017 && x.Month == 5); //1263.54
            Assert.True(actualResult[i3] > 1263 && actualResult[i3] < 1264);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 3); //8582.07
            Assert.True(actualResult[i1] > 8582 && actualResult[i1] < 8583);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2021 && x.Month == 4);//38098.85
            Assert.True(actualResult[i2] > 38098 && actualResult[i2] < 38099);
        }
    }
}
