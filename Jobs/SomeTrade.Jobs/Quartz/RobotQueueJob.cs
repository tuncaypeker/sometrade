using Quartz;
using SomeTrade.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Quartz
{
    public class RobotQueueJob : IJob
    {
        SomeTrade.Data.JobEditQueueData jobEditQueueData;
        SomeTrade.Data.SymbolPairData symbolPairData;
        SomeTrade.Data.SymbolData symbolData;

        SomeTrade.Data.RobotData robotData;
        SomeTrade.Data.RobotSymbolPairData robotSymbolPairData;
        SymbolDataHelpers symbolDataHelpers;

        public RobotQueueJob(JobEditQueueData jobEditQueueData, SymbolPairData symbolPairData
            , SymbolData symbolData, RobotData robotData, RobotSymbolPairData robotSymbolPairData, SymbolDataHelpers symbolDataHelpers)
        {
            this.jobEditQueueData = jobEditQueueData;
            this.symbolPairData = symbolPairData;
            this.symbolData = symbolData;

            this.robotData = robotData;
            this.robotSymbolPairData = robotSymbolPairData;
            this.symbolDataHelpers = symbolDataHelpers;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var scheduler = ((IScheduler)dataMap["scheduler"]);

            //acaba bekleyen queue var mi074
            var queues = jobEditQueueData.GetBy(x => !x.HasProcessed && x.JobType == "Robot");

            if (queues == null)
            {
                //TODO: burada 3-4 kere null gelirse telegram mesj atmalı
                return;
            }

            foreach (var queue in queues)
            {
                var robot = robotData.GetByKey(queue.JobId);

                var jobIdendity = $"robotJob-{queue.JobId}";
                var jobGroupIdentiy = "groupRobots";
                var triggerIdendity = $"robotTrigger-{queue.JobId}";

                //eger robot silindi ise, jobqueue ekleniyor ancak db'den silinmis oluyor
                //bu yuzden eger, robot null gelirse her durumda job'u unschedule edelim ve
                //job tmmlandi olarak isaretleyelim
                if (robot == null)
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

                    IJobDetail jobRobot = JobBuilder.Create<SomeTrade.Jobs.Quartz.RobotJob>()
                       .WithIdentity(jobIdendity, jobGroupIdentiy)
                       .Build();

                    //# Find and Build Symbols for loop
                    var symbolPairIds = robotSymbolPairData
                        .GetBy(x => x.RobotId == robot.Id)
                        .Select(x => x.SymbolPairId).ToArray();
                    List<Model.Symbol> symbolsFromDb = symbolData.GetAll();
                    var symbols = symbolDataHelpers.GenerateSymbols(symbolsFromDb, robot.ExchangeId, symbolPairIds);
                    jobRobot.JobDataMap["symbols"] = symbols.ToArray();
                    jobRobot.JobDataMap["robotId"] = robot.Id;

                    var cronExpression = Jobs.Quartz.CronExpressionHelper.GetExpressionBySecond(robot.IntervalInSeconds);
                    ITrigger trigger = TriggerBuilder.Create()
                      .WithIdentity(triggerIdendity, jobGroupIdentiy)
                      .StartNow()
                      .WithCronSchedule(cronExpression)
                      .Build();

                    scheduler.ScheduleJob(jobRobot, trigger).Wait();

                    _writeColorLine($"{DateTime.UtcNow.ToString("dd:mm:ss")} Al-Sat=>{robot.Name} Upsert Edildi", ConsoleColor.Green);
                    _writeColorLine($"\t{queue.Reason}", ConsoleColor.White);
                }
                else if (queue.JobAction == "Delete")
                {
                    //eger bu identity ile varsa bunu kaldirmaliyiz
                    scheduler.UnscheduleJob(new TriggerKey(triggerIdendity, jobGroupIdentiy)).Wait();
                    scheduler.DeleteJob(new JobKey(jobIdendity, jobGroupIdentiy)).Wait();

                    _writeColorLine($"{DateTime.UtcNow.ToString("dd:mm:ss")} Al-Sat=>{robot.Name} Silindi", ConsoleColor.Red);
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
