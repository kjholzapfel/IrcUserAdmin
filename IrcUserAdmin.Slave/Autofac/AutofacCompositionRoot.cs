using Autofac;
using IrcUserAdmin.Slave.KeyListener;
using IrcUserAdmin.Slave.Service;

namespace IrcUserAdmin.Slave.Autofac
{
    public static class AutofacCompositionRoot
    {
        public static void InitAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<WcfService>().As<IWcfService>().SingleInstance();
            builder.RegisterType<KeyboardListener>().As<IKeyboardListener>().SingleInstance();
            builder.RegisterType<UserAdmin>().As<IUserAdmin>().SingleInstance();
            builder.RegisterModule(new NHibernateComponentModule());
            using (IContainer container = builder.Build())
            {
                using (ILifetimeScope lifetimeScope = container.BeginLifetimeScope())
                {
                    var service = lifetimeScope.Resolve<IWcfService>();
                    service.Run();
                    var listener = lifetimeScope.Resolve<IKeyboardListener>();
                    listener.Start();
                }
            }
        }
    }
}