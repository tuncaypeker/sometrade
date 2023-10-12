﻿using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.TA.Tests.Utilities
{
    public static class BinanceHelper
    {
        public static List<DataCandle> GetCandles(string symbol, KlineInterval interval, int limit)
        {
             var binanceClient = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new BinanceApiCredentials("CvObhm6X2An9IzTlA7m9FXC2EWtyAcBOY7skdEZiuZ0QsvGk5ROA03taGLlzArGn",
                     "wIzj3TQrX4fRPF97ScGNk6ieSgsgFrs21JEI1kURSDULqugby2Ws61AbpMx1p1et")
            });

            //Canli fiyat verisi
            var binanceCandlesResult = binanceClient.UsdFuturesApi.ExchangeData.GetKlinesAsync(symbol, interval: interval, limit: limit).Result;

            if (!binanceCandlesResult.Success)
                return null;

            var serverDate = GetServerDate(binanceClient);
            var binanceServerHourDifference = (DateTime.Now - serverDate).TotalHours;

            //mumlara, binance server ile olan saat farkini da ekleyerek donelim
            var candlesFromBinance = binanceCandlesResult.Data.ToList();
            candlesFromBinance.ForEach(x =>
            {
                x.CloseTime = x.CloseTime.AddHours(binanceServerHourDifference);
                x.OpenTime = x.OpenTime.AddHours(binanceServerHourDifference);
            });

            //Son mum henüz kapanmadigi icin, dikkate almiyoruz. Tamamen kapanmis mumlar uzerinde islem yapiyoruz
            var candles = candlesFromBinance.Take(limit - 1).Select(x => new DataCandle(x.OpenPrice, x.HighPrice, x.LowPrice, x.ClosePrice,x.CloseTime, x.Volume)).ToList();

            return candles;
        }
        private static DateTime GetServerDate(BinanceClient binanceClient)
        {
            var serverDate = binanceClient.SpotApi.ExchangeData.GetServerTimeAsync().Result;

            return serverDate.Data;
        }
    }
}
