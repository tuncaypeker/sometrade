using Quartz;
using System;
using System.Linq;

namespace SomeTrade.Jobs.Bot.Quartz.Base
{
    public static class QuartzGoodMethods
    {
        public static DateTime getNextFireTimeForJob(IScheduler scheduler, string jobName, string groupName = "")
        {
            JobKey jobKey = null;

            if (!string.IsNullOrEmpty(groupName))
            {
                jobKey = new JobKey(jobName, groupName);
            }
            else
            {
                jobKey = new JobKey(jobName);
            }

            DateTime nextFireTime = DateTime.MinValue;

            bool isJobExisting = scheduler.CheckExists(jobKey).Result;
            if (isJobExisting)
            {
                var detail = scheduler.GetJobDetail(jobKey);
                var triggers = scheduler.GetTriggersOfJob(jobKey).Result;

                var myTrigger = triggers.Where(f => f.Key.Name == "SecondTrigger").SingleOrDefault();

                if (triggers.Count > 0)
                {
                    var nextFireTimeUtc = triggers.FirstOrDefault().GetNextFireTimeUtc();
                    nextFireTime = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTimeUtc.Value.DateTime);
                }
            }

            return (nextFireTime);
        }
    }
}
