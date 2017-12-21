using System;
using System.Linq;
using IrcUserAdmin.Commands.OperCommands;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class DeleteKillUser : AbstractIrcCommand<DeleteKillUserArguments>
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;

        public DeleteKillUser(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange): base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
        }

        public override CommandName Name { get { return CommandName.DeleteKillUser; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[DeleteKillUserArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[DeleteKillUserArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            _persistance.DeleteUser(Mapping[DeleteKillUserArguments.Username]);
            Messages.Add(string.Format("User {0} succesfully deleted.", Mapping[DeleteKillUserArguments.Username]));
            var info = _ircReadWriteExchange.GetWhoList().Where(u => String.Equals(u.Ident, Mapping[DeleteKillUserArguments.Username], StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (info.Any())
            {
                foreach (var who in info)
                {
                    var command = KillOperCommmand.SetKill(who.Nick);
                    OperCommands.Add(command);
                }
                string nicks = string.Join(", ", info.Select(s => s.Nick));
                Messages.Add(string.Format("Killed the following nick(s): {0}", nicks));
            }
            else
            {
                Messages.Add(string.Format("No nicks killed, user {0} wasn't online.", Mapping[DeleteKillUserArguments.Username]));
            }
        }
    }

    public enum DeleteKillUserArguments
    {
        Username
    }
}