namespace SomeTrade.TA
{
    public class Fibonacci
    {
        /// <summary>
        /// https://www.udemy.com/course/tradingview-pinescript-v5/learn/lecture/32488368?start=75#content
        /// 
        /// pinescript:
        ///     //@version 5
        ///     indicator("otomatik fibonacci", overlay= true)
        ///     fiboLookback = input.int(250, "LOOKBACK"),
        ///     
        ///     tepe = ta.highest(fibonacciLookback)
        ///     dip = ta.lowest(fibonacciLookback)
        ///     
        ///     mutlakDeger(sayi) => 
        ///         math.abs(sayi)
        /// 
        ///     //ikisine olan uzaklıgi bulmaliyim, negatif değer olarak verir asagidaki fonksiyon
        ///     tepeyeUzaklik = mutlakDeger(ta.highestbars(fibonacciLookback))
        ///     dibeUzaklik = mutlakDeger(ta.lowestbars(fibonacciLookback))
        ///     
        ///     //dip daha uzaksa once dipten hesaplamam lazim
        ///     //1- tepeden dibi cıkar farkın 618 ini al, sonra bunu tepeden cıkar
        ///     fib618 = dibeUzaklik > tepeyeUzaklik ? tepe - ((tepe-dip) * 618 / 1000) : dip + ((tepe-dip) * 618 / 1000)
        /// 
        ///     plot(fib618)
        /// 
        /// </summary>
        /// <param name="candles"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Calculate(double[] candles, double value)
        {
            return 0;
        }
    }
}
