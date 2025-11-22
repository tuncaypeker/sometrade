using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using SomeTrade.Data;
using SomeTrade.Infrastructure.Interfaces;
using SomeTrade.Infrastructure.Logging.DummyLog;
using SomeTrade.Jobs.Bot.Quartz.Base;
using SomeTrade.Jobs.Quartz;
using SomeTrade.Socket;
using SomeTrade.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs.Bot.Quartz
{
    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Bu deprecated, bunun yerine iki farklı uygulama olarak calistiriyoruz
    /// burada 3 tür robot aynı anda calisiyor, bunun yerine bunlari ayirmayi dusunuyoruz ve surekli calismalarini bekliyoruz
    /// 3 farkli robot türü icin, 3 farklı application yapilabilir
    /// bunlarin ortak kullandiklari yerleri merkeiz bir yere alabiliriz
    /// </summary>
    class Program
    {
        static ServiceProvider serviceProvider;
        static List<Model.Symbol> symbolsFromDb;
        static SymbolPairData symbolPairData;
        static SymbolData symbolData;

        static void Main(string[] args)
        {
            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                //.AddJsonFile($"appsettings.{envName}.json", optional: true)
                .Build();

            serviceProvider = new ServiceCollection()
              .AddOptions()
              .AddSingleton<SymbolPairData>()
              .AddSingleton<SymbolData>()
              .AddSingleton<JobEditQueueData>()

              .AddSingleton<IScreeningResolver, ScreeningResolver>()
              .AddSingleton<ScreeningMetaData>()
              .AddSingleton<AlertData>()
              .AddSingleton<AlertMetaValueData>()
              .AddSingleton<AlertSymbolPairData>()
              .AddSingleton<AlertLogData>()

              .AddSingleton<IStrategyResolver, StrategyResolver>()
              .AddSingleton<StrategyMetaData>()

              .AddSingleton<RobotData>()

              .AddSingleton<RobotExecutionData>()
              .AddSingleton<RobotExecutionCalculationData>()
              .AddSingleton<RobotExecutionCandleData>()

              .AddSingleton<RobotPositionData>()
              .AddSingleton<RobotPositionCalculationData>()
              .AddSingleton<RobotPositionCandleData>()
              .AddSingleton<RobotMetaValueData>()
              .AddSingleton<RobotSymbolPairData>()

              .AddSingleton<TradeHub>()
              .AddSingleton<RobotHub>()
              .AddTransient<SomeTrade.Jobs.Quartz.AlertJob>()
              .AddTransient<SomeTrade.Jobs.Quartz.AlertQueueJob>()
              .AddScoped<DataContext>(x => new DataContext(configuration["DatabaseSettings:ConnectionString"]))
              .AddDbContext<DataContext>(ServiceLifetime.Scoped)
              .AddTransient(typeof(ILogger<>), typeof(Logger<>))
              .BuildServiceProvider();

            symbolPairData = serviceProvider.GetService<SymbolPairData>();
            symbolData = serviceProvider.GetService<SymbolData>();

            var alertData = serviceProvider.GetService<AlertData>();
            var alertSymbolPairData = serviceProvider.GetService<AlertSymbolPairData>();

            var robotData = serviceProvider.GetService<RobotData>();
            var robotSymbolPairData = serviceProvider.GetService<RobotSymbolPairData>();

            //get all symbols
            symbolsFromDb = symbolData.GetAll();

            CustomJobFactory jobFactory = new CustomJobFactory(serviceProvider);
            var schedulerFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.JobFactory = jobFactory;
            scheduler.Start().Wait();

            //#######################
            //Queue Job'ı job'lar uzerinde web ekraninda degisiklik yapildi ise bunu job'a yansitir
            IJobDetail jobQueueu = JobBuilder.Create<SomeTrade.Jobs.Quartz.AlertQueueJob>()
                .WithIdentity($"queueJob", "queueJobGroup")
                .Build();

            jobQueueu.JobDataMap["scheduler"] = scheduler;
            
            ITrigger triggerQueueu = TriggerBuilder.Create()
                .WithIdentity("queueTrigger", "queueJobGroup")
                .WithCronSchedule("3/30 * * * * ?") //her 30sn nin 3. snsinde
                .StartAt(DateTime.UtcNow)
                .WithPriority(1)
                .Build();

            scheduler.ScheduleJob(jobQueueu, triggerQueueu);
            Console.WriteLine("Queue Job ayaga kaldırıldı...");

            //#######################
            //Alert Job'lari anlik durumda veritabaninda aktif olan taramalari schedule eder
            var alerts = alertData.GetBy(x => x.IsActive);
            foreach (var alert in alerts)
            {
                IJobDetail jobAlert = JobBuilder.Create<SomeTrade.Jobs.Quartz.AlertJob>()
                .WithIdentity($"alertJob-{alert.Id}", "groupAlerts")
                .Build();

                 //# Find and Build Symbols for loop
                var symbolPairIds = alertSymbolPairData
                    .GetBy(x => x.AlertId == alert.Id)
                    .Select(x => x.SymbolPairId).ToArray();
                var symbols = _generateSymbols(alert.ExchangeId, symbolPairIds);
                jobAlert.JobDataMap["symbols"] = symbols.ToArray();
                jobAlert.JobDataMap["alertId"] = alert.Id;

                var cronExpression = CronExpressionHelper.GetExpressionBySecond(alert.IntervalInSeconds);
                ITrigger triggerAlert = TriggerBuilder.Create()
                  .WithIdentity($"alertTrigger-{alert.Id}", "groupAlerts")
                  .StartNow()
                  .WithCronSchedule(cronExpression)
                  .Build();

                scheduler.ScheduleJob(jobAlert, triggerAlert);
            }

            //#######################
            //Robot Job'lari anlik durumda veritabaninda aktif olan testleri schedule eder
            var robots = robotData.GetBy(x => x.IsActive);
            foreach (var robot in robots)
            {
                IJobDetail job = JobBuilder.Create<SomeTrade.Jobs.Quartz.RobotJob>()
                .WithIdentity($"robotJob-{robot.Id}", "groupRobots")
                .Build();

                //# Find and Build Symbols for loop
                var symbolPairIds = robotSymbolPairData
                    .GetBy(x => x.RobotId == robot.Id)
                    .Select(x => x.SymbolPairId).ToArray();
                var symbols = _generateSymbols(robot.ExchangeId, symbolPairIds);
                job.JobDataMap["symbols"] = symbols.ToArray();
                job.JobDataMap["robotId"] = robot.Id;

                var cronExpression = Jobs.Quartz.CronExpressionHelper.GetExpressionBySecond(robot.IntervalInSeconds);
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity($"robotTrigger-{robot.Id}", "groupRobots")
                  .StartNow()
                  .WithCronSchedule(cronExpression)
                  .Build();

                scheduler.ScheduleJob(job, trigger);
            }

            Console.ReadLine();
        }

        private static List<string> _generateSymbols(int exchangeId, int[] livetestSymbolPairIds)
        {
            var symbols = new List<string>();
            var symbolPairs = symbolPairData.GetBy(x => x.ExchangeId == exchangeId && livetestSymbolPairIds.Contains(x.Id));

            foreach (var symbolPair in symbolPairs)
            {
                var fromSymbol = symbolsFromDb.FirstOrDefault(x => x.Id == symbolPair.FromSymbolId);
                var toSymbol = symbolsFromDb.FirstOrDefault(x => x.Id == symbolPair.ToSymbolId);

                if (fromSymbol == null || toSymbol == null)
                    continue;

                symbols.Add($"{toSymbol.Short}{fromSymbol.Short}");
            }

            return symbols;
        }
    }
}
