namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using SomeTrade.TA.Tests.Utilities._DataProviders;
    using System.Linq;
    using Xunit;

    public class EMATests
    {
        [Fact]
        public void Execute_4hourly_Length9()
        {
            // Arrange
            var candles = _Data_BINANCE_BTC_USDT_4H.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EMA.Calculate(close, 9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 14 && x.Hour == 6); //23324,34
            Assert.True(actualResult[i3] > 23324 && actualResult[i3] < 23325);

            var i1 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 20 && x.Hour == 6); //27587.43
            Assert.True(actualResult[i1] > 27587 && actualResult[i1] < 27588);

            var i2 = dateTimes.FindIndex(x => x.Year == 2023 && x.Month == 3 && x.Day == 25 && x.Hour == 6);//27773.30
            Assert.True(actualResult[i2] > 27773 && actualResult[i2] < 27774);
        }

        [Fact]
        public void Execute_MainMonthlData_Length9()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EMA.Calculate(close, 9);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 6); //6702.75
            Assert.True(actualResult[i3] > 6702 && actualResult[i3] < 6703);

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 3); //7870.40
            Assert.True(actualResult[i1] > 7870 && actualResult[i1] < 7871);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 11);//46978.87
            Assert.True(actualResult[i2] > 46978 && actualResult[i2] < 46979);
        }

        [Fact]
        public void Execute_MainMonthlData_Length26()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EMA.Calculate(close, 26);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 6); //5716.34
            Assert.True(actualResult[i3] > 5716 && actualResult[i3] < 5717);

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 3); //7012.31
            Assert.True(actualResult[i1] > 7012 && actualResult[i1] < 7013);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 11);//32093.90
            Assert.True(actualResult[i2] > 32093 && actualResult[i2] < 32094);

            var i4 = dateTimes.FindIndex(x => x.Year == 2016 && x.Month == 2); //327.88
            Assert.True(actualResult[i4] > 327 && actualResult[i4] < 328);
        }

        [Fact]
        public void Execute_MainMonthlData_Length5()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EMA.Calculate(close, 5);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 6); //7525.45
            Assert.True(actualResult[i3] > 7525 && actualResult[i3] < 7526);

            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 3); //7781.08
            Assert.True(actualResult[i1] > 7781 && actualResult[i1] < 7782);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 11);//51838.12
            Assert.True(actualResult[i2] > 51838 && actualResult[i2] < 51839);
        }

        [Fact]
        public void Execute_MainMonthlData_Length100()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = EMA.Calculate(close, 100);

            // Assert
            var i1 = dateTimes.FindIndex(x => x.Year == 2020 && x.Month == 3); //2794.67
            Assert.True(actualResult[i1] > 2794 && actualResult[i1] < 2795);

            var i2 = dateTimes.FindIndex(x => x.Year == 2021 && x.Month == 11);//13070.12
            Assert.True(actualResult[i2] > 13070 && actualResult[i2] < 13071);
        }
    }
}
