using IrcUserAdmin.IrcBot;

namespace IrcUserAdmin.Commands
{
    public interface IIrcCommands
    {
        bool CommandExists(CommandName? ircCommandName);
        void ExecuteIrcCommand(IrcCommandContext ircCommandName);
    }
}