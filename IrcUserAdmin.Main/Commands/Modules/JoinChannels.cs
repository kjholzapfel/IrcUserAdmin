using System;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class JoinChannels : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;

        public JoinChannels(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.JoinChannels; } }
        public override CommandType CommandType { get { return CommandType.UserCommand; } }

        public override void Execute()
        {
            User user = _persistance.GetUserInfo(Context.Ident);
            foreach (var group in user.Groups)
            {
                if (!string.IsNullOrEmpty(group.Channel))
                {
                    var sajoinCommand = SajoinOperCommmand.SetSaJoin(Context.User, group.Channel);
                    OperCommands.Add(sajoinCommand);
                    if (group.GroupAdmins.Contains(user))
                    {
                        var saModeCommand = SaModeOpsOperCommmand.SetSamode(Context.User, group.Channel);
                        OperCommands.Add(saModeCommand);
                    }
                }
            }
        }
    }
}