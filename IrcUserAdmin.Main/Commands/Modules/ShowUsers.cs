using System;
using IrcUserAdmin.Commands.Common;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class ShowUsers : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;
        private readonly UserDisplay _userDisplay;

        public ShowUsers(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange, UserDisplay userDisplay) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            if (userDisplay == null) throw new ArgumentNullException("userDisplay");
            _persistance = persistance;
            _userDisplay = userDisplay;
        }

        public override CommandName Name { get { return CommandName.ShowUsers; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override void Execute()
        {
            var userlist = _persistance.GetUsers();
            Messages.Add("Added users:");
            var messages = _userDisplay.SortAlphabetically(userlist);
            Messages.AddRange(messages);
            Messages.Add(string.Format("Total: {0}", userlist.Count));
        }
    }
}