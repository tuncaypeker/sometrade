namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class MACDTests
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
            var actualResult = MACD.Calculate(close, 12, 26, 9, "ema");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //
            Assert.True(actualResult.MACDHistory[i3] > 7 && actualResult.MACDHistory[i3] < 8);
            Assert.True(actualResult.MACD[i3] > 18 && actualResult.MACD[i3] < 19);
            Assert.True(actualResult.MACDSignal[i3] > 10 && actualResult.MACDSignal[i3] < 11);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //
            Assert.True(actualResult.MACDHistory[i1] > 216 && actualResult.MACDHistory[i1] < 217);
            Assert.True(actualResult.MACD[i1] > 488 && actualResult.MACD[i1] < 489);
            Assert.True(actualResult.MACDSignal[i1] > 271 && actualResult.MACDSignal[i1] < 272);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//1983.99
            Assert.True(actualResult.MACDHistory[i2] > -42 && actualResult.MACDHistory[i2] < -41);
            Assert.True(actualResult.MACD[i2] > 841 && actualResult.MACD[i2] < 842);
            Assert.True(actualResult.MACDSignal[i2] > 883 && actualResult.MACDSignal[i2] < 884);
        }
    }
}
