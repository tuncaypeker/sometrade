namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    using SomeTrade.TA.Indicators.KivancOzbilgic;
    using SomeTrade.TA.Tests.Utilities;
    using SomeTrade.TA.Tests.Utilities._DataProviders;
    using System.Linq;
    using Xunit;

    public class IFTSTOCHTests
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

            var stochLength = 5;
            var wmaLength = 9;

            // Act
            var actualResult = IFTSTOCH.Calculate(close, high, low, stochLength, wmaLength);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //0.84
            Assert.True(actualResult[i3] > 0.83 && actualResult[i3] < 0.84);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //1.00
            Assert.True(actualResult[i1] > 0.99 && actualResult[i1] < 1);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//0.36
            Assert.True(actualResult[i2] > 0.36 && actualResult[i2] < 0.37);
        }

        [Fact]
        public void Execute_4H_Binance_BTC()
        {
            // Arrange
            var candles = _Data_BINANCE_BTC_USDT_4H.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            var stochLength = 5;
            var wmaLength = 9;

            // Act
            var actualResult = IFTSTOCH.Calculate(close, high, low, stochLength, wmaLength);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 20 && x.Hour == 18); //0.81
            Assert.True(actualResult[i3] > 0.80 && actualResult[i3] < 0.81);

            var i1 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 25 && x.Hour == 10); //-0,74
            Assert.True(actualResult[i1] > -0.75 && actualResult[i1] < -0.74);

            var i2 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 30 && x.Hour == 10);//0,91
            Assert.True(actualResult[i2] > 0.91 && actualResult[i2] < 92);
        }
    }
}
