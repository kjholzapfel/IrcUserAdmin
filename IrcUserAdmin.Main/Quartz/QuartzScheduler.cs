using System;
using System.Collections.Generic;
using Autofac;
using Quartz;
using Quartz.Impl;

namespace IrcUserAdmin.Quartz
{
    public class QuartzScheduler : IQuartzScheduler
    {
        private readonly ILifetimeScope _scope;
        private readonly Dictionary<QuartzJobs, IJobDetail> _jobDetails;
        private IScheduler _scheduler;

        public QuartzScheduler(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
            _jobDetails = new Dictionary<QuartzJobs, IJobDetail>();
            GetQuartzScheduler();
        }

        public void ScheduleDailyJob()
        {
            IJobDetail job = JobBuilder.Create<DailyJob>().WithIdentity(QuartzJobs.DailyJob.ToString()).Build();
            _jobDetails.Add(QuartzJobs.DailyJob, job);
            ITrigger trigger =
                TriggerBuilder.Create()
                                .WithIdentity(QuartzJobs.DailyJob.ToString())
                                .WithCronSchedule("0 0 0 * * ?")
                                .Build();
            _scheduler.ScheduleJob(job, trigger);
        }

        public void FireTrigger(QuartzJobs job)
        {
            IJobDetail jobDetail;
            if (_jobDetails.TryGetValue(job, out jobDetail))
            {
                try
                {
                    _scheduler.TriggerJob(jobDetail.Key);
                }
                catch (Exception exception)
                {
                    
                }
             
            }
        }

        private void GetQuartzScheduler()
        {
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.JobFactory = new AutofacJobFactory(_scope);
            _scheduler.Start();
        }

        public void StopScheduler()
        {
            _scheduler.Shutdown(false);
        }
    }

    public enum QuartzJobs
    {
        DailyJob
    }
}