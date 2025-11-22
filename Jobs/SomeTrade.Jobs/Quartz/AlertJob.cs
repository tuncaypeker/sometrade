using Quartz;
using System.Linq;
using System.Threading.Tasks;

namespace SomeTrade.Jobs.Quartz
{
    public class AlertJob : IJob
    {
        SomeTrade.Jobs.AlertJob alertJob;

        public AlertJob(Data.AlertData alertData, Data.ScreeningMetaData screeningMetaData, Data.AlertMetaValueData alertMetaValueData
            , Data.AlertSymbolPairData alertSymbolPairData, Strategies.IScreeningResolver screeningResolver
            , Data.AlertLogData alertLogData)
        {
            alertJob = new Jobs.AlertJob(alertData, screeningMetaData, alertMetaValueData, screeningResolver, alertLogData);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int alertId = dataMap.GetInt("alertId");
            var symbols = ((string[])dataMap["symbols"]).ToList();

            alertJob.Execute(alertId, symbols);
        }
    }
}
