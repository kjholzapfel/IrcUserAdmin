using System;
using IrcUserAdmin.Commands;
using IrcUserAdmin.IrcBot;
using Quartz;

namespace IrcUserAdmin.Quartz
{
    public class DailyJob : IJob
    {
        private readonly IIrcCommands _ircCommands;

        public DailyJob(IIrcCommands ircCommands)
        {
            if (ircCommands == null) throw new ArgumentNullException("ircCommands");
            _ircCommands = ircCommands;
        }

        public void Execute(IJobExecutionContext context)
        {
            var commandContext = new IrcCommandContext { Command = CommandName.IrcSecuritySweep, MessageOrigin = MessageOrigin.Quartz};
            _ircCommands.ExecuteIrcCommand(commandContext);
        }
    }
}
