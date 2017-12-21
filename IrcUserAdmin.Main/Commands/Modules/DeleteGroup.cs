using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class DeleteGroup : AbstractIrcCommand<DeleteGroupArguments>
    {
        private readonly IPersistance _persistance;

        public DeleteGroup(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.DeleteGroup; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.GroupExists(Mapping[DeleteGroupArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists.", Mapping[DeleteGroupArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.DeleteGroup(Mapping[DeleteGroupArguments.Group]);
            Messages.Add(string.Format("Group {0} succesfully deleted.", Mapping[DeleteGroupArguments.Group]));
        }
    }

    public enum DeleteGroupArguments
    {
        Group
    }
}