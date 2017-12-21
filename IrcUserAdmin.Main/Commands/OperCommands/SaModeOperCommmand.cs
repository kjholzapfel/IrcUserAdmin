using System;

namespace IrcUserAdmin.Commands.OperCommands
{
    public class SaModeOperCommmand : IOperCommand
    {
        private readonly string _nick;
        private readonly string _mode;
        private readonly string _channel;

        private SaModeOperCommmand(string nick, string mode, string channel)
        {
            if (nick == null) throw new ArgumentNullException("nick");
            if (mode == null) throw new ArgumentNullException("mode");
            if (channel == null) throw new ArgumentNullException("channel");
            _nick = nick;
            _mode = mode;
            _channel = channel;
        }

        public static SaModeOperCommmand SetSamode(string nick, string mode, string channel)
        {
            return new SaModeOperCommmand(nick, mode, channel);
        }

        public string GetOperText()
        {
            string operText = string.Format("SAMODE {0} {1} {2}", _channel, _mode, _nick);
            return operText;
        }
    }
}