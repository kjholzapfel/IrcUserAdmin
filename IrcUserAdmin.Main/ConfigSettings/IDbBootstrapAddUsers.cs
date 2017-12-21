namespace IrcUserAdmin.ConfigSettings
{
    public interface IDbBootstrapAddUsers
    {
        void BootStrapInitialUsers();
        void BootStrapIrcBotAccount();
    }
}