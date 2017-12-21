using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class IrcSecuritySweep : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;

        public IrcSecuritySweep(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
        }

        public override CommandName Name { get { return CommandName.IrcSecuritySweep; } }
        public override CommandType CommandType { get { return CommandType.SecurityCommand; } }

        public override void Execute()
        {
            var wholist = _ircReadWriteExchange.GetWhoList();
            var dbUsers = _persistance.GetUsers();
            var violators = new List<string>();
            foreach (var whoInfo in wholist)
            {
                if (!dbUsers.Contains(whoInfo.Ident, StringComparer.InvariantCultureIgnoreCase))
                {
                    violators.Add(whoInfo.Ident);
                }
            }
            if (violators.Any())
            {
                Messages.Add("IRC security sweep: security violations detected, user(s) are online which are not present in the database:");
                foreach (var violator in violators)
                {
                    var user = wholist.First(w => w.Ident.Equals(violator));
                    var msg = string.Format("User: {0}  Server: {1}", user.Ident, user.Server);
                    Messages.Add(msg);
                }
            }
            else
            {
                if (Context.MessageOrigin == MessageOrigin.Channel || Context.MessageOrigin == MessageOrigin.Query)
                {
                    Messages.Add("IRC security sweep: Ok. No users detected who are not present in the database.");
                }
            }
        }
    }
}