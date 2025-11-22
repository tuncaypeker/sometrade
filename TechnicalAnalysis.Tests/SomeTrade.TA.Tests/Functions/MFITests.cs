using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class MFITests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] open = candles.Select(x => x.Open).ToArray();
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = MFI.Execute(high, low, close, volume, 14);

             var i3 = dateTimes.FindIndex(x=>x.Year == 2016 && x.Month == 4); //68.41
            Assert.True(actualResult[i3] > 68 && actualResult[i3] < 69);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 12); //40.42
            Assert.True(actualResult[i1] > 40 && actualResult[i1] < 41);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2020 && x.Month == 8);//61.05
            Assert.True(actualResult[i2] > 61 && actualResult[i2] < 62);
        }
    }
}
