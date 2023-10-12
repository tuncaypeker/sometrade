namespace SomeTrade.TA.Tests.Functions
{
    using SomeTrade.TA.Tests.Utilities;
    using System.Linq;
    using Xunit;

    /*
        //@version=5
        indicator(title="Commodity Channel Index", shorttitle="CCI", format=format.price, precision=2, timeframe="", timeframe_gaps=true)
        length = input.int(20, minval=1)
        src = input(hlc3, title="Source")
        ma = ta.sma(src, length)
        cci = (src - ma) / (0.015 * ta.dev(src, length))
        plot(cci, "CCI", color=#2962FF)
        band1 = hline(100, "Upper Band", color=#787B86, linestyle=hline.style_dashed)
        hline(0, "Middle Band", color=color.new(#787B86, 50))
        band0 = hline(-100, "Lower Band", color=#787B86, linestyle=hline.style_dashed)
        fill(band1, band0, color=color.rgb(33, 150, 243, 90), title="Background")

        ma(source, length, type) =>
            switch type
                "SMA" => ta.sma(source, length)
                "EMA" => ta.ema(source, length)
                "SMMA (RMA)" => ta.rma(source, length)
                "WMA" => ta.wma(source, length)
                "VWMA" => ta.vwma(source, length)

        typeMA = input.string(title = "Method", defval = "SMA", options=["SMA", "EMA", "SMMA (RMA)", "WMA", "VWMA"], group="Smoothing")
        smoothingLength = input.int(title = "Length", defval = 5, minval = 1, maxval = 100, group="Smoothing")

        smoothingLine = ma(cci, smoothingLength, typeMA)
        plot(smoothingLine, title="Smoothing Line", color=#f37f20, display=display.none)

    !!!!!!!!!!!!!!!!!!!!!!!! TradingView/Commodity Channel Index/_data içinde data'sı var
    !!!!!!!!!!!!!!!!!!!!!111 pine script altında ma seçtiriyor bizimkinde yok
     */ 

    public class CCITests
    {
        [Fact]
        public void Execute()
        {
            // Arrange
            var candles = _Data_BITSTAMP_BTC_USD_1Mo.Data;
            double[] open = candles.Select(x => x.Open).ToArray();
            double[] high = candles.Select(x => x.High).ToArray();
            double[] low = candles.Select(x => x.Low).ToArray();
            double[] close = candles.Select(x => x.Close).ToArray();
            var dateTimes = candles.Select(x => x.Time).ToList();

            // Act
            var actualResult = CCI.Calculate(high, low, close, 20);

           var i3 = dateTimes.FindIndex(x=>x.Year == 2016 && x.Month == 4); //101.85
            Assert.True(actualResult[i3] > 101 && actualResult[i3] < 102);

            var i1 = dateTimes.FindIndex(x=>x.Year == 2018 && x.Month == 12); //-81.64
            Assert.True(actualResult[i1] > -82 && actualResult[i1] < -81);

            var i2 = dateTimes.FindIndex(x=>x.Year == 2020 && x.Month == 8);//119.78
            Assert.True(actualResult[i2] > 119 && actualResult[i2] < 120);
        }
    }
}
