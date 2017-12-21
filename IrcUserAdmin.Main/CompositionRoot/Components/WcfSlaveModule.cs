using System;
using Autofac;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.SlavePersistance;
using IrcUserAdmin.WCFServiceReference;

namespace IrcUserAdmin.CompositionRoot.Components
{
    public class WcfSlaveModule : Module
    {
        private readonly IBotConfig _config;

        public WcfSlaveModule(IBotConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            _config = config;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            int initialCount = 0;
            if (_config.Settings.NHSlaves != null && _config.Settings.NHSlaves.NHSlave != null)
            {
                //continue with the count from nhibernate slaves:
                initialCount =  _config.Settings.NHSlaves.NHSlave.Count;
            }
            if (_config.Settings.SoapSlaves != null)
            {
                for (int i = initialCount; i < _config.Settings.SoapSlaves.Count; i++)
                {
                    var wcfSlave = _config.Settings.SoapSlaves[i];
                    builder.RegisterType<WcfServiceWrapper>().Keyed<ISlavePersistence>(i).InstancePerLifetimeScope()
                        .WithParameter("adress", wcfSlave.Address);
                }
            }
        }
    }
}