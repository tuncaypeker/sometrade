using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class IIRTests
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
            var actualResult = IIR.Calculate(close);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //427.26
            Assert.True(actualResult[i3] > 427 && actualResult[i3] < 428);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //2320.98
            Assert.True(actualResult[i1] > 2320 && actualResult[i1] < 2321);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//7987.99
            Assert.True(actualResult[i2] > 7987 && actualResult[i2] < 7988);
        }
    }
}
