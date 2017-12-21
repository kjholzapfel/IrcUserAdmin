using System;
using Autofac;

namespace IrcUserAdmin.ConfigSettings
{
    public class DbBootstrap : IDbBootstrap
    {
        private readonly ILifetimeScope _scope;

        public DbBootstrap(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        public void BootStrapIrcBotAccount()
        {
            using (var sessionScope = _scope.BeginLifetimeScope())
            {
                var adduser = sessionScope.Resolve<IDbBootstrapAddUsers>();
                adduser.BootStrapIrcBotAccount();
            }
        }

        public void BootStrapInitialUsers()
        {
            using (var sessionScope = _scope.BeginLifetimeScope())
            {
                var adduser = sessionScope.Resolve<IDbBootstrapAddUsers>();
                adduser.BootStrapInitialUsers();
            }
        }
    }
}