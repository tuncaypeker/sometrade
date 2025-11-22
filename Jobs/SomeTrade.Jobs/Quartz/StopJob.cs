using Quartz;
using SomeTrade.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Quartz
{
	public class StopJob : IJob
	{
		SomeTrade.Jobs.StopJob stopJob;

		public StopJob(RobotData robotData
		   , Strategies.IStrategyResolver strategyResolver, RobotPositionData robotPositionData
		   , RobotPositionCalculationData robotPositionCalculationData, RobotPositionCandleData robotPositionCandleData, RobotMetaValueData robotMetaValueData
		   , StrategyMetaData strategyMetaData, SymbolData symbolData
		   , RobotPositionPnlData robotPositionPnlData
			, RobotExecutionData robotExecutionData, RobotExecutionCandleData robotExecutionCandleData, RobotExecutionCalculationData robotExecutionCalculationData
			, RobotSymbolPairData robotSymbolPairData, SymbolPairData symbolPairData, RobotPositionStopLogData robotPositionStopLogData)
		{
			stopJob = new SomeTrade.Jobs.StopJob(robotData, strategyResolver, robotPositionData, robotPositionCalculationData
				, robotPositionCandleData, robotMetaValueData, strategyMetaData, symbolData, robotPositionPnlData, robotExecutionData, robotExecutionCandleData, robotExecutionCalculationData
				, robotSymbolPairData,symbolPairData,robotPositionStopLogData);
		}

		public async Task Execute(IJobExecutionContext context)
		{
			stopJob.Execute();
		}
	}
}
