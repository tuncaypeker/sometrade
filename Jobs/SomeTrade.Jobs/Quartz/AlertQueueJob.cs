using Quartz;
using SomeTrade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Quartz
{
    public class AlertQueueJob : IJob
    {
        SomeTrade.Data.JobEditQueueData jobEditQueueData;
        SomeTrade.Data.SymbolPairData symbolPairData;
        SomeTrade.Data.SymbolData symbolData;

        SomeTrade.Data.AlertData alertData;
        SomeTrade.Data.AlertSymbolPairData alertSymbolPairData;

        SymbolDataHelpers symbolDataHelpers;

        public AlertQueueJob(JobEditQueueData jobEditQueueData, SymbolPairData symbolPairData, AlertSymbolPairData alertSymbolPairData
            , AlertData alertData, SymbolData symbolData, SymbolDataHelpers symbolDataHelpers)
        {
            this.jobEditQueueData = jobEditQueueData;
            this.symbolPairData = symbolPairData;
            this.symbolData = symbolData;

            this.alertSymbolPairData = alertSymbolPairData;
            this.alertData = alertData;
            this.symbolDataHelpers = symbolDataHelpers;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var scheduler = ((IScheduler)dataMap["scheduler"]);

            //acaba bekleyen queue var mi074
            var queues = jobEditQueueData.GetBy(x => !x.HasProcessed && x.JobType == "Alert");

            if (queues == null)
            {
                //TODO: burada 3-4 kere null gelirse telegram mesj atmalı
                return;
            }

            foreach (var queue in queues)
            {
                var alert = alertData.GetByKey(queue.JobId);

                var jobIdendity = $"alertJob-{queue.JobId}";
                var jobGroupIdentiy = "groupAlerts";
                var triggerIdendity = $"alertTrigger-{queue.JobId}";

                //eger alert silindi ise, jobqueue ekleniyor ancak db'den silinmis oluyor
                //bu yuzden eger, aler null gelirse her durumda job'u unschedule edelim ve
                //job tmmlandi olarak isaretleyelim
                if (alert == null)
                {
                    //eger bu identity ile varsa bunu kaldirmaliyiz
                    scheduler.UnscheduleJob(new TriggerKey(triggerIdendity, jobGroupIdentiy)).Wait();
                    scheduler.DeleteJob(new JobKey(jobIdendity, jobGroupIdentiy)).Wait();

                    queue.Processed = DateTime.UtcNow;
                    queue.HasProcessed = true;

                    var dbResultx = jobEditQueueData.Update(queue);
                    continue;
                }

                if (queue.JobAction == "Upsert")
                {
                    //eger bu identity ile varsa bunu kaldirmaliyiz
                    scheduler.UnscheduleJob(new TriggerKey(triggerIdendity, jobGroupIdentiy)).Wait();
                    scheduler.DeleteJob(new JobKey(jobIdendity, jobGroupIdentiy)).Wait();

                    IJobDetail jobAlert = JobBuilder.Create<SomeTrade.Jobs.Quartz.AlertJob>()
                       .WithIdentity(jobIdendity, jobGroupIdentiy)
                       .Build();

                    //# Find and Build Symbols for loop
                    var symbolPairIds = alertSymbolPairData
                        .GetBy(x => x.AlertId == queue.JobId)
                        .Select(x => x.SymbolPairId).ToArray();
                    List<Model.Symbol> symbolsFromDb = symbolData.GetAll();
                    var symbols = symbolDataHelpers.GenerateSymbols(symbolsFromDb, alert.ExchangeId, symbolPairIds);
                    jobAlert.JobDataMap["symbols"] = symbols.ToArray();
                    jobAlert.JobDataMap["alertId"] = queue.JobId;

                    var cronExpression = Jobs.Quartz.CronExpressionHelper.GetExpressionBySecond(alert.IntervalInSeconds);
                    ITrigger trigger = TriggerBuilder.Create()
                      .WithIdentity(triggerIdendity, jobGroupIdentiy)
                      .StartNow()
                      .WithCronSchedule(cronExpression)
                      .Build();

                    scheduler.ScheduleJob(jobAlert, trigger).Wait();

                    _writeColorLine($"{DateTime.UtcNow.ToString("dd:mm:ss")} Alert=>{alert.Name} Upsert Edildi", ConsoleColor.Green);
                    _writeColorLine($"\t{queue.Reason}", ConsoleColor.White);
                }
                else if (queue.JobAction == "Delete")
                {
                    //eger bu identity ile varsa bunu kaldirmaliyiz
                    scheduler.UnscheduleJob(new TriggerKey(triggerIdendity, jobGroupIdentiy)).Wait();
                    scheduler.DeleteJob(new JobKey(jobIdendity, jobGroupIdentiy)).Wait();

                    _writeColorLine($"{DateTime.UtcNow.ToString("dd:mm:ss")} Alert=>{alert.Name} Silindi", ConsoleColor.Red);
                    _writeColorLine($"\t{queue.Reason}", ConsoleColor.White);
                }

                queue.Processed = DateTime.UtcNow;
                queue.HasProcessed = true;

                var dbResult = jobEditQueueData.Update(queue);
            }
        }

        private static void _writeColorLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
