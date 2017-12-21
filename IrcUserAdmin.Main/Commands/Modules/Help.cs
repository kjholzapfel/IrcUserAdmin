using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands.Modules
{
    public class Help : AbstractIrcCommand<EmptyEnum>
    {
        private readonly ILifetimeScope _scope;
        public override CommandName Name { get { return CommandName.Help; } }
        public override CommandType CommandType { get { return CommandType.AdminCommand; } }

        public Help(IPersistance persistance, IIrcReadWriteExchange ircReadWriteExchange, ILifetimeScope scope) : base(persistance, ircReadWriteExchange)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (ircReadWriteExchange == null) throw new ArgumentNullException("ircReadWriteExchange");
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        public override void Execute()
        {
            var commandList = _scope.Resolve<IList<IIrcCommand>>();
            var commandTypes = Enum.GetValues(typeof(CommandType)).Cast<CommandType>();
            Messages.Add("Available commands:");
            foreach (var commandType in commandTypes)
            {
                CommandType type = commandType;
                if (type != CommandType.IrcNoticeCommmand)
                {
                    var commands = commandList.Where(ic => ic.CommandType.Equals(type));
                    string helpLine = string.Format("{0} : {1} ", type, string.Join(", ", commands.Select(c => c.Name)));
                    Messages.Add(helpLine.ToLowerInvariant());
                }
            }
        }
    }
}