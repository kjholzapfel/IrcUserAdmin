using Autofac;
using IrcUserAdmin.ConfigSettings;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BotConfig>().As<IBotConfig>().SingleInstance();
            builder.RegisterType<DbBootstrap>().As<IDbBootstrap>();
            builder.RegisterType<DbBootStrapAddUsers>().As<IDbBootstrapAddUsers>();
        }
    }
}