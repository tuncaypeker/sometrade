namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ALPHATRENDTests
    {
        [Fact]
        public void Execute_14_1_with_volume()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ALPHATREND.Execute(close, high, low, volume, 14, 1, true);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 2); //7008.
            Assert.True(actualResult.AlphaTrend[i3] > 7008 && actualResult.AlphaTrend[i3] < 7009);
            Assert.True(actualResult.AlphaTrendK2[i3] > 7386 && actualResult.AlphaTrendK2[i3] < 7387);
            Assert.True(actualResult.BuySignal[i3] == 0);
            Assert.True(actualResult.SellSignal[i3] == 1);
        }

        [Fact]
        public void Execute_12_1dot2_with_volume()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ALPHATREND.Execute(close, high, low, volume, 12, 1.2, true);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 3); //7008.
            Assert.True(actualResult.AlphaTrend[i3] > 6221 && actualResult.AlphaTrend[i3] < 6222);
            Assert.True(actualResult.AlphaTrendK2[i3] > 6626 && actualResult.AlphaTrendK2[i3] < 6627);
            Assert.True(actualResult.BuySignal[i3] == 0);
            Assert.True(actualResult.SellSignal[i3] == 1);
        }
    }
}
