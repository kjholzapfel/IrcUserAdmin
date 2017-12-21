using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class RemoveUserFromGroup : AbstractIrcCommand<RemoveUserFromGroupArguments>
    {
        private readonly IPersistance _persistance;

        public RemoveUserFromGroup(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.RemoveUserFromGroup; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[RemoveUserFromGroupArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[RemoveUserFromGroupArguments.Username]));
                return false;
            }
            if (!_persistance.GroupExists(Mapping[RemoveUserFromGroupArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists.", Mapping[RemoveUserFromGroupArguments.Group]));
                return false;
            }
            if (!_persistance.UserExistsInGroup(Mapping[RemoveUserFromGroupArguments.Username], Mapping[RemoveUserFromGroupArguments.Group]))
            {
                Messages.Add(string.Format("User {0} not present in group {1}.", Mapping[RemoveUserFromGroupArguments.Username], Mapping[RemoveUserFromGroupArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.RemoveUserFromGroup(Mapping[RemoveUserFromGroupArguments.Username], Mapping[RemoveUserFromGroupArguments.Group]);
            Messages.Add(string.Format("User {0} removed from group {1}.", Mapping[RemoveUserFromGroupArguments.Username], Mapping[RemoveUserFromGroupArguments.Group]));
        }
    }

    public enum RemoveUserFromGroupArguments
    {
        Username,
        Group
    }
}