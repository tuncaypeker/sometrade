using Quartz;
using Quartz.Spi;
using System;

namespace SomeTrade.Jobs.Bot.Quartz.Base
{
    public class CustomJobFactory : IJobFactory
    {
        protected readonly IServiceProvider Container;

        public CustomJobFactory(IServiceProvider container)
        {
            Container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return Container.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
