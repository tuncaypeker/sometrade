namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class DarvasBoxKTests
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
            var actualResult = DarvasBox.Calculate(high, low, 5);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2);
            Assert.True(actualResult.BottomBox[i3] == 235);
            Assert.True(actualResult.TopBox[i3] == 502);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //778.85, 435
            Assert.True(actualResult.BottomBox[i1] == 435);
            Assert.True(actualResult.TopBox[i1] > 778 && actualResult.TopBox[i1] < 779);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//13380 , 5263.03
            Assert.True(actualResult.BottomBox[i2] > 5263 && actualResult.BottomBox[i2] < 5264);
            Assert.True(actualResult.TopBox[i2] == 13880.00);
        }
    }
}
