using System;

namespace IrcUserAdmin.Commands.OperCommands
{
    public class VhostOperCommmand : IOperCommand
    {
        private readonly string _nick;
        private readonly string _vhost;

        private VhostOperCommmand(string nick, string vhost)
        {
            if (nick == null) throw new ArgumentNullException("nick");
            if (vhost == null) throw new ArgumentNullException("vhost");
            _nick = nick;
            _vhost = vhost;
        }

        public static VhostOperCommmand SetVhost(string nick, string vhost)
        {
            return new VhostOperCommmand(nick, vhost);
        }

        public string GetOperText()
        {
            string operText = string.Format("CHGHOST {0} {1}", _nick, _vhost);
            return operText;
        }
    }
}