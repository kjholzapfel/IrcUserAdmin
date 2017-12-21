using System;
using System.Collections.Generic;
using Autofac;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public class ExcuteSlaveCommandsContext : ExecutionContext<ISlavePersistence, int>
    {
        private IEnumerable<ISlaveCommand> _slaveCommands;

        public ExcuteSlaveCommandsContext(ILifetimeScope scope) : base(scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
        }

        public void Execute(IEnumerable<ISlaveCommand> slaveCommands, int key)
        {
            Key = key;
            _slaveCommands = slaveCommands;
            StartThread();
        }

        protected override void InnerExecute()
        {
            foreach (var slaveCommand in _slaveCommands)
            {
                slaveCommand.Execute(Resolved);
            }
            Resolved.Persist();
        }
    }
}