using System;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class Announce : AbstractIrcCommand<EmptyEnum>
    {
        private readonly IPersistance _persistance;
        public Announce(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override bool NeedsOperPermission
        {
            get { return false; }
        }

        public override CommandName Name { get { return CommandName.Announce; } }

        public override bool CheckPreconditions()
        {
            if (!(Context.MsgArray[2] == "Forbidden" || Context.MsgArray[3] == "connection")) return false;
            string hostname = Context.MsgArray[5];
            char[] hostparts = hostname.ToCharArray();
            if (!hostparts.Contains('!') || !hostparts.Contains('@')) return false;
            string[] splitnick = hostname.Split('!');
            string rightpart = splitnick[1];
            string[] splithost = rightpart.Split('@');
            string ident = splithost[0];
            string host = splithost[1];
            string msgjoin = string.Join(" ", Context.MsgArray);
            string msg = msgjoin.Split('(').Last().TrimEnd(')');
            if (msg == "SQL query returned no matches")
            {
                if (_persistance.UserNameExists(ident))
                {
                    msg = "Invalid password";
                }
                else
                {
                    msg = "User does not exist in database";
                }
            }
            Messages.Add(string.Format("Connection rejected from user '{0}' with hostname '{1}'. Reason: {2}", ident, host, msg));
            return false;
        }

        public override CommandType CommandType
        {
            get { return CommandType.IrcNoticeCommmand; }
        }

        public override void Execute(){}
    }
}