using Binance.Net.Clients;
using Binance.Net.Enums;
using SomeTrade.Data;
using SomeTrade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Helpers
{
    public class RobotPositionHelper
    {
        BinanceClient binanceClient;
        RobotPositionData robotPositionData;
        RobotData robotData;

        public RobotPositionHelper(BinanceClient binanceClient, RobotPositionData robotPositionData, RobotData robotData)
        {
            this.binanceClient = binanceClient;
            this.robotPositionData = robotPositionData;
            this.robotData = robotData;
        }

        /// <summary>
        /// pozisyon db'de kapatilir ve livetest guncellenir
        /// </summary>
        /// <param name="closePrice"></param>
        /// <param name="Notes"></param>
        /// <param name="liveTestPosition"></param>
        /// <param name="liveTest"></param>
        /// 
        /// <returns></returns>
        public bool ClosePositionInDb(string notes, Model.RobotPosition robotPosition, Model.Robot robot)
        {
            var positionResult = binanceClient.UsdFuturesApi.Account.GetPositionInformationAsync(robotPosition.Symbol).Result;
            if (positionResult.Data.Count() == 0)
                return false;
            var position = positionResult.Data.First();

            binanceClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(robotPosition.Symbol, position.Leverage).Wait();
            binanceClient.UsdFuturesApi.Account.ChangeMarginTypeAsync(robotPosition.Symbol, FuturesMarginType.Isolated).Wait();

            var placeOrderData = binanceClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                    robotPosition.Symbol,
                    position.Quantity <= 0 ? OrderSide.Buy : OrderSide.Sell,
                    FuturesOrderType.Market,
                    quantity: System.Math.Abs(robotPosition.Quantity)
                ).Result;

            if (!placeOrderData.Success)
                return false;

            //Emir islensin diye biraz bekleyelim
            System.Threading.Thread.Sleep(3 * 1000);

            //Emir ile ilgili detay alalim
            //TODO: Burada orderData acaba gercekleşmeyebilir mi
            //TODO: Burada orderdata gonderirken bir hata alabilir miyiz
            var closeOrderData = binanceClient.UsdFuturesApi.Trading.GetOrdersAsync(robotPosition.Symbol, placeOrderData.Data.Id).Result;
            if (closeOrderData.Data.Count() == 0)
                return false;

            var closeOrder = closeOrderData.Data.First();

            //cik
            robotPosition.IsClosed = true;
            robotPosition.ExitDate = DateTime.UtcNow;
            robotPosition.ExitPrice = Convert.ToDecimal(closeOrder.AvgPrice);
            robotPosition.Notes = notes;
            robotPosition.ExchangeExitOrderId = closeOrder.Id.ToString();

            decimal exitBudget = 0;

            if (robotPosition.Side == 1)
            {
                robotPosition.Profit = (robotPosition.ExitPrice * robotPosition.Quantity) - (robotPosition.EntryPrice * robotPosition.Quantity);
                exitBudget = robotPosition.EntryBudget + robotPosition.Profit;
            }
            else if (robotPosition.Side == 0)
            {
                //exitbudget, short oldugu icin aslinda fiyat farki ters hesaplanmali
                //45000den girdin 42000den ciktin aslinda bu 48000 den cikmis gibi olmali
                //45000den girdin 46000den ciktin aslinda bu 44000 den cikmis gibi olmali

                robotPosition.Profit = (robotPosition.EntryPrice * robotPosition.Quantity) - (robotPosition.ExitPrice * robotPosition.Quantity);
                exitBudget = robotPosition.EntryBudget + robotPosition.Profit;
            }
            robotPosition.ExitBudget = exitBudget;

            var updatePositionResult = robotPositionData.Update(robotPosition);

            robot.CurrentBudget += robotPosition.ExitBudget;
            var updateTestResult = robotData.Update(robot);

            return updatePositionResult.IsSucceed && updateTestResult.IsSucceed;

        }

        /// <summary>
        /// Gelen parametrelere uygun sekilde quantity belirler ve mevcut pozisyonu azaltir ya da tamamen kapatir
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="robot"></param>
        /// <param name="openedPosition"></param>
        /// <param name="divider">ne kadar kapatilacagini belirleyen parametre /1 tamami /2 yarisini /3 ucte birini</param>
        /// <returns></returns>
        private bool _closePositionInDbPartially(Model.Symbol symbol, Model.Robot robot, Model.RobotPosition openedPosition, int divider)
        {
            //divider 1 ise hepsini kapat aq
            if (divider <= 1)
                return false;

            //pozun yarısını kapat
            var positionResult = binanceClient.UsdFuturesApi.Account.GetPositionInformationAsync(openedPosition.Symbol).Result;
            if (positionResult.Data.Count() == 0)
                return false;

            var position = positionResult.Data.First();

            binanceClient.UsdFuturesApi.Account.ChangeInitialLeverageAsync(openedPosition.Symbol, position.Leverage).Wait();
            binanceClient.UsdFuturesApi.Account.ChangeMarginTypeAsync(openedPosition.Symbol, FuturesMarginType.Isolated).Wait();

            var newQuantity = System.Math.Abs(openedPosition.Quantity / divider);
            newQuantity = System.Math.Round(newQuantity, symbol.Precision);
            var placeOrderData = binanceClient.UsdFuturesApi.Trading.PlaceOrderAsync(
                    openedPosition.Symbol,
                    position.Quantity <= 0 ? OrderSide.Buy : OrderSide.Sell,
                    FuturesOrderType.Market,
                    quantity: newQuantity
                ).Result;

            if (!placeOrderData.Success)
                return false;

            //Emir islensin diye biraz bekleyelim
            System.Threading.Thread.Sleep(3 * 1000);

            //Emir ile ilgili detay alalim
            //TODO: Burada orderData acaba gercekleşmeyebilir mi
            //TODO: Burada orderdata gonderirken bir hata alabilir miyiz
            var closeOrderData = binanceClient.UsdFuturesApi.Trading.GetOrdersAsync(openedPosition.Symbol, placeOrderData.Data.Id).Result;
            if (closeOrderData.Data.Count() == 0)
                return false;

            var closeOrder = closeOrderData.Data.First();

            openedPosition.IsPartiallyClosed = true;
            openedPosition.Quantity = newQuantity;
            openedPosition.EntryBudget = openedPosition.EntryBudget / divider;

            robotPositionData.Update(openedPosition);
            
            /*
            robotPositionTickStopLogData.Insert(new RobotPositionTickStopLog()
            {
                LossTick = openedPosition.LossTicks,
                Price = closeOrder.AvgPrice,
                ProfitTick = openedPosition.ProfitTicks,
                Quantity = newQuantity,
                RobotPositionId = openedPosition.Id,
                CreateDate = DateTime.UtcNow
            });
            */

            return true;
        }
    }
}
