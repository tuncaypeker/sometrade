using Binance.Net.Clients;
using Binance.Net.Enums;
using SomeTrade.Infrastructure.Extensions;
using SomeTrade.Jobs.Helpers;
using SomeTrade.Model;
using SomeTrade.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs
{
    public class AlertJob
    {
        public static readonly string queue_name = "alert_queue";
        public static readonly string job_format = "Alert-{0}";

        Data.AlertData alertData;
        Data.ScreeningMetaData screeningMetaData;
        Data.AlertMetaValueData alertMetaValueData;
        Data.AlertLogData alertLogData;
        Strategies.IScreeningResolver screeningResolver;
        BinanceClient binanceClient;
        BinanceHelper binanceHelper;

        public AlertJob(Data.AlertData alertData, Data.ScreeningMetaData screeningMetaData, Data.AlertMetaValueData alertMetaValueData
            , Strategies.IScreeningResolver screeningResolver
            , Data.AlertLogData alertLogData)
        {
            this.alertData = alertData;
            this.screeningMetaData = screeningMetaData;
            this.alertMetaValueData = alertMetaValueData;
            this.screeningResolver = screeningResolver;
            this.alertLogData = alertLogData;
            this.binanceClient = new BinanceClient();
            this.binanceHelper = new BinanceHelper(binanceClient);
        }

        public void Execute(int alertId, List<string> symbols)
        {
            var startDate = DateTime.UtcNow;
            var alert = alertData.GetByKey(alertId);
            if (alert == null || !alert.IsActive)
                return;

            Console.WriteLine($"{startDate.ToString("dd:mm:ss")} Alert Job[{alert.Name}]...");

            //# Definitions
            var interval = alert.Interval.ToBinanceInterval();

            var alertLog = new Model.AlertLog()
            {
                AlertId = alertId,
                EndDate = new DateTime(1970, 1, 1),
                IsSucceed = false,
                StartDate = DateTime.UtcNow,
                Values = ""
            };
            var dbResultLogInsert = alertLogData.Insert(alertLog);


            //bu oncesinde _buildScreening altinda cagiriliyordu ama bu her symbol'de cagirilmasi demek
            //burada farklı ScreeningId'ler gelemeyecegi icin her symbol icin meta data tekrar cekmek anlamsiz
            var screeningMetas = screeningMetaData.GetBy(x => x.ScreeningId == alert.ScreeningId);
            var metaIds = screeningMetas.Select(x => x.Id);
            var alertMetaValues = alertMetaValueData.GetBy(x => metaIds.Contains(x.ScreeningMetaId) && x.AlertId == alertId);
            var alertMetaValuesDictionary = new Dictionary<string, object>();
            foreach (var screeningMeta in screeningMetas)
            {
                var metaValue = alertMetaValues.FirstOrDefault(x => x.ScreeningMetaId == screeningMeta.Id);
                if (metaValue == null)
                    continue;

                alertMetaValuesDictionary.Add(screeningMeta.Name, metaValue.Value);
            }

            foreach (var symbol in symbols)
            {
                //Canli fiyat verisi, ilk symbol'u loop disinda aldigimiz icin, tekrar almaya gerek yok
                var candles = _getCandles(alert, binanceHelper, symbol, interval);
                if (candles == null)
                {
                    Console.WriteLine("\t Binance'den mumlari alamadim..");
                    continue;
                }

                //# tabi burada gecici sinyal, kalici sinyal ayarina sadik kalmaliyiz
                //# eger gecici sinyal ile işlem aç seçili ise pata pat aç kapat olmali, diger turlu mum kapandi mi
                //# sorusu devreye giriyor, mum kapandi mi, kapnadiktan sonra ne kadar süre gecti gibi
                if (!alert.AllowOnTemporarySignal)
                {
                    var secondsFromCandleClose = (DateTime.UtcNow - candles.LastCandleCloseTime).TotalSeconds;
                    var tolerateSeconds = alert.Interval.ToTolerateInSeconds();
                    
                    if (secondsFromCandleClose > tolerateSeconds)
                    {
                        Console.WriteLine("\t Tolere edilen sureyi gectiği icin islem almiyorum..");
                        continue;
                    }
                }

                var screening = _buildScreening(alert.ScreeningId, candles.Candles, alertMetaValuesDictionary);
                if (screening == null)
                {
                    //TODO: birden fazla symbol hata verirse ne olur, alertlog'da item olmasi gerekiyor
                    alertLog.EndDate = DateTime.UtcNow;
                    alertLog.Values = "Exception:ScreeningNull";

                    var alertLogUpdate1 = alertLogData.Update(alertLog);

                    continue;
                }

                var result = screening.Execute();
                if (result.HasAlert)
                    _telegramHelper._trySendAlertToTelegram(symbol, alert.Interval, result.Message);

                //TODO: birden fazla symbol'de buranın kafasi karisir
                //her dongude uzerine yazar
                alertLog.Values = string.Join('|', result.Values.Select(x => $"{x.Key}:{x.Value}"));
            }

            alertLog.IsSucceed = true;
            alertLog.EndDate = DateTime.UtcNow;

            var alertLogUpdate2 = alertLogData.Update(alertLog);
        }

        private IScreening _buildScreening(int screeningId, IEnumerable<Binance.Net.Interfaces.IBinanceKline> candles
            , Dictionary<string, object> alertMetaValuesDictionary)
        {
            var screening = screeningResolver.GetScreeningById(screeningId
                , candles.Select(x => new Strategies.Dto.Candle(x.CloseTime, x.OpenPrice, x.ClosePrice, x.LowPrice, x.HighPrice, x.Volume, x.OpenTime)).ToList()
                , alertMetaValuesDictionary);

            return screening;
        }

        /// <summary>
        /// Local log dosyasindan ya da binance uzerinden bana mum verilerini getir
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="binanceHelper"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private JobBinanceCandlesDto _getCandles(Alert alert, BinanceHelper binanceHelper, string symbol, KlineInterval interval)
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
            var logFileName = $"_candles/alert_{alert.Id}_{alert.CandleLimit}_{symbol}_{(int)alert.Interval}.txt";
            bool writeLogAgain = false;
            if (!System.IO.File.Exists(logFileName))
                writeLogAgain = true;

            //Dosya var ancak guncel mi bakalim
            if (System.IO.File.Exists(logFileName))
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
                candlesFromExchange = binanceHelper.GetCandles(symbol, interval: interval, limit: alert.CandleLimit + 1);
                if (candlesFromExchange != null)
                    candlesFromExchange.WriteToLogFile(logFileName);
            }

            return candlesFromExchange;
        }
    }
}
