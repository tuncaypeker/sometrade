using System;
using Xunit;

namespace SomeTrade.Strategies.Tests
{
    public class MARibbonStrategyTests
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void SortShortToLongTrueTest()
        {
            double[] prevs = new double[] { 1.9456, 1.7245, 1.8464 }; //onceki ma'larda prev daha buyuk
            double[] lasts = new double[] { 1.9456, 1.8464, 1.7245 }; //guncel ma'larda olmasi gerektigi gibi siralanmis

            var result = new MARibbonStrategy(new System.Collections.Generic.List<Dto.Candle>(), new System.Collections.Generic.Dictionary<string, object>())
                .SortShortToLong(lasts[0], lasts[1], lasts[2], prevs[0], prevs[1], prevs[2]);

            Assert.True(result);
        }

        [Fact]
        public void SortShortToLongFalseTest()
        {
            double[] prevs = new double[] { 1.9456, 1.7245, 1.8464 }; //onceki ma'larda prev daha buyuk
            double[] lasts = new double[] { 1.9456, 1.9564, 1.7245 }; //ilk'de dogru sirali degil, ikincisi de

            var result = new MARibbonStrategy(new System.Collections.Generic.List<Dto.Candle>(), new System.Collections.Generic.Dictionary<string, object>())
                .SortShortToLong(lasts[0], lasts[1], lasts[2], prevs[0], prevs[1], prevs[2]);

            Assert.False(result);
        }

        [Fact]
        public void SortLongToShortTrueTest()
        {
            double[] prevs = new double[] { 1.8464, 1.7245, 1.9456 }; //onceki ma'larda prev daha kucuk
            double[] lasts = new double[] { 1.7245, 1.8464, 1.9456 }; //guncel ma'larda olmasi gerektigi gibi siralanmis

            var result = new MARibbonStrategy(new System.Collections.Generic.List<Dto.Candle>(), new System.Collections.Generic.Dictionary<string, object>())
                .SortLongToShort(lasts[0], lasts[1], lasts[2], prevs[0], prevs[1], prevs[2]);

            Assert.True(result);
        }

         [Fact]
        public void SortLongToShortFalseTest()
        {
            double[] prevs = new double[] { 1.8464, 1.7245, 1.9456 }; //onceki ma'larda prev daha kucuk
            double[] lasts = new double[] { 1.7245, 1.9564, 1.9456 }; //middle long'dan buyuk siralama hatali

            var result = new MARibbonStrategy(new System.Collections.Generic.List<Dto.Candle>(), new System.Collections.Generic.Dictionary<string, object>())
                .SortLongToShort(lasts[0], lasts[1], lasts[2], prevs[0], prevs[1], prevs[2]);

            Assert.False(result);
        }
    }
}
