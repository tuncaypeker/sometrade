namespace SomeTrade.TA.Indicators.Tests.KivancOzbilgic
{
    
    using SomeTrade.TA.Indicators.KivancOzbilgic.OTT;
    using SomeTrade.TA.Tests.Utilities;
    using SomeTrade.TA.Tests.Utilities._DataProviders;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class OTTTests
    {
        [Fact]
        public void Execute_40_1_SMA()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = OTT.Calculate(close, 40, 1, "SMA");

            // Assert
            var i2 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 2);
            Assert.True(actualResult.OTTLine[i2] > 1575 && actualResult.OTTLine[i2] < 1576);
            Assert.True(actualResult.SupportLine[i2] > 2076 && actualResult.SupportLine[i2] < 2077);

            var i3 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 3);
            Assert.True(actualResult.OTTLine[i3] > 1818 && actualResult.OTTLine[i3] < 1819);
            Assert.True(actualResult.SupportLine[i3] > 2240 && actualResult.SupportLine[i3] < 2241);

            var i4 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 1);
            Assert.True(actualResult.OTTLine[i4] > 8249 && actualResult.OTTLine[i4] < 8250);
            Assert.True(actualResult.SupportLine[i4] > 9618 && actualResult.SupportLine[i4] < 9619);
        }

        [Fact]
        public void Execute_40_1_EMA()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = OTT.Calculate(close, 40, 1, "EMA");

            // Assert
            var i2 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 2);
            Assert.True(actualResult.OTTLine[i2] > 2425 && actualResult.OTTLine[i2] < 2426);
            Assert.True(actualResult.SupportLine[i2] > 3179 && actualResult.SupportLine[i2] < 3180);

            var i3 = dateTimes.FindIndex(x => x.Year == 2018 && x.Month == 3);
            Assert.True(actualResult.OTTLine[i3] > 2799 && actualResult.OTTLine[i3] < 2800);
            Assert.True(actualResult.SupportLine[i3] > 3362 && actualResult.SupportLine[i3] < 3363);

            var i4 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 1);
            Assert.True(actualResult.OTTLine[i4] > 8084 && actualResult.OTTLine[i4] < 8085);
            Assert.True(actualResult.SupportLine[i4] > 10313 && actualResult.SupportLine[i4] < 10314);
        }
    }
}
