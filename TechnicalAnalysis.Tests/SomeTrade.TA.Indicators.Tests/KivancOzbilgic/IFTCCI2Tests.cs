namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class IFTCCI2Tests
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

            var cciLength = 5;
            var wmaLength = 9;

            //TODO: CCI sonucu kontrol edilmeli

            // Act
            var actualResult = IFTCCI2.Calculate(close, high, low, cciLength, wmaLength);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //0.81
            Assert.True(actualResult[i3] > 0.80 && actualResult[i3] < 0.81);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1.00
            Assert.True(actualResult[i1] > 0.99 && actualResult[i1] < 1);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//0.20
            Assert.True(actualResult[i2] > 0.20 && actualResult[i2] < 0.21);
        }
    }
}
