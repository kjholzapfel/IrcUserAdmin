using System.Collections.Generic;
using IrcUserAdmin.Commands;

namespace IrcUserAdmin.IrcBot
{
    public class IrcCommandContext
    {
        public CommandName? Command { get; set; }
        public List<string> MsgArray { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
        public string Ident { get; set; }
        public string Host { get; set; }
        public MessageOrigin MessageOrigin { get; set; }
    }
}