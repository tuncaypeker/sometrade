using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using SomeTrade.Data;
using SomeTrade.Infrastructure.Interfaces;
using SomeTrade.Infrastructure.Logging.DummyLog;
using SomeTrade.Jobs.Bot.Quartz.Base;
using SomeTrade.Jobs.Quartz;
using SomeTrade.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SomeTrade.Jobs.Bot.Quartz.Alert
{
    /// <summary>
    /// </summary>
    class Program
    {
        static ServiceProvider serviceProvider;
        static List<Model.Symbol> symbolsFromDb;
        static SymbolData symbolData;

        static void Main(string[] args)
        {
            Console.Title = "[ALERT] Robotu Quartz ile Çalışıyor....";

            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
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

              .AddTransient<Jobs.Quartz.AlertJob>()
              .AddTransient<Jobs.Quartz.AlertQueueJob>()

              .AddTransient<SymbolDataHelpers>()

              .AddScoped(x => new DataContext(configuration["DatabaseSettings:ConnectionString"]))
              .AddDbContext<DataContext>(ServiceLifetime.Scoped)
              .AddTransient(typeof(ILogger<>), typeof(Logger<>))
              .BuildServiceProvider();

            symbolData = serviceProvider.GetService<SymbolData>();

            var alertData = serviceProvider.GetService<AlertData>();
            var alertSymbolPairData = serviceProvider.GetService<AlertSymbolPairData>();

            var symbolDataHelpers = serviceProvider.GetService<SymbolDataHelpers>();

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
                var symbols = symbolDataHelpers.GenerateSymbols(symbolsFromDb, alert.ExchangeId, symbolPairIds);

                jobAlert.JobDataMap["symbols"] = symbols.ToArray();
                jobAlert.JobDataMap["alertId"] = alert.Id;

                var cronExpression = Jobs.Quartz.CronExpressionHelper.GetExpressionBySecond(alert.IntervalInSeconds);
                ITrigger triggerAlert = TriggerBuilder.Create()
                  .WithIdentity($"alertTrigger-{alert.Id}", "groupAlerts")
                  .StartNow()
                  .WithCronSchedule(cronExpression)
                  .Build();

                scheduler.ScheduleJob(jobAlert, triggerAlert);
            }

            Console.ReadLine();
        }

    }
}
