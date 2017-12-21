using System;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddGroupChannel : AbstractIrcCommand<AddGroupChannelArguments>
    {
        private readonly IPersistance _persistance;
        public AddGroupChannel(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AddGroupChannel; } }

        public override CommandType CommandType { get { return CommandType.GroupCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.GroupExists(Mapping[AddGroupChannelArguments.Group]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists.", Mapping[AddGroupChannelArguments.Group]));
                return false;
            }
            if (_persistance.GroupChannelExists(Mapping[AddGroupChannelArguments.Channel]))
            {
                Messages.Add(string.Format("Channel {0} already exists.", Mapping[AddGroupChannelArguments.Channel]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.AddChannelToGroup(Mapping[AddGroupChannelArguments.Channel], Mapping[AddGroupChannelArguments.Group]);
            Messages.Add(string.Format("Channel {0} addded to group {1}.", Mapping[AddGroupChannelArguments.Channel], Mapping[AddGroupChannelArguments.Group]));
        }
    }

    public enum AddGroupChannelArguments
    {
        Group = 0,
        Channel = 1,
    }
}