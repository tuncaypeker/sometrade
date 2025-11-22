using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class HMATests
    {
        [Fact]
        public void Execute_close9()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = HMA.Calculate(close,9);

            // Assert
            var i3 = dateTimes.FindIndex(x=>x.Year == 2017 && x.Month == 5); //1657.65
            Assert.True(actualResult[i3] > 1657 && actualResult[i3] < 1658);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 3); //11410.29
            Assert.True(actualResult[i1] > 11410 && actualResult[i1] < 11411);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2021 && x.Month == 4);//60487.09 
            Assert.True(actualResult[i2] > 60487 && actualResult[i2] < 60488);
        }
    }
}
