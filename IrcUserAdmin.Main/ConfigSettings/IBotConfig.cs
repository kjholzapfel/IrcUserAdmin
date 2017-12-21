using IrcUserAdmin.ConfigSettings.ConfigClasses;

namespace IrcUserAdmin.ConfigSettings
{
    public interface IBotConfig
    {
        Settings Settings { get; }
        BotSettings BotSettings { get; }
    }
}