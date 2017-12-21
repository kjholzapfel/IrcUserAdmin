using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class RemoveUserAsAdmin : AbstractIrcCommand<RemoveUserAsAdminArguments>
    {
        private readonly IPersistance _persistance;

        public RemoveUserAsAdmin(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange)
            : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.RemoveUserAdmin; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[RemoveUserAsAdminArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[RemoveUserAsAdminArguments.Username]));
                return false;
            }
            if (!_persistance.GroupExists(Mapping[RemoveUserAsAdminArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exist.", Mapping[RemoveUserAsAdminArguments.Group]));
                return false;
            }
            if (!_persistance.UserIsAdminOfGroup(Mapping[RemoveUserAsAdminArguments.Username], Mapping[RemoveUserAsAdminArguments.Group]))
            {
                Messages.Add(string.Format("User {0} not present in group {1}.", Mapping[RemoveUserAsAdminArguments.Username], Mapping[RemoveUserAsAdminArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.RemoveUserAsAdminFromGroup(Mapping[RemoveUserAsAdminArguments.Username], Mapping[RemoveUserAsAdminArguments.Group]);
            Messages.Add(string.Format("User {0} removed from group {1} as admin..", Mapping[RemoveUserAsAdminArguments.Username], Mapping[RemoveUserAsAdminArguments.Group]));
        }
    }

    public enum RemoveUserAsAdminArguments
    {
        Username = 0,
        Group = 1
    }
}