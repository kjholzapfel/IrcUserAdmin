using System;

namespace IrcUserAdmin.Commands.OperCommands
{
    public class KillOperCommmand : IOperCommand
    {
        private readonly string _nick;

        private KillOperCommmand(string nick)
        {
            if (nick == null) throw new ArgumentNullException("nick");
            _nick = nick;
        }

        public static KillOperCommmand SetKill(string nick)
        {
            return new KillOperCommmand(nick);
        }

        public string GetOperText()
        {
            string operText = string.Format("KILL {0} killed", _nick);
            return operText;
        }
    }
}