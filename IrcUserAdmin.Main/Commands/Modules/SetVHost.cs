using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class SetVHost : AbstractIrcCommand<SetVHostArguments>
    {
        private readonly IPersistance _persistance;

        public SetVHost(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.SetVHost; } }
        public override CommandType CommandType { get { return CommandType.OperCommand; } }


        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[SetVHostArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[SetVHostArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.SetVHost(Mapping[SetVHostArguments.Username], Mapping[SetVHostArguments.Vhost]);
            Messages.Add(string.Format("Setting new vhost {0} for user {1}.", Mapping[SetVHostArguments.Username], Mapping[SetVHostArguments.Vhost]));
        }
    }

    public enum SetVHostArguments
    {
        Username,
        Vhost,
    }
}