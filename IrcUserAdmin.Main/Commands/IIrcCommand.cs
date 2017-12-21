using IrcUserAdmin.IrcBot;

namespace IrcUserAdmin.Commands
{
    public interface IIrcCommand
    {
        CommandName Name { get; }
        CommandType CommandType { get; }
        bool NeedsOperPermission { get; }
        void SetContext(IrcCommandContext context);
        bool CheckPermissions();
        bool CheckPreconditions();
        void Execute();
        void Help();
        void MapInput();
        void Write();
    }
}