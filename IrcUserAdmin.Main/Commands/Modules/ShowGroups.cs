using System;
using System.Collections.Generic;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class ShowGroups : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;
        public override CommandName Name { get { return CommandName.ShowGroups; } }
        public override CommandType CommandType { get { return CommandType.GroupCommand; } }

        public ShowGroups(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override void Execute()
        {
            var grouplist = _persistance.GetGroupList();
            var groups = new List<string>();
            foreach (Group group in grouplist)
            {
                string groupstring = string.Format("{0} ({1})", group.GroupName, group.Members.Count);
                groups.Add(groupstring);
            }
            string message = string.Join(", ", groups);
            Messages.Add("Displaying all groups with the total number of users:");
            Messages.Add(message);
        }
    }
}