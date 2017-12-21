using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddUserToGroup : AbstractIrcCommand<AddUserToGroupArguments>
    {
        private readonly IPersistance _persistance;

        public AddUserToGroup(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AddUserToGroup; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[AddUserToGroupArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[AddUserToGroupArguments.Username]));
                return false;
            }
            if (!_persistance.GroupExists(Mapping[AddUserToGroupArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists.", Mapping[AddUserToGroupArguments.Group]));
                return false;
            }
            if (_persistance.UserExistsInGroup(Mapping[AddUserToGroupArguments.Username], Mapping[AddUserToGroupArguments.Group]))
            {
                Messages.Add(string.Format("User {0} already added to {1}.", Mapping[AddUserToGroupArguments.Username], Mapping[AddUserToGroupArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.AddUserToGroup(Mapping[AddUserToGroupArguments.Username], Mapping[AddUserToGroupArguments.Group]);
            Messages.Add(string.Format("User {0} added to {1}.", Mapping[AddUserToGroupArguments.Username], Mapping[AddUserToGroupArguments.Group]));
        }
    }

    public enum AddUserToGroupArguments
    {
        Username,
        Group
    }
}