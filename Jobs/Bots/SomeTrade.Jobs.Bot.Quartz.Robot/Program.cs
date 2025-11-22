using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using SomeTrade.Data;
using SomeTrade.Infrastructure.Interfaces;
using SomeTrade.Infrastructure.Logging.DummyLog;
using SomeTrade.Jobs.Bot.Quartz.Base;
using SomeTrade.Jobs.Quartz;
using SomeTrade.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace SomeTrade.Jobs.Bot.Quartz.Robot
{
	/// <summary>
	/// burada 2 tür robot aynı anda calisiyor, bunun yerine bunlari ayirmayi dusunuyoruz ve surekli calismalarini bekliyoruz
	/// 2 farkli robot türü icin, 2 farklı application yapilabilir
	/// bunlarin ortak kullandiklari yerleri merkeiz bir yere alabiliriz
	/// </summary>
	class Program
	{
		static ServiceProvider serviceProvider;
		static List<Model.Symbol> symbolsFromDb;
		static SymbolData symbolData;

		static void Main(string[] args)
		{
			Console.Title = "[ROBOT] Robotu Quartz ile Çalışıyor....";

			string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
				//.AddJsonFile($"appsettings.{envName}.json", optional: true)
				.Build();

			serviceProvider = new ServiceCollection()
			  .AddOptions()
			  .AddSingleton<SymbolPairData>()
			  .AddSingleton<SymbolData>()
			  .AddSingleton<JobEditQueueData>()

			  .AddSingleton<IStrategyResolver, StrategyResolver>()
			  .AddSingleton<StrategyMetaData>()

			  .AddSingleton<RobotData>()
			  .AddSingleton<RobotMetaValueData>()
			  .AddSingleton<RobotSymbolPairData>()

			  .AddSingleton<RobotPositionData>()
			  .AddSingleton<RobotPositionCalculationData>()
			  .AddSingleton<RobotPositionCandleData>()
			  .AddSingleton<RobotPositionPnlData>()
			  .AddSingleton<RobotPositionStopLogData>()

			  .AddSingleton<RobotExecutionData>()
			  .AddSingleton<RobotExecutionCalculationData>()
			  .AddSingleton<RobotExecutionCandleData>()

			  .AddTransient<SomeTrade.Jobs.Quartz.RobotJob>()
			  .AddTransient<SomeTrade.Jobs.Quartz.RobotQueueJob>()
			  .AddTransient<SomeTrade.Jobs.Quartz.StopJob>()

			  .AddTransient<SymbolDataHelpers>()

			  .AddScoped<DataContext>(x => new DataContext(configuration["DatabaseSettings:ConnectionString"]))
			  .AddDbContext<DataContext>(ServiceLifetime.Scoped)
			  .AddTransient(typeof(ILogger<>), typeof(Logger<>))
			  .BuildServiceProvider();

			symbolData = serviceProvider.GetService<SymbolData>();

            var robotData = serviceProvider.GetService<RobotData>();
			var robotSymbolPairData = serviceProvider.GetService<RobotSymbolPairData>();

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
			IJobDetail jobQueueu = JobBuilder.Create<SomeTrade.Jobs.Quartz.RobotQueueJob>()
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
			//Stop Job'ı
			IJobDetail jobStop = JobBuilder.Create<SomeTrade.Jobs.Quartz.StopJob>()
				.WithIdentity($"stopJob", "stopJobGroup")
				.Build();

			ITrigger triggerStop = TriggerBuilder.Create()
				.WithIdentity("stopTrigger", "stopJobGroup")
				.StartNow()
				.WithCronSchedule("1/10 * * * * ?") //her 10sn nin 3. snsinde
				.Build();

			scheduler.ScheduleJob(jobStop, triggerStop);
			Console.WriteLine("Stop Job ayaga kaldırıldı...");

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
				var symbols = symbolDataHelpers.GenerateSymbols(symbolsFromDb, robot.ExchangeId, symbolPairIds);
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

	}
}
