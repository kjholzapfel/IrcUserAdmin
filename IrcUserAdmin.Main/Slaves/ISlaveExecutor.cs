using System.Collections.Generic;

namespace IrcUserAdmin.Slaves
{
    public interface ISlaveExecutor
    {
        void ExecuteCommandsOnSlaves(IList<ISlaveCommand> slaveCommands);
    }
}