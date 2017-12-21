using System;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class RemoveHost : AbstractIrcCommand<RemoveHostArguments>
    {
        private readonly IPersistance _persistance;

        public RemoveHost(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.RemoveHost; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[RemoveHostArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[RemoveHostArguments.Username]));
                return false;
            }
            foreach (var host in Context.MsgArray.Skip(1))
            {
                if (!_persistance.HostExists(Mapping[RemoveHostArguments.Username], host))
                {
                    Messages.Add(string.Format("Host/IP not added, skipping ...  {0}.", host));
                }
            }
            return true;
        }
        public override void Execute()
        {
            var hosts = Context.MsgArray.Skip(1).ToList();
            _persistance.RemoveHosts(Mapping[RemoveHostArguments.Username], hosts);
            Messages.Add(string.Format("The following hosts removed from user {0}: {1}", Mapping[RemoveHostArguments.Username], string.Join(", ", hosts)));
        }
    }

    public enum RemoveHostArguments
    {
        Username,
        Host
    }
}