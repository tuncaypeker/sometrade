using Quartz;
using SomeTrade.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Quartz
{
	public class RobotJob : IJob
	{
		SomeTrade.Jobs.RobotJob robotJob;

		public RobotJob(RobotData robotData
		   , Strategies.IStrategyResolver strategyResolver, RobotPositionData robotPositionData
		   , RobotPositionCalculationData robotPositionCalculationData, RobotPositionCandleData robotPositionCandleData, RobotMetaValueData robotMetaValueData
		   , StrategyMetaData strategyMetaData, SymbolData symbolData
		   , RobotPositionPnlData robotPositionPnlData
			, RobotExecutionData robotExecutionData, RobotExecutionCandleData robotExecutionCandleData, RobotExecutionCalculationData robotExecutionCalculationData
			, RobotSymbolPairData robotSymbolPairData, SymbolPairData symbolPairData, RobotPositionStopLogData robotPositionStopLogData)
		{
			robotJob = new SomeTrade.Jobs.RobotJob(robotData, strategyResolver, robotPositionData, robotPositionCalculationData
				, robotPositionCandleData, robotMetaValueData, strategyMetaData, symbolData, robotPositionPnlData, robotExecutionData, robotExecutionCandleData, robotExecutionCalculationData
				, robotSymbolPairData,symbolPairData,robotPositionStopLogData);
		}

		public async Task Execute(IJobExecutionContext context)
		{
			JobDataMap dataMap = context.JobDetail.JobDataMap;
			int robotId = dataMap.GetInt("robotId");
			var symbols = ((string[])dataMap["symbols"]).ToList();

			robotJob.Execute(robotId, symbols);
		}
	}
}
