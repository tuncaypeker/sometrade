namespace SomeTrade.TA.Indicators.Tests
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ATRPTests
    {
        /// <summary>
        /// Trading View'de RMA ile alýyor, bizimki SMA ile 
        /// o yuzden tutarsiz
        /// </summary>
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
            var actualResult = ATRP.Calculate(close, high, low, 14);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //24.35
            Assert.True(actualResult[i3] > 24 && actualResult[i3] < 25);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //18.85
            Assert.True(actualResult[i1] > 18 && actualResult[i1] < 19);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//30.88
            Assert.True(actualResult[i2] > 30 && actualResult[i2] < 31);
        }
    }
}
