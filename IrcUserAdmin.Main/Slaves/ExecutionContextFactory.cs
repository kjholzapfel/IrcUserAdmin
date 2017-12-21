using System;
using Autofac;

namespace IrcUserAdmin.Slaves
{
    public class ExecutionContextFactory
    {
        private readonly ILifetimeScope _scope;

        public ExecutionContextFactory(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        public ExcuteSlaveCommandsContext GetExcuteSlaveCommandsContext()
        {
            var context = new ExcuteSlaveCommandsContext(_scope);
            return context;
        }
    }
}