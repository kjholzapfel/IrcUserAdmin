using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class SetHost : AbstractIrcCommand<SetHostArguments>
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;

        public SetHost(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
        }

        public override CommandName Name { get { return CommandName.SetHost; } }
        public override CommandType CommandType { get { return CommandType.OperCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[SetHostArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[SetHostArguments.Username]));
                return false;
            }
            User user = _persistance.GetUserInfo(Mapping[SetHostArguments.Username]);
            if (string.IsNullOrEmpty(user.Vhost))
            {
                Messages.Add(string.Format("User {0} has no vhost set, use setvhost first", Mapping[SetHostArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            User user = _persistance.GetUserInfo(Mapping[SetHostArguments.Username]);
            List<string> nicks = _ircReadWriteExchange.GetWhoList().Where(i => i.Ident == user.Name).Select(n => n.Nick).ToList();
            if (nicks.Count == 0)
            {
                Messages.Add(string.Format("Can't change hostname, user {0} is currently offline", Mapping[SetHostArguments.Username]));
            }
            else
            {
                foreach (string nick in nicks)
                {
                   var command = VhostOperCommmand.SetVhost(nick, user.Vhost);
                   OperCommands.Add(command);
                }
                string processednicks = string.Join(" ,", nicks);
                Messages.Add(string.Format("Executing hostname change for user {0}, affected nick(s) {1}", Mapping[SetHostArguments.Username], processednicks));
            }
        }
    }

    public enum SetHostArguments
    {
        Username,
    }
}