using Autofac;
using IrcUserAdmin.CompositionRoot.Components;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.KeyListener;
using IrcUserAdmin.Quartz;

namespace IrcUserAdmin.CompositionRoot
{
    public static class AutofacCompositionRoot
    {
        public static void InitAutofac()
        {
            IBotConfig botconfig = new BotConfig();
            var builder = new ContainerBuilder();
            builder.RegisterModule<QuartzModule>();
            builder.RegisterModule<RuntimeModule>();
            builder.RegisterModule<ConfigModule>();
            builder.RegisterModule<IrcModule>();
            builder.RegisterModule(new NHibernateComponentModule(botconfig));
            builder.RegisterModule(new NhibernateSlaveModule(botconfig));
            using (IContainer container = builder.Build())
            {
                using (ILifetimeScope lifetimeScope = container.BeginLifetimeScope())
                {
                    var runtime = lifetimeScope.Resolve<IBotRuntime>();
                    runtime.Start();
                    var scheduler = lifetimeScope.Resolve<IQuartzScheduler>();
                    scheduler.ScheduleDailyJob();
                    var keyboardListener = lifetimeScope.Resolve<IKeyboardListener>();
                    keyboardListener.Start();
                }
            }
        }
    }
}