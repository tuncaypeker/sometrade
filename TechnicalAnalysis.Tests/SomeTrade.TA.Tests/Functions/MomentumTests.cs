using SomeTrade.TA.Tests.Utilities;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class MomentumTests
    {
        [Fact]
        public void Calculate()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = Momentum.Calculate(close, 10);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //184.26
            Assert.True(actualResult[i3] > 184 && actualResult[i3] < 185);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //469.53
            Assert.True(actualResult[i1] > 469 && actualResult[i1] < 470);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//93.66
            Assert.True(actualResult[i2] > 93 && actualResult[i2] < 94);
        }

        [Fact]
        public void Calculate2()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = Momentum.Calculate2(close, 10);

           // Assert
             var i3 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 11); //-6177.94
            Assert.True(actualResult[i3] > -6178 && actualResult[i3] < 6177);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2020 && x.Month == 12); //20465.05
            Assert.True(actualResult[i1] > 20465 && actualResult[i1] < 20466);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2021 && x.Month == 8);//33340.01
            Assert.True(actualResult[i2] > 33340 && actualResult[i2] < 33341);
        }
    }
}
