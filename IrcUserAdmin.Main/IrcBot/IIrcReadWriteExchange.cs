using System.Collections.Generic;
using IrcUserAdmin.Commands.OperCommands;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public interface IIrcReadWriteExchange
    {
        void Write(IEnumerable<IrcWriteContext> context);
        void ExecuteOperCommands(IEnumerable<IOperCommand> operCommands);
        IList<WhoInfo> GetWhoList();
    }
}