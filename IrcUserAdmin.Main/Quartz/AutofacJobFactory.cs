using System;
using System.Collections.Concurrent;
using Autofac;
using Quartz;
using Quartz.Spi;

namespace IrcUserAdmin.Quartz
{
    public class AutofacJobFactory : IJobFactory
    {
        private readonly ILifetimeScope _container;
        private readonly ConcurrentDictionary<IJob, ILifetimeScope> _lifetimeScopes;

        public AutofacJobFactory(ILifetimeScope container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
            _lifetimeScopes = new ConcurrentDictionary<IJob, ILifetimeScope>();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Type jobtype = bundle.JobDetail.JobType;
            ILifetimeScope newJobLifetimeSope = _container.BeginLifetimeScope();
            var job = (IJob)newJobLifetimeSope.Resolve(jobtype);
            string uniqueGuidString = Guid.NewGuid().ToString();
            bundle.JobDetail.JobDataMap.Add("uniqueID", uniqueGuidString);
            _lifetimeScopes.TryAdd(job, newJobLifetimeSope);
            return job;
        }

        public void ReturnJob(IJob job)
        {
            ILifetimeScope lifetimeScope;
            if (_lifetimeScopes.TryRemove(job, out lifetimeScope))
            {
                lifetimeScope.Dispose();
            }
        }
    }
}