namespace IrcUserAdmin.ConfigSettings
{
    public interface IDbBootstrap
    {
        void BootStrapInitialUsers();
        void BootStrapIrcBotAccount();
    }
}