using System;
using System.Collections.Generic;
using IrcUserAdmin.ConfigSettings;

namespace IrcUserAdmin.Slaves
{
    public class SlaveExecutor : ISlaveExecutor
    {
        private readonly IBotConfig _botConfig;
        private readonly ExecutionContextFactory _contextFactory;
        private readonly IList<int> _nhibernateFactoryKeys;
        public SlaveExecutor(IBotConfig botConfig, ExecutionContextFactory contextFactory)
        {
            if (botConfig == null) throw new ArgumentNullException("botConfig");
            if (contextFactory == null) throw new ArgumentNullException("contextFactory");
            _botConfig = botConfig;
            _contextFactory = contextFactory;
            _nhibernateFactoryKeys = new List<int>();
            InitNhibernateSlaves();
        }

        private void InitNhibernateSlaves()
        {
            for (var index = 0; index < _botConfig.Settings.NHSlaves.NHSlave.Count; index++)
            {
                _nhibernateFactoryKeys.Add(index);
            }
        }

        public void ExecuteCommandsOnSlaves(IList<ISlaveCommand> slaveCommands)
        {
            foreach (var key in _nhibernateFactoryKeys)
            {
                var key2 = key + 1;
                var context = _contextFactory.GetExcuteSlaveCommandsContext();
                 context.Execute(slaveCommands, key2);
            }
        }
    }
}