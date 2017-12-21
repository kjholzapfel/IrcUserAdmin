using System;
using System.Collections.Generic;
using IrcUserAdmin.Entities;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class ShowGroupDetails : AbstractIrcCommand<ShowGroupDetailArguments>
    {
        private readonly IPersistance _persistance;

        public ShowGroupDetails(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.ShowGroupDetails; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.GroupExists(Mapping[ShowGroupDetailArguments.Groupname]))
            {
                Messages.Add(string.Format("Groupname {0} doesn't exists.", Mapping[ShowGroupDetailArguments.Groupname]));
                return false;
            }
            return true;
        }

        public override void Execute()
        {
            Group group = _persistance.GetGroupDetails(Mapping[ShowGroupDetailArguments.Groupname]);
            Messages.Add(string.Format("Group name: {0} ", group.GroupName));
            string channel;
            if (string.IsNullOrEmpty(group.Channel))
            {
                channel = "no channel set";
            }
            else
            {
                channel = group.Channel;
            }
            const int numresults = 5;
            Messages.Add(string.Format("Group channel: {0} ", channel));
            Messages.Add("Group users:");
            var groupmembers = new List<string>();
            for (int index = 0; index < group.Members.Count; index++)
            {
                string name = group.Members[index].Name;
                groupmembers.Add(name);
                int modulo = index%numresults;
                if (index != 0 && modulo == 0)
                {
                    string userstring = string.Join(", ", groupmembers);
                    Messages.Add(userstring);
                    groupmembers = new List<string>();
                }
            }
            if (groupmembers.Count > 0)
            {
                string userstring = string.Join(", ", groupmembers);
                Messages.Add(userstring);
            }
            Messages.Add("Group Admins:");
            var groupadmins = new List<string>();
            for (int index = 0; index < group.GroupAdmins.Count; index++)
            {
                string name = group.GroupAdmins[index].Name;
                groupmembers.Add(name);
                int modulo = index%numresults;
                if (index != 0 && modulo == 0)
                {
                    string userstring = string.Join(", ", groupmembers);
                    Messages.Add(userstring);
                    groupmembers = new List<string>();
                }
            }
            if (groupadmins.Count > 0)
            {
                string userstring = string.Join(", ", groupmembers);
                Messages.Add(userstring);
            }
        }
    }

    public enum ShowGroupDetailArguments
    {
        Groupname
    }
}