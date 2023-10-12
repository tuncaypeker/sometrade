namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class OBVTests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = OBV.Execute(close, volume);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 7); //1513000
            Assert.True(actualResult[i3] > 1512000 && actualResult[i3] < 1513000);

            var i1 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 12); //2717000
            Assert.True(actualResult[i1] > 2716000 && actualResult[i1] < 2717000);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 12);//3933000
            Assert.True(actualResult[i2] > 3933000 && actualResult[i2] < 3934000);
        }
    }
}
