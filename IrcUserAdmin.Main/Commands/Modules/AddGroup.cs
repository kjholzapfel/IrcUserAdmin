using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddGroup : AbstractIrcCommand<AddGroupArguments>
    {
        private readonly IPersistance _persistance;

        public AddGroup(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AddGroup; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (_persistance.GroupExists(Mapping[AddGroupArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} already exists.", Mapping[AddGroupArguments.Group]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.AddGroup(Mapping[AddGroupArguments.Group]);
            Messages.Add(string.Format("Group {0} succefully created", Mapping[AddGroupArguments.Group]));
        }
    }

    public enum AddGroupArguments
    {
        Group = 0,
    }
}