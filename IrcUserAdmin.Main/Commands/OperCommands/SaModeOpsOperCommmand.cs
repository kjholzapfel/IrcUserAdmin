using System;

namespace IrcUserAdmin.Commands.OperCommands
{
    public class SaModeOpsOperCommmand : IOperCommand
    {
        private readonly string _nick;
        private readonly string _channel;

        private SaModeOpsOperCommmand(string nick, string channel)
        {
            if (nick == null) throw new ArgumentNullException("nick");
            if (channel == null) throw new ArgumentNullException("channel");
            _nick = nick;
            _channel = channel;
        }

        public static SaModeOpsOperCommmand SetSamode(string nick, string channel)
        {
            return new SaModeOpsOperCommmand(nick, channel);
        }

        public string GetOperText()
        {
            string operText = string.Format("SAMODE {0} +o {1}", _channel, _nick);
            return operText;
        }
    }
}