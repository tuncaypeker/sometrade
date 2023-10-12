namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class ATRTests
    {
        [Fact]
        public void Execute_SMA14()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ATR.Calculate(close, high, low, 14, maType: "sma");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //86.50
            Assert.True(actualResult[i3] > 86 && actualResult[i3] < 87);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //418.05
            Assert.True(actualResult[i1] > 418 && actualResult[i1] < 419);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//3228.42
            Assert.True(actualResult[i2] > 3228 && actualResult[i2] < 3229);
        }

        [Fact]
        public void Execute_EMA14()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ATR.Calculate(close, high, low, 14, maType: "ema");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //105.99
            Assert.True(actualResult[i3] > 105 && actualResult[i3] < 106);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //538.42
            Assert.True(actualResult[i1] > 538 && actualResult[i1] < 539);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//2917.20
            Assert.True(actualResult[i2] > 2917 && actualResult[i2] < 2918);
        }

        [Fact]
        public void Execute_WMA14()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ATR.Calculate(close, high, low, 14, maType: "wma");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //93.27
            Assert.True(actualResult[i3] > 93 && actualResult[i3] < 94);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //584.29
            Assert.True(actualResult[i1] > 584 && actualResult[i1] < 585);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//3078.13
            Assert.True(actualResult[i2] > 3078 && actualResult[i2] < 3079);
        }

        [Fact]
        public void Execute_RMA14()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ATR.Calculate(close, high, low, 14, maType: "rma");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //116.43
            Assert.True(actualResult[i3] > 116 && actualResult[i3] < 117);

            var i1 = dateTimes.FindIndex(x => x.Year == 2017 && x.Month == 7); //371.80
            Assert.True(actualResult[i1] > 371 && actualResult[i1] < 372);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5);//2742.34
            Assert.True(actualResult[i2] > 2742 && actualResult[i2] < 2743);
        }

        [Fact]
        public void Execute_DB_RMA14()
        {
            // Arrange
            var candles = _Data_DATABASE_BTC_USDT_1D.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = ATR.Calculate(close, high, low, 14, maType: "rma");

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5 && x.Day == 10); //541.6206999013115
            var resulti3 = actualResult[i3];
            Assert.True(resulti3 > 541 && resulti3 < 542);

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5 && x.Day == 11); //572.0763641940749
            var resulti1 = actualResult[i1];
            Assert.True(resulti1 > 572 && resulti1 < 573);

            var i2 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 5 && x.Day == 12);//563.3194810373553
            var resulti2 = actualResult[i2];
            Assert.True(resulti2 > 563 && resulti2 < 564);
        }
    }
}
