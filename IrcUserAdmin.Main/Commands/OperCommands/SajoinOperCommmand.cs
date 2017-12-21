using System;

namespace IrcUserAdmin.Commands.OperCommands
{
    public class SajoinOperCommmand : IOperCommand
    {
        private readonly string _nick;
        private readonly string _channel;

        private SajoinOperCommmand(string nick, string channel)
        {
            if (nick == null) throw new ArgumentNullException("nick");
            if (channel == null) throw new ArgumentNullException("channel");
            _nick = nick;
            _channel = channel;
        }

        public static SajoinOperCommmand SetSaJoin(string nick, string channel)
        {
            return new SajoinOperCommmand(nick, channel);
        }

        public string GetOperText()
        {
            string operText = string.Format("SAJOIN {0} {1}", _nick, _channel);
            return operText;
        }
    }
}