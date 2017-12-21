using System;
using IrcUserAdmin.NHibernate;
using IrcUserAdmin.Tools;

namespace IrcUserAdmin.ConfigSettings
{
    public class DbBootStrapAddUsers : IDbBootstrapAddUsers
    {
        private readonly IBotConfig _botconfig;
        private readonly IPersistance _persistance;

        public DbBootStrapAddUsers(IPersistance persistance, IBotConfig botconfig)
        {
            if (persistance == null) throw new ArgumentNullException("persistance");
            if (botconfig == null) throw new ArgumentNullException("botconfig");
            _persistance = persistance;
            _botconfig = botconfig;
        }

        public void BootStrapIrcBotAccount()
        {
            CheckAndAddOperGroup();
            string username = _botconfig.BotSettings.IrcUserName;
            if (!_persistance.UserNameExists(username))
            {
                string password = _botconfig.BotSettings.IrcPassword;
                string hashedPassword = HashFunctions.ComputeHash(password);
                _persistance.AddNewUserToGroup(username, hashedPassword, "opers");
            }
            _persistance.Persist();
        }

        public void BootStrapInitialUsers()
        {
            CheckAndAddOperGroup();
            if (_botconfig.Settings.DbUsers != null)
            {
                foreach (var adduser in _botconfig.Settings.DbUsers)
                {
                    if (!_persistance.UserNameExists(adduser.Username))
                    {
                        string hashedPassword = HashFunctions.ComputeHash(adduser.Password);
                        _persistance.AddNewUserToGroup(adduser.Username, hashedPassword, "opers");
                        _persistance.SetAdmin(adduser.Username, adduser.IsAdmin);
                        _persistance.SetOper(adduser.Username, adduser.IsOper);
                    }
                }
            }
            _persistance.Persist();
        }

        private void CheckAndAddOperGroup()
        {
            if (!_persistance.GroupExists("opers"))
            {
                _persistance.AddGroup("opers");
            }
        }
    }
}