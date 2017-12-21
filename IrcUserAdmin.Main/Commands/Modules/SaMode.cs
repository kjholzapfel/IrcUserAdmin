using System;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class SaMode : AbstractIrcCommand<SaModeArguments>
    {
        public SaMode(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
        }

        public override CommandName Name { get { return CommandName.SaMode; } }
        public override CommandType CommandType { get { return CommandType.OperCommand; } }

        public override void Execute()
        {
            var operCommand = SaModeOperCommmand.SetSamode(Mapping[SaModeArguments.Nick], Mapping[SaModeArguments.Mode], Mapping[SaModeArguments.Channel]);
            OperCommands.Add(operCommand);
        }
    }

    public enum SaModeArguments
    {
        Channel = 0,
        Mode = 1,
        Nick = 2
    }
}