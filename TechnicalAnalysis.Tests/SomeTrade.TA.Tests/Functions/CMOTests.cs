using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class CMOTests
    {
        [Fact]
        public void Execute_9_Close()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] open = candles.Select(x => x.Open).ToArray();
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = CMO.Calculate(close, 9);

           var i3 = dateTimes.FindIndex(x=>x.Year == 2016 && x.Month == 4); //37.37
            Assert.True(actualResult[i3] > 37 && actualResult[i3] < 38);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 12); //-30.69
            Assert.True(actualResult[i1] > -31 && actualResult[i1] < -30);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2020 && x.Month == 8);//36.29
            Assert.True(actualResult[i2] > 36 && actualResult[i2] < 37);
        }
    }
}
