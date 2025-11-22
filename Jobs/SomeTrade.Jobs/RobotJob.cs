using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Hangfire;
using SomeTrade.Data;
using SomeTrade.Infrastructure.Extensions;
using SomeTrade.Jobs.Helpers;
using SomeTrade.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SomeTrade.Jobs
{
    public class RobotJob
    {
        Data.RobotData robotData;

        Data.RobotExecutionData robotExecutionData;
        Data.RobotExecutionCandleData robotExecutionCandleData;
        Data.RobotExecutionCalculationData robotExecutionCalculationData;

        Data.RobotPositionData robotPositionData;
        Data.RobotPositionCalculationData robotPositionCalculationData;
        Data.RobotPositionCandleData robotPositionCandleData;
        Data.RobotPositionPnlData robotPositionPnlData;
        Data.RobotPositionStopLogData robotPositionStopLogData;

        Data.RobotMetaValueData robotMetaValueData;
        Data.RobotSymbolPairData robotSymbolPairData;

        Data.SymbolData symbolData;
        Data.SymbolPairData symbolPairData;
        BinanceClient binanceClient;

        Data.StrategyMetaData strategyMetaData;
        Strategies.IStrategyResolver strategyResolver;

        public RobotJob(RobotData robotData
            , Strategies.IStrategyResolver strategyResolver, RobotPositionData robotPositionData
            , RobotPositionCalculationData robotPositionCalculationData, RobotPositionCandleData robotPositionCandleData, RobotMetaValueData robotMetaValueData
            , StrategyMetaData strategyMetaData, SymbolData symbolData
            , RobotPositionPnlData robotPositionPnlData, RobotExecutionData robotExecutionData
            , RobotExecutionCandleData robotExecutionCandleData, RobotExecutionCalculationData robotExecutionCalculationData, RobotSymbolPairData robotSymbolPairData
            , SymbolPairData symbolPairData, RobotPositionStopLogData robotPositionStopLogData)
        {
            this.robotData = robotData;
            this.strategyResolver = strategyResolver;
            this.robotPositionData = robotPositionData;
            this.robotPositionCalculationData = robotPositionCalculationData;
            this.robotPositionCandleData = robotPositionCandleData;
            this.strategyMetaData = strategyMetaData;

            binanceClient = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new BinanceApiCredentials("CvObhm6X2An9IzTlA7m9FXC2EWtyAcBOY7skdEZiuZ0QsvGk5ROA03taGLlzArGn",
                     "wIzj3TQrX4fRPF97ScGNk6ieSgsgFrs21JEI1kURSDULqugby2Ws61AbpMx1p1et")
            });

            this.robotMetaValueData = robotMetaValueData;
            this.symbolData = symbolData;
            this.robotPositionPnlData = robotPositionPnlData;

            this.robotExecutionData = robotExecutionData;
            this.robotExecutionCandleData = robotExecutionCandleData;
            this.robotExecutionCalculationData = robotExecutionCalculationData;
            this.robotSymbolPairData = robotSymbolPairData;
            this.symbolPairData = symbolPairData;
            this.robotPositionStopLogData = robotPositionStopLogData;
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(timeoutInSeconds: 2 * 60)]
        public void Execute(int robotId, List<string> symbols)
        {
            Console.Clear();

            var startDate = DateTime.UtcNow;
            var robot = robotData.GetByKey(robotId);
            if (robot == null || !robot.IsActive)
                return;

            robot.HeartBeat = DateTime.UtcNow;
            robotData.Update(robot);

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine($"{startDate.ToString("dd:mm:ss")} Robot Job[{robot.Name}]...");
            //robotHub.SendMessage(5, $"---------------------------------------", "green").Wait();

            //# Definitions
            KlineInterval interval = robot.Interval.ToBinanceInterval();
            var binanceHelper = new BinanceHelper(binanceClient);
            //------------------------------------------------------------------------------------------------------------
            var symbolNames = new string[symbols.Count];
            for (int i = 0; i < symbols.Count; i++)
                symbolNames[i] = symbols[i].Replace("USDT", "");
            var symbolsInDb = symbolData.GetBy(x => symbolNames.Contains(x.Name));

            var openPositions = robotPositionData.GetBy(x => x.RobotId == robot.Id && !x.IsClosed);
            if (openPositions == null)
                openPositions = new List<Model.RobotPosition>();

            //bu oncesinde _buildScreening altinda cagiriliyordu ama bu her symbol'de cagirilmasi demek
            //burada farklı ScreeningId'ler gelemeyecegi icin her symbol icin meta data tekrar cekmek anlamsiz
            var strategyMetas = strategyMetaData.GetBy(x => x.StrategyId == robot.StrategyId);
            var metaIds = strategyMetas.Select(x => x.Id);
            var robotMetaValues = robotMetaValueData.GetBy(x => metaIds.Contains(x.StrategyMetaId) && x.RobotId == robotId);
            var robotMetaValuesDictionary = new Dictionary<string, object>();
            foreach (var strategyMeta in strategyMetas)
            {
                var metaValue = robotMetaValues.FirstOrDefault(x => x.StrategyMetaId == strategyMeta.Id);
                if (metaValue == null)
                    continue;

                robotMetaValuesDictionary.Add(strategyMeta.Name, metaValue.Value);
            }

            //TODO: Bu bizim db'de acik ama gercekten binance'da acik mi?
            foreach (var symbol in symbols)
            {
                //db'den symbol'u almamiz lazim
                var name = symbol.Replace("USDT", "");
                var symbolInDb = symbolsInDb.Where(x => x.Name == name).FirstOrDefault();

                Console.WriteLine($"{symbol} @ {DateTime.UtcNow.ToLongTimeString()}");

                //bu symbol ile acilan pozisyon var mi
                var openedPosition = openPositions.FirstOrDefault(x => x.Symbol == symbol);

                //Bu symbol'un acik pozisyonu yok ve toplam pozisyon sayisi izin verilenden cok
                //dolayisiyla yeni pozisyon acma sansimiz yok 
                bool hasReachedMaxPositionLimit = false;
                if (openedPosition == null && openPositions.Count >= robot.MaxPosition)
                    hasReachedMaxPositionLimit = true;

                var candles = _getCandles(robot, binanceHelper, symbol, interval);
                if (candles == null)
                {
                    Console.WriteLine("\t Binance'den mumlari alamadim..");
                    continue;
                }

                //# tabi burada gecici sinyal, kalici sinyal ayarina sadik kalmaliyiz
                //# eger gecici sinyal ile işlem aç seçili ise pata pat aç kapat olmali, diger turlu mum kapandi mi
                //# sorusu devreye giriyor, mum kapandi mi, kapnadiktan sonra ne kadar süre gecti gibi
                if (!robot.AllowOnTemporarySignal)
                {
                    var dtNow = DateTime.UtcNow;
                    var secondsFromCandleClose = (dtNow - candles.LastCandleCloseTime).TotalSeconds;
                    var secondsAllowing = robot.AllowingTolerateSeconds <= 0
                        ? robot.Interval.ToTolerateInSeconds()
                        : robot.AllowingTolerateSeconds;

                    if (secondsFromCandleClose > secondsAllowing)
                    {
                        //robotHub.SendMessage(4, $"{symbol}:closetime[{candlesFromExchange.LastCandleCloseTime.ToString("HH:mm:ss")}] mum kapanmasini bekliyorum", "orange").Wait();
                        Console.WriteLine($"\t Tolere edilen sureyi gectiği icin islem almiyorum [{dtNow.ToLongTimeString()}] - [{candles.LastCandleCloseTime.ToLongTimeString()}]..");
                        continue;
                    }
                }

                var strategy = _buildStrategy(robot.StrategyId, candles.Candles, robotMetaValuesDictionary);

                //tehlikeli ama en azindan, hata verdirmez
                if (strategy == null)
                    continue;

                //# Acik pozisyon var ise, take profit ya da stop-loss kontrol etmelisin
                if (openedPosition != null)
                {
                    var shouldCloseLong = strategy.ShouldCloseLong();
                    var shouldCloseShort = strategy.ShouldCloseShort();
                    var shouldUpdateTrailingStop = strategy.UpdateTrailingStop(openedPosition.EntryPrice, openedPosition.Side, openedPosition.Quantity);

                    //acaba son 1 dk içinde eklenen log var mi? varsa tekrar ekleme
                    //TODO, son 1 dk içinde eklenen log demişsin ama +1 diyerek gelecek 1dk yı eklemişsin, ek olarak 1dk lık mumlarla calisirsak burasi sapitmaz mi bilemedim
                    var dateTime1MAgo = DateTime.UtcNow.AddMinutes(1);
                    var logAnyInDb = robotPositionCalculationData.FirstOrDefault(x => x.Date > dateTime1MAgo && x.RobotPositionId == openedPosition.Id);
                    if (logAnyInDb == null)
                    {
                        var strategyCalculations = strategy.GetCalculations();
                        var robotPositionCalculations = _generateRobotPositionCalculations(openedPosition.Id, strategyCalculations, candles.Last.OpenTime);

                        var logBulkInsert = robotPositionCalculationData.InsertBulk(robotPositionCalculations);
                        if (!logBulkInsert.IsSucceed)
                        { 
                            
                        }

                        //current price ile yuzde farkini hesaplayalim
                        var currentPrice = candles.Last.ClosePrice;
                        var entryPrice = openedPosition.EntryPrice;
                        var percentage = (100 * currentPrice / entryPrice) - 100;

                        //if position side is short than percentage 
                        if (openedPosition.Side == 0)
                            percentage *= -1;

                        var entryPriceForPnl = openedPosition.EntryPrice * openedPosition.Quantity;
                        var currentPriceForPnl = candles.Last.ClosePrice * openedPosition.Quantity;
                        decimal currentProfit = 0;
                        if (openedPosition.Side == 1)
                            currentProfit = currentPriceForPnl - entryPriceForPnl;
                        else if (openedPosition.Side == 0)
                            currentProfit = entryPriceForPnl - currentPriceForPnl;

                        var robotPositionPnl = new Model.RobotPositionPnl()
                        {
                            CurrentPrice = currentPrice,
                            Date = candles.Last.CloseTime,
                            Percentage = percentage,
                            RobotPositionId = openedPosition.Id,
                            CurrentProfit = currentProfit
                        };

                        var pnlInsert = robotPositionPnlData.Insert(robotPositionPnl);
                        if (!pnlInsert.IsSucceed)
                        { 
                            
                        }
                    }

                    var candleAnyInDb = robotPositionCandleData.FirstOrDefault(x => x.OpenTime == candles.LastCandleCloseTime && x.RobotPositionId == openedPosition.Id);
                    if (candleAnyInDb == null)
                    {
                        var livetestCandle = _generateRobotCandle(openedPosition.Id, candles.Last);
                        var candleInsert = robotPositionCandleData.Insert(livetestCandle);

                        if (!candleInsert.IsSucceed)
                        { 
                            
                        }
                    }

                    //pozisyon kapatildi ise yeni pozisyon icin firsat aranacaktir
                    bool positionClosed = false;

                    //max kar zarar durumunu hesaplayalim
                    var entry = openedPosition.EntryPrice * openedPosition.Quantity;
                    var current = candles.Last.ClosePrice * openedPosition.Quantity;
                    var difference = openedPosition.Side == 1
                        ? current - entry
                        : entry - current;
                    if (difference > 0 && difference > openedPosition.MaxProfit)
                        openedPosition.MaxProfit = difference;
                    if (difference < 0 && difference < openedPosition.MaxLoss)
                        openedPosition.MaxLoss = difference;

                    if (difference > 0) openedPosition.ProfitTicks += 1;
                    else openedPosition.LossTicks += 1;

                    robotPositionData.Update(openedPosition);

                    bool stoppedByProfitLossTickStop = _checkProfitLossTickStop(symbolInDb, robot, openedPosition);

                    //position long mu short mu ve stop olmamis mi
                    if (openedPosition.Side == 1 && !positionClosed)
                    {
                        if (shouldCloseLong.Result)
                        {
                            var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);
                            bool closeResult = robotPositionHelper.ClosePositionInDb("Should Close Long true geldi", openedPosition, robot);

                            if (closeResult)
                                openPositions.Remove(openPositions.FirstOrDefault(x => x.Id == openedPosition.Id));

                            hasReachedMaxPositionLimit = false;
                            positionClosed = closeResult;
                        }
                    }
                    else if (openedPosition.Side == 0 && !positionClosed)
                    {
                        if (shouldCloseShort.Result)
                        {
                            var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);
                            bool closeResult = robotPositionHelper.ClosePositionInDb("Should Close Short true geldi", openedPosition, robot);
                            if (closeResult)
                                openPositions.Remove(openPositions.FirstOrDefault(x => x.Id == openedPosition.Id));

                            hasReachedMaxPositionLimit = false;
                            positionClosed = closeResult;
                        }
                    }

                    //eger pozisyon kapatilmadi ise, tekrar acmamak icin continue ile gecelim
                    //kapatildi ise yeni pozisyon firsati arayalim
                    if (!positionClosed)
                    {
                        if (shouldUpdateTrailingStop.Result && openedPosition.StopPrice != shouldUpdateTrailingStop.NewStopPrice)
                        {
                            //iz suren stop mantigi bana stop noktasi ekle diyor
                            openedPosition.StopPrice = shouldUpdateTrailingStop.NewStopPrice;

                            robotPositionData.Update(openedPosition);

                            var robotPositionStopLog = new RobotPositionStopLog()
                            {
                                CreateDate = DateTime.UtcNow,
                                Description = "İz süren stop güncellendi, stratejinin iz suren stobu geldi",
                                RobotPositionId = openedPosition.Id,
                                StopPrice = openedPosition.StopPrice
                            };
                            var stopLogInsertResult = robotPositionStopLogData.Insert(robotPositionStopLog);
                        }

                        continue;
                    }
                }

                //bu noktadan sonra yeni pozisyon acmaya calisiyorsun, dolayisiyla max position count'a geldik ise acamazsın
                if (hasReachedMaxPositionLimit)
                    continue;

                if (robot.ShouldLog)
                {
                    //symbolpair'de exchange ve FromSymbol'de var, exchange binance:2, FromSymbol'de 388: USDT ama yine de bu şekilde almak arizali
                    var symbolPair = symbolPairData.FirstOrDefault(x => x.ToSymbolId == symbolInDb.Id);
                    var robotSymbolPair = robotSymbolPairData.FirstOrDefault(x => x.RobotId == robot.Id && x.SymbolPairId == symbolPair.Id);
                    if (robotSymbolPair != null)
                    {
                        //yeni execution insert etmem gerekiyor
                        var newRobotExecution = new Model.RobotExecution()
                        {
                            Date = DateTime.UtcNow,
                            Notes = "",
                            RobotId = robot.Id,
                            RobotSymbolPairId = robotSymbolPair.Id
                        };

                        var executionInsertResult = robotExecutionData.Insert(newRobotExecution);
                        if (executionInsertResult.IsSucceed)
                        {
                            //candle
                            var robotExecutionCandle = new Model.RobotExecutionCandle()
                            {
                                Close = candles.Last.ClosePrice,
                                High = candles.Last.HighPrice,
                                Low = candles.Last.LowPrice,
                                Volume = candles.Last.Volume,
                                Open = candles.Last.OpenPrice,
                                RobotExecutionId = newRobotExecution.Id,
                                StartTime = candles.Last.OpenTime,
                                RobotId = robot.Id,
                                Symbol = name
                            };
                            var candleInsertData = robotExecutionCandleData.Insert(robotExecutionCandle);

                            //metaValues
                            //strategy'den gelen bilgileri, strategy metaId'ler ile esle ve kaydet
                            //burada calculations'ı nasil yapacaksın, bunun icin bir metod yazıp metavalue degerlerini stratejiden alabiliriz
                            var robotExecutionCalculations = new List<Model.RobotExecutionCalculation>();
                            var calculations = strategy.GetCalculations();
                            foreach (var calculation in calculations)
                            {
                                robotExecutionCalculations.Add(new RobotExecutionCalculation()
                                {
                                    Key = calculation.Key,
                                    RobotExecutionId = newRobotExecution.Id,
                                    Value = calculation.Value,
                                    RobotId = robot.Id
                                });
                            }

                            var calculationInsertData = robotExecutionCalculationData.InsertBulk(robotExecutionCalculations);
                        }
                    }
                }

                //# Acik pozisyon yok ise, yeni pozisyon acma firsati var mi bakmalisin
                var shouldOpenLong = strategy.ShouldOpenLong();
                var shouldOpenShort = strategy.ShouldOpenShort();

                //Ayni anda ikisini de acma soz konusu olamaz
                //todo: buraya dusuyorsa mutlaka uyarmali
                if (shouldOpenLong.Result && shouldOpenShort.Result)
                    continue;

                //Bir pozisyon acma firsati yoksa diger sembole gec
                if (!shouldOpenLong.Result && !shouldOpenShort.Result)
                    continue;

                var entryBudget = robot.StartBudget * robot.PercentagePerPosition / 100;
                if (entryBudget > robot.CurrentBudget)
                {
                    //robotHub.SendMessage(6, "Bütçe yetersiz giriş yapamadım", "red").Wait();
                    Console.WriteLine("\t Bütçe Yetersiz giriş yapamadım..");
                    continue;
                }

                //quantity hesaplayalim, burada yuvarlamalar sebbeiyle cent bazinda tutarsizliklar olabilir
                //TODO: cozum olarak, binance ile sync olunabilir 
                // https://api.binance.com/api/v3/exchangeInfo buradan LOT_SIZE bilgisini alman lazim
                //asagida math.round yapıyorsun o olmamali, LOT_SIZE dan alıp ona gore yuvarlamalisin
                var quantity = Convert.ToDecimal(entryBudget / Convert.ToDecimal(candles.ClosePrice) * robot.Leverage);

                //leverage carpani burada miktari kaldiriyor
                quantity = decimal.Round(quantity, 3, MidpointRounding.AwayFromZero);

                openedPosition = new Model.RobotPosition()
                {
                    RobotId = robot.Id,
                    EntryPrice = Convert.ToDecimal(candles.ClosePrice),
                    Symbol = symbol,
                    RobotPositionCandles = _generateRobotCandles(-1, candles.Candles),
                    EntryBudget = entryBudget,
                    Leverage = robot.Leverage,
                    Quantity = Math.Round(quantity, symbolInDb.Precision),
                    RobotPositionCalculations = _generateRobotPositionCalculations(-1, strategy.GetCalculations(), candleDate: candles.Last.OpenTime)
                };

                if (shouldOpenLong.Result)
                {
                    openedPosition.Side = 1;

                    binanceClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(symbol, robot.Leverage).Wait();
                    binanceClient.UsdFuturesApi.Account.ChangeMarginTypeAsync(symbol, FuturesMarginType.Isolated).Wait();
                    var orderData = binanceClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                               symbol,
                               side: OrderSide.Buy,
                               FuturesOrderType.Market,
                               quantity: openedPosition.Quantity
                        ).Result;

                    if (!orderData.Success)
                    {
                        //TODO: acmaya calisip acamadik, mutlaka log atmali, incelemeli
                        Console.WriteLine(orderData.Error == null ? "Long islem acmaya calisirken hata: " : "Long islem acmaya calisirken hata: " + orderData.Error.Message);

                        continue;
                    }

                    openedPosition.ExchangeEntryOrderId = orderData.Data.Id.ToString();

                    System.Threading.Thread.Sleep(3 * 1000);//marketten aldigim icin, bilgisini alabilmem lazim.

                    //TODO: bunu order id ile detay olarak da alabiliriz, bu durumda position'a donmus mu vs gorebiliriz sanirim
                    var positionResult = binanceClient.UsdFuturesApi.Account.GetPositionInformationAsync(symbol).Result;
                    if (positionResult.Success && positionResult.Data.Count() > 0)
                    {
                        var positionResultFirst = positionResult.Data.ToList()[0];

                        openedPosition.SyncedExchange = true;
                        openedPosition.SyncedDate = DateTime.UtcNow;
                        openedPosition.ExchangeEntryPrice = positionResultFirst.EntryPrice;
                        openedPosition.LiquidationPrice = positionResultFirst.LiquidationPrice;
                        openedPosition.ExchangeQuantity = positionResultFirst.Quantity;
                        openedPosition.Leverage = positionResultFirst.Leverage;
                        openedPosition.StopPrice = 0;

                        _telegramHelper._trySendAlertToTelegram(symbol, robot.Interval, openedPosition.ExchangeEntryPrice + " dan long actım");
                    }
                }

                if (shouldOpenShort.Result)
                {
                    openedPosition.Side = 0;

                    binanceClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(symbol, robot.Leverage).Wait();
                    binanceClient.UsdFuturesApi.Account.ChangeMarginTypeAsync(symbol, FuturesMarginType.Isolated).Wait();
                    var orderData = binanceClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                               symbol,
                               side: OrderSide.Sell,
                               FuturesOrderType.Market,
                               quantity: openedPosition.Quantity
                        ).Result;

                    if (!orderData.Success)
                    {
                        //TODO: acmaya calisip acamadik, mutlaka log atmali, incelemeli
                        Console.WriteLine(orderData.Error == null ? "Short islem acmaya calisirken hata: " : "Short islem acmaya calisirken hata: " + orderData.Error.Message);

                        continue;
                    }

                    openedPosition.ExchangeEntryOrderId = orderData.Data.Id.ToString();

                    System.Threading.Thread.Sleep(3 * 1000);//marketten aldigim icin, bilgisini alabilmem lazim.
                    var positionResult = binanceClient.UsdFuturesApi.Account.GetPositionInformationAsync(symbol).Result;
                    if (positionResult.Success && positionResult.Data.Count() > 0)
                    {
                        var positionResultFirst = positionResult.Data.ToList()[0];

                        openedPosition.SyncedExchange = true;
                        openedPosition.SyncedDate = DateTime.UtcNow;
                        openedPosition.ExchangeEntryPrice = positionResultFirst.EntryPrice;
                        openedPosition.LiquidationPrice = positionResultFirst.LiquidationPrice;
                        openedPosition.ExchangeQuantity = positionResultFirst.Quantity;
                        openedPosition.Leverage = positionResultFirst.Leverage;
                        openedPosition.StopPrice = 0;

                        _telegramHelper._trySendAlertToTelegram(symbol, robot.Interval, openedPosition.ExchangeEntryPrice + " dan short actım");
                    }
                }

                var dbInsertResultLong = robotPositionData.Insert(openedPosition);
                if (dbInsertResultLong.IsSucceed)
                    openPositions.Add(openedPosition);

                //TODO: else'de mutlaka biseyler olmali, pozisyon actin db'ye kaydedemedin. Felaket
                robot.CurrentBudget -= entryBudget;
                var dbLivetestUpdateResult = robotData.Update(robot);
            }

            _detachEntities();
        }

        /// <summary>
        /// Uzun süre zararda devam eden pozisyonların kademeli olarak küçültülmesi
        /// Kademeli olarak satacagi icin burada level onemli
        ///     Level 0: İşlem yok
        ///     Level 1: 10 mum 100% zarar tamamını kapat
        ///     Level 2: 20 mum 80% zarar tamamını kapat
        ///     Level 3: 40 mum 70% zarar tamamını kapat
        ///     
        /// Bu level'i de poz üzerinde tutmak lazim
        /// </summary>
        /// <param name="openedPosition"></param>
        /// <returns></returns>
        private bool _checkProfitLossTickStop(Model.Symbol symbol, Model.Robot robot, Model.RobotPosition openedPosition)
        {
            var profitTickCount = openedPosition.ProfitTicks;
            var lossTickCount = openedPosition.LossTicks;
            var totalTickCount = profitTickCount + lossTickCount;
            var lossTickPercentage = 0;

            //LEVEL 0 => 10. mumda eger 100% zararlı devam ediyorsa, bu pozisyonunun tamamını kapat
            if (totalTickCount < 10)
                return false;

            var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);

            lossTickPercentage = 100 * lossTickCount / totalTickCount;
            if (lossTickPercentage == 100)
                return robotPositionHelper.ClosePositionInDb("10 mum sonunda 100% ekside oldugu icin kapadim", openedPosition, robot);

            //-------------------


            //LEVEL 1 => 20. mumda eger 80% zararlı devam ediyorsa, bu pozisyonunun tamamını kapat
            if (totalTickCount < 20)
                return false;

            lossTickPercentage = 100 * lossTickCount / totalTickCount;
            if (lossTickPercentage >= 80)
                return robotPositionHelper.ClosePositionInDb("20 mum sonunda 80% ekside oldugu icin kapadim", openedPosition, robot);
            //-------------------

            //LEVEL 2 => 40. mumda eger 70% zararlı devam ediyorsa, bu pozisyonunun tamamını kapat
            if (totalTickCount < 40)
                return false;

            lossTickPercentage = 100 * lossTickCount / totalTickCount;
            if (lossTickPercentage > 70)
                return robotPositionHelper.ClosePositionInDb("40 mum sonunda 70% ekside oldugu icin kapadim", openedPosition, robot);
            //-------------------


            return false;
        }



        /// <summary>
        /// Local log dosyasindan ya da binance uzerinden bana mum verilerini getir
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="binanceHelper"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private JobBinanceCandlesDto _getCandles(Robot robot, BinanceHelper binanceHelper, string symbol, KlineInterval interval)
        {
            //Canli fiyat verisi
            //burada local bir log dosyasina aldigimiz mum verilerini yazmali ve her seferinde binance'den 300-500 mum verisi almamaliyiz
            //{robotid_candlelimit_symbol_interval.txt}
            //kritik olan konu ise bu verilerin guncel oldugundan emin olmaliyiz
            //dolayisiyla asamalarimiz su sekilde olmali
            //1- dosya yok ise tum veriyi cekerek yazmaliyiz.
            //2- dosya var ancak guncel degil ise tum veriyi cekerek yazmaliyiz
            //3- timeframe'de degisiklik var ise (timeframe degisti ise ztn dosya ismi de degisir)
            //4- candlelimit degisti ise tum veriyi cekerek yazmaliyiz (candlelimit degisti ise ztn dosya ismi de degisir)
            JobBinanceCandlesDto candlesFromExchange = null;

            if (!System.IO.Directory.Exists("_candles"))
                System.IO.Directory.CreateDirectory("_candles");
            var logFileName = $"_candles/robot_{robot.Id}_{robot.CandleLimit}_{symbol}_{(int)robot.Interval}.txt";
            bool writeLogAgain = false;
            if (!System.IO.File.Exists(logFileName))
                writeLogAgain = true;

            //Dosya var ancak guncel mi bakalim
            else
            {
                candlesFromExchange = new JobBinanceCandlesDto();
                var canFill = candlesFromExchange.FillFromLogFile(logFileName);

                if (canFill)
                {
                    //dosyanin guncel oldugunu nasil anlariz, son 2 veriyi ceksek ve sondan bir onceki mum'u karsilastirsak mesela
                    //bu durumda eger sondan bir onceki listeden gelen son mum'a esitse, 2 mum verisi ile bu isi cozmus oluruz
                    //diger turlusu de almam gereken zaman'i hesap edip burada kontrol etmek ki bu su an icin beni zorladi
                    //zaman farkı, timeframe'e gore hesap etmek, son 2 verinin alinmasi ve kontrol edilmesi daha kolay gibi

                    var candlesFromExchangeToCheck = binanceHelper.GetCandles(symbol, interval: interval, limit: 3);
                    //bize gelen sondan bir onceki veri, listeden gelen son veri ile eşleşmeli
                    //bu eslesme iki sekilde olabilir
                    //1- Yeni mum geldi ise yeni verilerden bir onceki, bizdeki verinin sonuncusu ile eşleşmelidir
                    //2- Yeni mum gelmedi ise (bizdeki son veri 09:00, 09:30'da istek atarsan en son yine 09:00 gelir) bu durumda gelen son iki bizdeki son iki ile eslesmelidir
                    if (candlesFromExchange.LastCandleCloseTime.EqualsUpToSeconds(candlesFromExchangeToCheck.Candles[1].CloseTime))
                    {
                        //son veriler ayni dolayisiyla su an baska bir veri cekmeme gerek olmadan devam etmeliyim
                        //bu bana binance'ten sadece 2 veri cekerek ilerlememi saglar
                    }
                    //eger son veriler ayni degilse bir ihtimal yeni mum verisi gelmistir, bu da demek oluyor ki
                    //benim listemde son veri, binance'ten gelen bir onceki veri ile eslesmeli
                    else if (candlesFromExchange.LastCandleCloseTime.EqualsUpToSeconds(candlesFromExchangeToCheck.Candles[0].CloseTime))
                    {
                        //bu durumda yeni bir mum verisi geldi, dolayısıyla bu mum verisini log dosyasina yazarak 
                        //yoluma devam edebilrim, gunun sonunda yine sadece 2 mum verisi alarak yoluma devam ediyorum
                        candlesFromExchange.Candles.Add(candlesFromExchangeToCheck.Candles[1]);
                        candlesFromExchange.AppendLastCandleToLogFile(logFileName);
                    }
                    else
                    {
                        //maalesef veri eslenmedi, yeniden yazmamiz gerekiyor
                        writeLogAgain = true;
                    }
                }
                else
                    writeLogAgain = true;
            }

            //geldigimiz noktada tekrar yaz'a karar verildi ise binance'den candleLimit kadar alip yaziyorum
            if (writeLogAgain)
            {
                //get min 500
                var candleLimit = robot.CandleLimit;
                if (candleLimit < 500)
                    candleLimit = 500;

                candlesFromExchange = binanceHelper.GetCandles(symbol, interval: interval, limit: candleLimit);
                if (candlesFromExchange != null)
                    candlesFromExchange.WriteToLogFile(logFileName);
            }

            return candlesFromExchange;
        }

        public bool ClosePosition(int id)
        {
            var robotPosition = robotPositionData.GetByKey(id);
            if (robotPosition == null || robotPosition.IsClosed)
                return true;

            var robot = robotData.GetByKey(robotPosition.RobotId);

            var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);

            return robotPositionHelper.ClosePositionInDb("Manuel olarak kapatildi", robotPosition, robot);
        }

        /// <summary>
        /// StrategyId, Strategy Meta  ve Mum verileri ile Strategy'i ayaga kaldirir
        /// </summary>
        /// <param name="strategyId"></param>
        /// <param name="candles"></param>
        /// <returns></returns>
        private Strategies.Interfaces.IStrategy _buildStrategy(int strategyId, IEnumerable<Binance.Net.Interfaces.IBinanceKline> candles
            , Dictionary<string, object> robotMetaValuesDictionary)
        {
            var strategy = strategyResolver.GetStrategyById(strategyId
                , candles.Select(x => new Strategies.Dto.Candle(x.CloseTime, x.OpenPrice, x.ClosePrice, x.LowPrice, x.HighPrice, x.Volume, x.OpenTime)).ToList()
                , robotMetaValuesDictionary);

            return strategy;
        }

        private void _detachEntities()
        {
            robotData.DetachAllEntities();
            robotPositionData.DetachAllEntities();
            robotPositionCalculationData.DetachAllEntities();
            robotPositionCandleData.DetachAllEntities();
        }

        /// <summary>
        /// Sonucta bu calculation ilgili candle ile eşleniyor, dolayisiyla bana candleDate'i
        /// su an loglanan candle'in dateini vermelisin ki ben o date ile bu calculation'lari 
        /// kaydedeyim, raporlarken eşleştirebileyim
        /// </summary>
        /// <param name="robotPositionId"></param>
        /// <param name="calculations"></param>
        /// <param name="candleDate"></param>
        /// <returns></returns>
        private List<Model.RobotPositionCalculation> _generateRobotPositionCalculations(int robotPositionId,
            Dictionary<string, string> calculations, DateTime candleDate)
        {
            var list = new List<Model.RobotPositionCalculation>();

            foreach (var keyValue in calculations)
            {
                list.Add(new Model.RobotPositionCalculation()
                {
                    Date = candleDate,
                    Key = keyValue.Key,
                    Value = keyValue.Value,
                    RobotPositionId = robotPositionId
                });
            }

            return list;
        }

        private Model.RobotPositionCandle _generateRobotCandle(int robotPositionId,
            Binance.Net.Interfaces.IBinanceKline candle)
        {
            return new Model.RobotPositionCandle()
            {
                Close = candle.ClosePrice,
                High = candle.HighPrice,
                Low = candle.LowPrice,
                Open = candle.OpenPrice,
                OpenTime = candle.OpenTime,
                Volume = candle.Volume,
                RobotPositionId = robotPositionId
            };
        }

        private List<Model.RobotPositionCandle> _generateRobotCandles(int robotPositionId, IEnumerable<Binance.Net.Interfaces.IBinanceKline> candles)
        {
            return candles.Select(x => _generateRobotCandle(robotPositionId, x)).ToList();
        }

        public static int _getStrategyMetaValueAsInt(string name, List<Model.StrategyMeta> metas
            , List<Model.RobotMetaValue> values)
        {
            var value = _getMetaValueAsString(name, metas, values);

            return Convert.ToInt32(value);
        }

        public static double _getStrategyMetaValueAsDouble(string name, List<Model.StrategyMeta> metas
            , List<Model.RobotMetaValue> values)
        {
            var str = _getMetaValueAsString(name, metas, values);
            str = str.Replace(',', '.');
            double value;
            double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out value);

            return value;
        }

        public static string _getMetaValueAsString(string name, List<Model.StrategyMeta> metas
            , List<Model.RobotMetaValue> values)
        {
            var meta = metas.FirstOrDefault(x => x.Name == name);
            if (meta == null)
                return meta.DefaultValue.ToString();

            string value = values.FirstOrDefault(x => x.StrategyMetaId == meta.Id).Value;

            return value;
        }
    }
}
