namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ICHIMOKUKTests
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
            var actualResult = ICHIMOKUK.Calculate(low, high, 9, 26, 26, 52, 26);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //0.84
            Assert.True(actualResult.TenkanSen[i3] > 350 && actualResult.TenkanSen[i3] < 351);
            Assert.True(actualResult.KijunSen[i3] > 573 && actualResult.KijunSen[i3] < 574);
            Assert.True(actualResult.SenkouSpanA[i3] > 593 && actualResult.SenkouSpanA[i3] < 594);
            //Assert.True(actualResult.SenkouSpanB[i3] > 0.83 && actualResult.SenkouSpanB[i3] < 0.84);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1.00
            Assert.True(actualResult.TenkanSen[i1] > 1825 && actualResult.TenkanSen[i1] < 1826);
            Assert.True(actualResult.KijunSen[i1] > 1589 && actualResult.KijunSen[i1] < 1590);
            Assert.True(actualResult.SenkouSpanA[i1] > 464 && actualResult.SenkouSpanA[i1] < 465);
            //Assert.True(actualResult.SenkouSpanB[i1] > 0.83 && actualResult.SenkouSpanB[i1] < 0.84);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//0.36
            Assert.True(actualResult.TenkanSen[i2] > 7399 && actualResult.TenkanSen[i2] < 7400);
            Assert.True(actualResult.KijunSen[i2] > 8501 && actualResult.KijunSen[i2] < 8502);
            Assert.True(actualResult.SenkouSpanA[i2] > 10381 && actualResult.SenkouSpanA[i2] < 10382);
            Assert.True(actualResult.SenkouSpanB[i2] > 9909 && actualResult.SenkouSpanB[i2] < 9910);
        }
    }
}
