using Autofac;
using IrcUserAdmin.Quartz;
using Quartz;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class QuartzModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IJob>();
            builder.RegisterType<DailyJob>();
            builder.RegisterType<QuartzScheduler>().As<IQuartzScheduler>().SingleInstance();
        }
    }
}