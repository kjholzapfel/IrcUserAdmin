using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddUserAsAdmin : AbstractIrcCommand<AddUserAsAdminArguments>
    {
        private readonly IPersistance _persistance;

        public AddUserAsAdmin(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AdduserAdmin; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[AddUserAsAdminArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[AddUserAsAdminArguments.Username]));
                return false;
            }
            if (!_persistance.GroupExists(Mapping[AddUserAsAdminArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exist.", Mapping[AddUserAsAdminArguments.Group]));
                return false;
            }
            if (_persistance.UserIsAdminOfGroup(Mapping[AddUserAsAdminArguments.Username], Mapping[AddUserAsAdminArguments.Group]))
            {
                Messages.Add(string.Format("User {0} already added to {1}.", Mapping[AddUserAsAdminArguments.Username], Mapping[AddUserAsAdminArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.AddUserAsAdminToGroup(Mapping[AddUserAsAdminArguments.Username], Mapping[AddUserAsAdminArguments.Group]);
            Messages.Add(string.Format("User {0} added as group admin to {1}.", Mapping[AddUserAsAdminArguments.Username], Mapping[AddUserAsAdminArguments.Group]));
        }
    }

    public enum AddUserAsAdminArguments
    {
        Username = 0,
        Group = 1
    }
}