using Autofac;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.KeyListener;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class RuntimeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BotRuntime>().As<IBotRuntime>().SingleInstance();
            builder.RegisterType<KeyboardListener>().As<IKeyboardListener>();
        }
    }
}