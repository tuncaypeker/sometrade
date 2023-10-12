using SomeTrade.Strategies.Dto;
using SomeTrade.Strategies.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Strategies
{
    /// <summary>
    /// AL: 3 EMA küçükten büyüge sıralanırsa AL MA50 > MA100 > MA150
    /// SAT: 3 EMA büyükten küçüğe sıralanırsa SAT MA150 > MA100 > MA50
    /// </summary>
    public class MARibbonStrategy : StrategyBase
    {
        //parameters
        private int pShortMaLength;
        private int pMiddleMaLength;
        private int pLongMaLength;
        private string pMA;

        public MARibbonStrategy(List<Candle> candles, Dictionary<string, object> parameters)
            : base(candles, parameters, "MARibbonStrategy", 9)
        {
            pShortMaLength = GetParameterAsInt("ShortMaLength");
            pMiddleMaLength = GetParameterAsInt("MiddleMaLength");
            pLongMaLength = GetParameterAsInt("LongMaLength");
            pMA = GetParameter("MA");
        }

        public override CheckResult ShouldOpenLong()
        {
            //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
             var maMiddle = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
                : TA.SMA.Calculate(closeArray, pMiddleMaLength);
           var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            //# Variables
            var maShortLast = maShort.Last();
            var maShortPrev = maShort.TakePrev();
            var maMiddleLast = maMiddle.Last();
            var maMiddlePrev = maMiddle.TakePrev();
            var maLongLast = maLong.Last();
            var maLongPrev = maLong.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                  { "LONG Sarti 1", $"{maShortLast.RoundTo(5)} > {maMiddleLast.RoundTo(5)} > {maLongLast.RoundTo(5)}" },
                  { "LONG Sarti 2", $"!!{maShortPrev.RoundTo(5)} > {maMiddlePrev.RoundTo(5)} > {maLongPrev.RoundTo(5)}" },
                  { "LONG Sarti", $"{SortShortToLong(maShortLast, maMiddleLast, maLongLast, maShortPrev, maMiddlePrev, maLongPrev)}" },
                },
                Result = false
            };

            //# Decision
            //Daha once sirali degillerdi bu mumda siralandilar
            //short > middle > long olmamali oncesinde
            if (SortShortToLong(maShortLast, maMiddleLast, maLongLast, maShortPrev, maMiddlePrev, maLongPrev))
                checkResult.Result = true;

            return checkResult;
        }

        public override CheckResult ShouldOpenShort()
        {
            //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
             var maMiddle = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
                : TA.SMA.Calculate(closeArray, pMiddleMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            //# Variables
            var maShortLast = maShort.Last();
            var maShortPrev = maShort.TakePrev();
            var maMiddleLast = maMiddle.Last();
            var maMiddlePrev = maMiddle.TakePrev();
            var maLongLast = maLong.Last();
            var maLongPrev = maLong.TakePrev();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"MaShort[{pShortMaLength}]",$"[{maShortPrev.RoundTo(5)},{maShortLast.RoundTo(5)}]" },
                   { $"MaMiddle[{pMiddleMaLength}]",$"[{maMiddlePrev.RoundTo(5)},{maMiddleLast.RoundTo(5)}]" },
                   { $"MaLong[{pLongMaLength}]",$"[{maLongPrev.RoundTo(5)},{maLongLast.RoundTo(5)}]" },
                   { "ClosePrice",closeArray.Last() },
                },
                Result = false
            };

            //# Decision
            //Daha once sirali degillerdi bu mumda siralandilar
            //short < middle < long olmamali oncesinde
            if (SortLongToShort(maShortLast, maMiddleLast, maLongLast, maShortPrev, maMiddlePrev, maLongPrev) )
                checkResult.Result = true;

            return checkResult;
        }

        /// <summary>
        /// Kisa bar hem orta hem uzun u asagi keserse alip cikmalisin
        /// Siraya girmesini beklememelisin
        /// </summary>
        /// <returns></returns>
        public override CheckResult ShouldTakeProfitLong()
        {
            //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
             var maMiddle = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
                : TA.SMA.Calculate(closeArray, pMiddleMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

            //# Variables
            var maShortLast = maShort.Last();
            var maMiddleLast = maMiddle.Last();
            var maLongLast = maLong.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"MaShort[{pShortMaLength}]",$"[{maShortLast.RoundTo(5)}]" },
                   { $"MaMiddle[{pMiddleMaLength}]",$"[{maMiddleLast.RoundTo(5)}]" },
                   { $"MaLong[{pLongMaLength}]",$"[{maLongLast.RoundTo(5)}]" },
                   { "ClosePrice",closeArray.Last() },
                },
                Result = false
            };

            //# Decision
            checkResult.Result = maShortLast < maMiddleLast && maShortLast < maLongLast;

            return checkResult;
        }

        /// <summary>
        /// Kisa bar hem orta hem uzun u yukari keserse alip cikmalisin
        /// Siraya girmesini beklememelisin
        /// </summary>
        /// <returns></returns>
        public override CheckResult ShouldTakeProfitShort()
        {
           //# Technical Analysis
            var maShort = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pShortMaLength)
                : TA.SMA.Calculate(closeArray, pShortMaLength);
             var maMiddle = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pMiddleMaLength)
                : TA.SMA.Calculate(closeArray, pMiddleMaLength);
            var maLong = pMA.ToLower() == "ema"
                ? TA.EMA.Calculate(closeArray, pLongMaLength)
                : TA.SMA.Calculate(closeArray, pLongMaLength);

              //# Variables
            var maShortLast = maShort.Last();
            var maMiddleLast = maMiddle.Last();
            var maLongLast = maLong.Last();

            var checkResult = new CheckResult()
            {
                PositionLogs = new Dictionary<string, object>() {
                   { $"MaShort[{pShortMaLength}]",$"[{maShortLast.RoundTo(5)}]" },
                   { $"MaMiddle[{pMiddleMaLength}]",$"[{maMiddleLast.RoundTo(5)}]" },
                   { $"MaLong[{pLongMaLength}]",$"[{maLongLast.RoundTo(5)}]" },
                   { "ClosePrice",closeArray.Last() },
                },
                Result = false
            };

            //# Decision
            checkResult.Result = maShortLast > maMiddleLast && maShortLast > maLongLast;

            return checkResult;
        }

        public override bool CheckAlarm()
        {
            return false;
        }

        public override bool UpdateTrailingStop()
        {
            return false;
        }

        public override bool ShouldStop(double entryPrice, int side)
        {
            return false;
        }

        /// <summary>
        /// short > middle > long
        /// Long islem almak icin firsattir
        /// </summary>
        /// <param name="_short"></param>
        /// <param name="_middle"></param>
        /// <param name="_long"></param>
        /// <returns></returns>
        public bool SortShortToLong(double _shortLast, double _middleLast, double _longLast, double _shortPrev, double _middlePrev, double _longPrev)
        {
            return !(_shortPrev > _middlePrev && _shortPrev > _longPrev && _middlePrev > _longPrev) &&
                    (_shortLast > _middleLast && _shortLast > _longLast && _middleLast > _longLast);
        }

        /// <summary>
        /// short > middle > long
        /// Long islem almak icin firsattir
        /// </summary>
        /// <param name="_short"></param>
        /// <param name="_middle"></param>
        /// <param name="_long"></param>
        /// <returns></returns>
        public bool SortLongToShort(double _shortLast, double _middleLast, double _longLast, double _shortPrev, double _middlePrev, double _longPrev)
        {
            return !(_shortPrev < _middlePrev && _shortPrev < _longPrev && _middlePrev < _longPrev) &&
                    (_shortLast < _middleLast && _shortLast < _longLast && _middleLast < _longLast);
        }

        /// <summary>
        /// Basit bir veri ile testlerini yapalim
        /// </summary>
        /// <param name="path"></param>
        public static void Backtest_Simple(string path)
        {
            //lines.Add($"{candle.OpenTime};{candle.CloseTime};{candle.OpenPrice};{candle.HighPrice};{candle.LowPrice};{candle.ClosePrice}");
            var lines = System.IO.File.ReadAllLines("_sample_data/ARPAUSDT_1M_Binance_Sample.csv");
            var candles = new List<Candle>();
            foreach (var line in lines)
            {
                var arr = line.Split(';');
                var candle = new Candle()
                {
                    Open = double.Parse(arr[2]),
                    High = double.Parse(arr[3]),
                    Low = double.Parse(arr[4]),
                    Close = double.Parse(arr[5]),
                    Date = System.DateTime.Parse(arr[1])
                };

                candles.Add(candle);
            }

            var parameters = new Dictionary<string, object>() {
                { "ShortMaLength", 13 },
                { "MiddleMaLength", 34 },
                { "LongMaLength", 50 },
                { "MA", "SMA" },
            };

            for (int i = 150; i < candles.Count - 1; i++)
            {
                var strategy = new MARibbonStrategy(candles.Take(i).ToList(), parameters);
                var shouldLong = strategy.ShouldOpenLong();
                if (shouldLong.Result) 
                {
                }

                //buradan long giriyor musun, girmeli misin
                //son emalara bakalim
                System.Console.WriteLine("-------------------------------");
                System.Console.WriteLine("Mum-" + i + "=>" + candles[i].Date.ToString() + ":" + shouldLong.Result);
                System.Console.WriteLine($"Open: {candles[i].Open}; Close: {candles[i].Close}");
                foreach (var nv in shouldLong.PositionLogs) {
                    System.Console.Write($"{nv.Key}:{nv.Value} || ");
                }

                var shouldShort = strategy.ShouldOpenShort();
                if (shouldShort.Result) 
                {
                }
            }
        }
    }
}
