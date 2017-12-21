using System;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class SetAutoJoin : AbstractIrcCommand<SetAutoJoinArguments>
    {
        private readonly IPersistance _persistance;

        public SetAutoJoin(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.SetAutoJoin; } }
        public override CommandType CommandType { get { return CommandType.OperCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[SetAutoJoinArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[SetAutoJoinArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            User user = _persistance.GetUserInfo(Mapping[SetAutoJoinArguments.Username]);
            if (user.AutoJoin)
            {
                _persistance.DisableAutoJoin(Mapping[SetAutoJoinArguments.Username]);
                Messages.Add(string.Format("Autojoin disabled for user {0}. Run the command again to enable.", Mapping[SetAutoJoinArguments.Username]));
            }
            else
            {
                _persistance.EnableAutoJoin(Mapping[SetAutoJoinArguments.Username]);
                Messages.Add(string.Format("Autojoin enabled for user {0}. Run the command again to disable.", Mapping[SetAutoJoinArguments.Username]));
            }
        }
    }

    public enum SetAutoJoinArguments
    {
        Username
    }
}