using System;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class Connect : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;
        private string _ident;
        private string _nick;

        public Connect(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException(nameof(persistance));
            _persistance = persistance;
        }

        public override bool CheckPreconditions()
        {
            int connectStringLength = Context.MsgArray.Count;
            string hostname = Context.MsgArray[connectStringLength - 3];
            char[] hostparts = hostname.ToCharArray();
            if (!hostparts.Contains('!') || !hostparts.Contains('@')) return false;
            string[] splitnick = hostname.Split('!');
            _nick = splitnick[0];
            string rightpart = splitnick[1];
            string[] splithost = rightpart.Split('@');
            _ident = splithost[0];
            string host = splithost[1];
            if (!_persistance.UserNameExists(_ident))
            {
                Messages.Add("Warning! Potential security breach!");
                Messages.Add(string.Format("A user connected with an ident not present in the database, this should never happen! Ident: {0} Host: {1}", _ident, host));
                Messages.Add("This probably means someone managed to edit the database and inserted their username/password, meaning the ircd machine is compromised." );
                Messages.Add("Act accordingly.");
                return false;
            }
            return true;
        }

        public override bool NeedsOperPermission
        {
            get { return false; }
        }

        public override CommandName Name { get { return CommandName.Connect; } }

        public override CommandType CommandType
        {
            get { return CommandType.IrcNoticeCommmand; }
        }

        public override void Execute()
        {
            User user = _persistance.GetUserFromIdent(_ident);
            if (!string.IsNullOrEmpty(user.Vhost))
            {
                var vhostOperCommmand = VhostOperCommmand.SetVhost(_nick, user.Vhost);
                OperCommands.Add(vhostOperCommmand);
            }
            if (user.AutoJoin)
            {
                foreach (var group in user.Groups)
                {
                    if (!string.IsNullOrEmpty(group.Channel))
                    {
                        var sajoinOperCommmand = SajoinOperCommmand.SetSaJoin(_nick, group.Channel);
                        OperCommands.Add(sajoinOperCommmand);
                        if (group.GroupAdmins.Contains(user))
                        {
                            var saModeOpsOperCommmand = SaModeOpsOperCommmand.SetSamode(_nick, group.Channel);
                            OperCommands.Add(saModeOpsOperCommmand);
                        }
                    }
                }
            }
        }
    }
}