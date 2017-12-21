using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.Commands.Modules
{
    public class KillUser : AbstractIrcCommand<KillUserArguments>
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;
        private List<WhoInfo> _nickInfo;

        public KillUser(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
        }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[KillUserArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[KillUserArguments.Username]));
                return false;
            }
            var whoList = _ircReadWriteExchange.GetWhoList();
            _nickInfo = whoList.Where(w => String.Equals(w.Ident, Mapping[KillUserArguments.Username], StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (!_nickInfo.Any())
            {
                Messages.Add(string.Format("User {0} isn't online.", Mapping[KillUserArguments.Username]));
                return false;
            }
            return true;
        }

        public override CommandName Name { get { return CommandName.KillUser; } }
        public override CommandType CommandType { get { return CommandType.OperCommand; } }

        public override void Execute()
        {
            foreach (WhoInfo who in _nickInfo)
            {
                var operCommand = KillOperCommmand.SetKill(who.Nick);
                OperCommands.Add(operCommand);
            }
            string nicks = string.Join(", ", _nickInfo.Select(s => s.Nick));
            Messages.Add(string.Format("Killed the following nick(s): {0}", nicks));
        }
    }

    public enum KillUserArguments
    {
        Username
    }
}