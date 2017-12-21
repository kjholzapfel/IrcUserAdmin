using System;
using System.Linq;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class UserInfo : AbstractIrcCommand<ShowUserArguments>
    {
        private readonly IPersistance _persistance;
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;

        public UserInfo(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
            _ircReadWriteExchange = ircReadWriteExchange;
        }

        public override CommandName Name { get { return CommandName.UserInfo; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[ShowUserArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[ShowUserArguments.Username]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            string groups;
            User user = _persistance.GetUserInfo(Mapping[ShowUserArguments.Username]);
            if (!user.Groups.Any())
            {
                groups = "No groups added";
            }
            else
            {
                groups = string.Join(", ", user.Groups.Select(s => s.GroupName));
            }
            string online = "Online: No";

            var info = _ircReadWriteExchange.GetWhoList().Where(u => String.Equals(u.Ident, Mapping[ShowUserArguments.Username], StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (info.Any())
            {
                string nicks = string.Join(", ", info.Select(s => s.Nick));
                online = string.Format("Online: Yes, nick(s): {0}", nicks);
            }
            Messages.Add(string.Format("User information for {0}", user.Name));
            Messages.Add(online);
            Messages.Add(string.Format("Groups: {0}", groups));
            Messages.Add(string.Format("Oper rights: {0}", user.IsOper));
            Messages.Add(string.Format("Admin rights: {0}", user.IsAdmin));
            Messages.Add(string.Format("Autojoin: {0}", user.AutoJoin));
            string vhost = "No vhost defined";
            if (!string.IsNullOrEmpty(user.Vhost))
            {
                vhost = user.Vhost;
            }
            Messages.Add(string.Format("Vhost: {0}", vhost));
            Messages.Add(string.Format("Hosts: {0}", string.Join(", ", user.Hosts)));
        }
    }

    public enum ShowUserArguments
    {
        Username
    }
}