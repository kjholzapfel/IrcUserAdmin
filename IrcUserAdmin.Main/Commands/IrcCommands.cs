using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.IrcBot;
using IrcUserAdmin.NHibernate;

namespace IrcUserAdmin.Commands
{
    public class IrcCommands : IIrcCommands
    {
        private readonly IList<IIrcCommand> _ircCommands;
        private readonly IUserDao _userDao;

        public IrcCommands(IList<IIrcCommand> ircCommands, IUserDao userDao)
        {
            if (ircCommands == null) throw new ArgumentNullException("ircCommands");
            if (userDao == null) throw new ArgumentNullException("userDao");
            _ircCommands = ircCommands;
            _userDao = userDao;
        }

        public bool CommandExists(CommandName? ircCommandName)
        {
            if (ircCommandName == null) return false;
            return _ircCommands.Any(ic => ic.Name == ircCommandName);
        }

        public void ExecuteIrcCommand(IrcCommandContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            IIrcCommand command = _ircCommands.First(ic => ic.Name == context.Command);
            command.SetContext(context);
            if (command.NeedsOperPermission && context.MessageOrigin != MessageOrigin.Quartz)
            {
                var isOper = command.CheckPermissions();
                if (!isOper) return;
            }
            else
            {
                //suppose people might "guess" the oper notice commands, silent ignore
                if (context.MessageOrigin != MessageOrigin.Notice & context.MessageOrigin != MessageOrigin.Quartz)
                {
                    return;
                }
            }
            command.MapInput();
            if (command.CheckPreconditions())
            {
                command.Execute();
            }
            _userDao.Persist();
            command.Write();
        }
    }
}