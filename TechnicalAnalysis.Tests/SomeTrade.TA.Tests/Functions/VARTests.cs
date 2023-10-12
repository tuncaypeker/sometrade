using SomeTrade.TA.Tests.Utilities;
using SomeTrade.TA.Tests.Utilities._DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SomeTrade.TA.Tests.Functions
{
    public class VARTests
    {
        [Fact]
        public void Execute_close30()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = VAR.Calculate(close,30);

            // Assert
            var i3 = dateTimes.FindIndex(x=>x.Year == 2017 && x.Month == 5); //1263.54
            Assert.True(actualResult[i3] > 591 && actualResult[i3] < 592);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 3); //8582.07
            Assert.True(actualResult[i1] > 3245 && actualResult[i1] < 3246);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2021 && x.Month == 4);//38098.85
            Assert.True(actualResult[i2] > 16628 && actualResult[i2] < 16629);
        }

        [Fact]
        public void Execute_TradingView_Export_1M()
        {
            var candles = TradingViewHelper.GetCandles("_DataProviders/TradingViewChart/VAR_Indicator_1M.csv");

            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();

            var dateTimes = candles.Select(x => x.Time).ToList();
            for (int i = 0; i < dateTimes.Count; i++)
                dateTimes[i] = dateTimes[i].AddHours(-3);

            // Act
            var actualResult = VAR.Calculate(close,40);

            // Assert
            var lastOne = actualResult.Last();
            Assert.True( lastOne > 2145 && lastOne < 2146);
        }

         [Fact]
        public void Execute_TradingView_Export_1H()
        {
            var candles = TradingViewHelper.GetCandles("_DataProviders/TradingViewChart/VAR_Indicator_1H.csv");

            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();

            var dateTimes = candles.Select(x => x.Time).ToList();
            for (int i = 0; i < dateTimes.Count; i++)
                dateTimes[i] = dateTimes[i].AddHours(-3);

            // Act
            var actualResult = VAR.Calculate(close,40);

            // Assert
            var lastOne = actualResult.Last();
            Assert.True( lastOne > 2145 && lastOne < 2146);
        }
    }
}
