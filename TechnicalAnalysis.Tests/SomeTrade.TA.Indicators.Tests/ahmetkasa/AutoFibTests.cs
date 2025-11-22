namespace SomeTrade.TA.Indicators.Tests.ahmetkasa
{
    using SomeTrade.TA.Indicators.ahmetkasa;
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    public class AutoFibTests
    {
        [Fact]
        public void Execute_20()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            double[] volume = candles.Select(x => x.Volume).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = AutoFib.Execute(close, 20);

            // Assert
            var i3 = dateTimes.FindIndex(x => x.Year == 2019 && x.Month == 2); 
            Assert.True(actualResult.Maxr[i3] > 13799 && actualResult.Maxr[i3] < 13881); //13880
            Assert.True(actualResult.Ss[i3] > 11278 && actualResult.Ss[i3] < 11279);     //11278
            Assert.True(actualResult.So[i3] > 9668 && actualResult.So[i3] < 9669);     //9669
            Assert.True(actualResult.Fi[i3] > 8367 && actualResult.Fi[i3] < 8368);     //8368
            Assert.True(actualResult.Te[i3] > 7067 && actualResult.Te[i3] < 7068);     //7067
            Assert.True(actualResult.Tt[i3] > 5457 && actualResult.Tt[i3] < 5458);     //5458
            Assert.True(actualResult.Minr[i3] > 2855 && actualResult.Minr[i3] < 2856); //2856
        }
    }
}
