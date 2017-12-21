namespace IrcUserAdmin.IrcBot
{
    public class IrcWriteContext
    {
        public string Message { get; set; }
        public MessageOrigin MessageOrigin { get; set; }
        public string User { get; set; }
    }
}