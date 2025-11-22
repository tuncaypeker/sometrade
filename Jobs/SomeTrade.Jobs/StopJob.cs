using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using CryptoExchange.Net.Interfaces;
using Hangfire;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SomeTrade.Data;
using SomeTrade.Infrastructure.Extensions;
using SomeTrade.Jobs.Helpers;
using SomeTrade.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace SomeTrade.Jobs
{
    public class StopJob
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

        public StopJob(RobotData robotData
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
        public void Execute()
        {
            var httpClient = new HttpClient();

            //mevcutta acik pozisyonlar var mi
            var openedPositions = robotPositionData.GetBy(x => !x.IsClosed);
            foreach (var openedPosition in openedPositions)
            {
                var robot = robotData.GetByKey(openedPosition.RobotId);
                var symbol = openedPosition.Symbol;//EX: OPUSDT
                var priceGatherPath = $"https://fapi.binance.com/fapi/v1/ticker/price?symbol={symbol}";
                var jsonResult = httpClient.GetStringAsync(priceGatherPath).Result;
                var jObjectPrice = JObject.Parse(jsonResult);

                var currentPrice = decimal.Parse(jObjectPrice["price"].ToString(), new NumberFormatInfo() { NumberDecimalSeparator = "." });

                //eger diff side 1 ve - ise zarardayım, fiyat dusmus demek
                //eger diff side 0 ve + ise kardayım, fiyat artmıs demek
                var difference = openedPosition.Side == 1
                    ? (currentPrice - openedPosition.EntryPrice)
                    : (openedPosition.EntryPrice - currentPrice);

                var differenceAmount = difference * openedPosition.Quantity;//toplamda ne kadar para artı ya da eksideyim, eger bu sıfırın altında ise zarardayım

                //************************** CHECK HARD STOP *************************************************

                if (robot.HardStopPercentage > 0 && differenceAmount < 0)
                {
                    //bu durumda zarardayım ve bu zarar belirlenen yüzdeden fazla ise hemen kapatmam gerekiyor
                    difference *= -1;
                    var lossPercentage = difference * 100 / openedPosition.EntryPrice;
                    if (lossPercentage > robot.HardStopPercentage)
                    {
                        var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);
                        bool closeResult = robotPositionHelper.ClosePositionInDb("Hard Stop'a[" + robot.HardStopPercentage + "%] temas etti ve kapatiyorum", openedPosition, robot);

                        openedPosition.Notes = $"Hard stop: Curr:${currentPrice},Entry:{openedPosition.EntryPrice},diff:{difference},DtNow:{DateTime.UtcNow},lossPercentage:{lossPercentage}";
                        robotPositionData.Update(openedPosition);

                        continue;
                    }
                }



                //************************** CHECK STOP PRICE *************************************************
                //bu pozisyonun stop fiyati var mi
                if (openedPosition.StopPrice > 0)
                {
                    //onceki fiyati alalim, txt dosyalarindan
                    var fileDirectory = "temp_price_files";
                    var filePath = $"{fileDirectory}/{symbol}.txt";
                    if (!System.IO.File.Exists(filePath))
                    {
                        if (!System.IO.Directory.Exists(fileDirectory)) System.IO.Directory.CreateDirectory(fileDirectory);

                        //write to file first time
                        System.IO.File.WriteAllText(filePath, currentPrice.ToString());
                    }
                    else
                    {
                        var fileContent = System.IO.File.ReadAllText(filePath);
                        var prevPrice = decimal.Parse(fileContent);

                        //check stop price if hit stop
                        if (
                             (prevPrice > openedPosition.StopPrice && currentPrice < openedPosition.StopPrice) ||
                             (prevPrice < openedPosition.StopPrice && currentPrice > openedPosition.StopPrice) ||
                             currentPrice == openedPosition.StopPrice)
                        {

                            var robotPositionHelper = new RobotPositionHelper(binanceClient, robotPositionData, robotData);
                            bool closeResult = robotPositionHelper.ClosePositionInDb("stop price'a temas etti ve kapatiyorum", openedPosition, robot);

                            System.IO.File.Delete(filePath);

                            continue;
                        }

                        //if not hit update price file
                        System.IO.File.WriteAllText(filePath, currentPrice.ToString());
                    }
                }







                //************************** CHECK ROBOT TRAILING STOP *************************************************
                //eger bu poz'un robotunda iz suren stop ayari varsa bunu girmem gerekir
                if (!robot.HasPercentageTrailingStop)
                    continue;

                //zarardaysa ne iz suren stopu
                if (differenceAmount <= 0)
                    continue;

                var totalAmount = openedPosition.Quantity * openedPosition.EntryPrice;

                var profitPercentage = 100 * differenceAmount / totalAmount;

                decimal newStopPercentage = 0m;
                string percentageLevel = "";
                foreach (var level in robot.GetTrailingStopPercentageLevels.OrderBy(x => x.Key))
                {
                    var hitPercentage = level.Key;
                    var stopPercentage = level.Value;

                    if (profitPercentage > hitPercentage)
                    {
                        newStopPercentage = stopPercentage;
                        percentageLevel = $"{hitPercentage}:{stopPercentage}";
                    }
                }

                if (newStopPercentage > 0)
                {
                    var priceDiff = openedPosition.EntryPrice * newStopPercentage / 100;

                    //stop price'i guncelleyelim
                    //iz suren stop mantigi bana stop noktasi ekle diyor
                    var newStopPrice = openedPosition.Side == 1
                        ? openedPosition.EntryPrice + priceDiff
                        : openedPosition.EntryPrice - priceDiff;

                    if (newStopPrice != openedPosition.StopPrice)
                    {
                        //tmm guncelliycem ama oldu'da bu level daha once girilmis tekrar girmeyelim
                        var robotPositionStopLog = robotPositionStopLogData.FirstOrDefault(x => x.RobotPositionId == openedPosition.Id && x.PercentageLevel == percentageLevel);
                        if (robotPositionStopLog == null)
                        {
                            //eger islem long ise mevcut iz süren stopun daha ustunde bir stop geldi ise guncellemeliyim
                            //eger islem shrt ise mevcut iz süren stop sıfır ise ya da daha altında ise guncellemeliyim
                            var shouldUpdateStop =
                                (openedPosition.Side == 1 && newStopPrice > openedPosition.StopPrice) ||
                                (openedPosition.Side == 0 && openedPosition.StopPrice == 0) ||
                                (openedPosition.Side == 0 && newStopPrice < openedPosition.StopPrice);

                            if (shouldUpdateStop)
                            {
                                openedPosition.StopPrice = newStopPrice;

                                robotPositionData.Update(openedPosition);

                                robotPositionStopLog = new RobotPositionStopLog()
                                {
                                    CreateDate = DateTime.UtcNow,
                                    Description = "İz süren stop güncellendi, robot'un iz suren stobu geldi",
                                    RobotPositionId = openedPosition.Id,
                                    StopPrice = openedPosition.StopPrice,
                                    PercentageLevel = percentageLevel
                                };
                                var stopLogInsertResult = robotPositionStopLogData.Insert(robotPositionStopLog);
                            }
                        }
                    }
                }
            }
        }
    }
}
