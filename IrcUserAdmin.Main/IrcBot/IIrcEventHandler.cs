using Meebey.SmartIrc4net;

namespace IrcUserAdmin.IrcBot
{
    public interface IIrcEventHandler
    {
        void HandleQueryNotice(IrcEventArgs ircEventArgs);
        void HandleQueryMessage(IrcEventArgs ircEventArgs);
        void HandleChannelMessage(IrcEventArgs ircEventArgs);
    }
}