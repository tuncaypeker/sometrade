namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class BOPTests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] open = candles.Select(x => x.Open).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = BOP.Calculate(open, high, close, low);

            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 4); //0.37
            Assert.True(actualResult[i3] > 0.36 && actualResult[i3] < 0.37);

            var i1 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 12); //-0.24
            Assert.True(actualResult[i1] > -0.25 && actualResult[i1] < -0.24);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 8);//0.16
            Assert.True(actualResult[i2] > 0.15 && actualResult[i2] < 0.16);
        }
    }
}
