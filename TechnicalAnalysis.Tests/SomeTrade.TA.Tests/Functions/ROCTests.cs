using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class ROCTests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ROC.Execute(close, 9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 7); //101.12
            Assert.True(actualResult[i3] > 101 && actualResult[i3] < 102);

            var i1 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 12); //-46.70
            Assert.True(actualResult[i1] > -47 && actualResult[i1] < -46);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 12);//351.52
            Assert.True(actualResult[i2] > 351 && actualResult[i2] < 352);
        }
    }
}
