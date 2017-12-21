using System;
using System.Linq;
using IrcUserAdmin.Commands.Common;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class ShowOnlineUsers : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IIrcReadWriteExchange _ircReadWriteExchange;
        private readonly UserDisplay _userDisplay;

        public ShowOnlineUsers(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange, UserDisplay userDisplay) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            if (userDisplay == null) throw new ArgumentNullException("userDisplay");
            _ircReadWriteExchange = ircReadWriteExchange;
            _userDisplay = userDisplay;
        }

        public override CommandName Name { get { return CommandName.ShowOnlineUsers; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override void Execute()
        {
            var whoInfos = _ircReadWriteExchange.GetWhoList();
            var online = whoInfos.Select(s => s.Ident).Distinct().ToList();
            Messages.Add("Online users:");
            var messages = _userDisplay.SortAlphabetically(online);
            Messages.AddRange(messages);
            Messages.Add(string.Format("Total online: {0}", online.Count));
        }
    }
}