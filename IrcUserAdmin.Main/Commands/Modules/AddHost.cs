using System;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;
using IrcUserAdmin.Tools;

namespace IrcUserAdmin.Commands.Modules
{
    public class AddHost : AbstractIrcCommand<AddHostArguments>
    {
        private readonly IPersistance _persistance;

        public AddHost(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            _persistance = persistance;
        }

        public override CommandName Name { get { return CommandName.AddHost; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public override bool CheckPreconditions()
        {
            if (!base.CheckPreconditions())
            {
                return false;
            }
            if (!_persistance.UserNameExists(Mapping[AddHostArguments.Username]))
            {
                Messages.Add(string.Format("Username {0} doesn't exists.", Mapping[AddHostArguments.Username]));
                return false;
            }
            var badHosts = Context.MsgArray.Skip(1).Where(host => !host.IsValidHostOrIp()).ToList();
            if (badHosts.Any())
            {
                Messages.Add(string.Format("The following hosts are not valid {0}.", string.Join(", ", badHosts)));
                return false;
            }
            foreach (var host in Context.MsgArray.Skip(1))
            {
                if (_persistance.HostExists(Mapping[AddHostArguments.Username], host))
                {
                    Messages.Add(string.Format("Host/IP already added, skipping ...  {0}.", host));
                }
            }
            return true;
        }

        public override void Execute()
        {
            var hosts = Context.MsgArray.Skip(1).ToList();
            _persistance.AddHosts(Mapping[AddHostArguments.Username], hosts);
            Messages.Add(string.Format("The following hosts are now added to user {0}: {1}", Mapping[AddHostArguments.Username], string.Join(", ", hosts)));
        }
    }

    public enum AddHostArguments
    {
        Username,
        Host
    }
}