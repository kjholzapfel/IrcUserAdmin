using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class DelUser : AbstractIrcCommand<DelUserArguments>
    {
        private readonly IPersistance _persistance;

        public DelUser(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.DelUser; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[DelUserArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[DelUserArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.DeleteUser(Mapping[DelUserArguments.Username]);
            Messages.Add(string.Format("User {0} succesfully deleted.", Mapping[DelUserArguments.Username]));
        }
    }

    public enum DelUserArguments
    {
        Username
    }
}