using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using IrcUserAdmin.Commands;
using IrcUserAdmin.ConfigSettings;
using IrcUserAdmin.Tools;
using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public class IrcEventHandler : IIrcEventHandler
    {
        private readonly ILifetimeScope _container;
        private readonly IBotConfig _config;

        public IrcEventHandler(ILifetimeScope container, IBotConfig config)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void HandleQueryNotice(IrcEventArgs ircEventArgs)
        {
            var ircCommandContext = GetContextFromIrcEventArgs(ircEventArgs, MessageOrigin.Notice);
            DispatchCommand(ircCommandContext);
        }

        public void HandleQueryMessage(IrcEventArgs ircEventArgs)
        {
            var ircCommandContext = GetContextFromIrcEventArgs(ircEventArgs, MessageOrigin.Query);
            DispatchCommand(ircCommandContext);
        }

        public void HandleChannelMessage(IrcEventArgs ircEventArgs)
        {
            var ircCommandContext = GetContextFromIrcEventArgs(ircEventArgs, MessageOrigin.Channel);
            DispatchCommand(ircCommandContext);
        }

        private void DispatchCommand(IrcCommandContext ircCommandContext)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var ircCommands = scope.Resolve<IIrcCommands>();
                if (ircCommands.CommandExists(ircCommandContext.Command))
                {
                    ircCommands.ExecuteIrcCommand(ircCommandContext);
                }
            }
        }

        private IrcCommandContext GetContextFromIrcEventArgs(IrcEventArgs ircEventArgs, MessageOrigin messageOrigin)
        {
            string command;
            CommandName? commandName;
            var msgarray = ircEventArgs.Data.Message.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> msgStrings = null;
            switch (messageOrigin)
            {
                case MessageOrigin.Channel:
                    command = ircEventArgs.Data.MessageArray[1];
                    commandName = GetCommandName(command);
                    if (commandName != null) msgStrings = FilterMsgArray(msgarray, commandName.Value).ToList();
                    break;
                case MessageOrigin.Query:
                    command = ircEventArgs.Data.MessageArray[1];
                    commandName = GetCommandName(command);
                    if (commandName != null) msgStrings = FilterMsgArray(msgarray, commandName.Value).ToList();
                    break;
                case MessageOrigin.Notice:
                    string connectpart = msgarray[1];
                    command = GetNoticeType(connectpart).ToString();
                    commandName = GetCommandName(command);
                    if (commandName != null) msgStrings = FilterMsgArray(msgarray, commandName.Value).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageOrigin));
            }
   
            var ircCommandContext = new IrcCommandContext
            {
                Channel = ircEventArgs.Data.Channel,
                Host = ircEventArgs.Data.Host,
                Ident = ircEventArgs.Data.Ident,
                MessageOrigin = messageOrigin,
                Command = commandName,
                MsgArray = msgStrings,
                User = ircEventArgs.Data.Nick
            };
            return ircCommandContext;
        }

        private CommandName? GetCommandName(string command)
        {
            CommandName name;
            if (Enum.TryParse(command, true, out name))
            {
                return name;
            }
            return null;
        }

        private IEnumerable<string> FilterMsgArray(List<string> msgarray, CommandName commandName)
        {
            var botname = _config.BotSettings.IrcBotName;
            msgarray.RemoveAll(n => n.Equals(botname, StringComparison.InvariantCultureIgnoreCase));
            msgarray.RemoveAll(n => n.Equals(commandName.ToString(), StringComparison.InvariantCultureIgnoreCase));
            msgarray.Remove(commandName.ToString());
            return msgarray;
        }

        private NoticeTypeInternal GetNoticeType(string command)
        {
            var withOutSpecialCharacters = new string(command.Where(char.IsLetterOrDigit).ToArray());
            var noticeType = withOutSpecialCharacters.ParseEnum<NoticeTypes>();
            if (noticeType == null) throw new Exception();
            NoticeTypeInternal noticeTypeInternal;
            switch (noticeType.Value)
            {
                case NoticeTypes.RemoteConnect:
                    noticeTypeInternal = NoticeTypeInternal.Connect;
                    break;
                case NoticeTypes.Connect:
                    noticeTypeInternal = NoticeTypeInternal.Connect;
                    break;
                case NoticeTypes.RemoteAnnouncement:
                    noticeTypeInternal = NoticeTypeInternal.Announce;
                    break;
                case NoticeTypes.Announcement:
                    noticeTypeInternal = NoticeTypeInternal.Announce;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return noticeTypeInternal;
        }
    }
}