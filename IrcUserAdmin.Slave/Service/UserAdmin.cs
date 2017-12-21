using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Autofac;
using IrcUserAdmin.Entities.Slave;
using IrcUserAdmin.Slave.Contracts;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slave.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class UserAdmin : IUserAdmin
    {
        private readonly ILifetimeScope _scope;

        public UserAdmin(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            _scope = scope;
        }

        public SlaveResponse AddUser(WcfUser wcfUser)
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.AddNewUser(wcfUser.Name, wcfUser.Password);
                    slave.Persist();
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }

        public SlaveResponse ChangePassword(WcfUser wcfUser)
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.ChangePassword(wcfUser.Name, wcfUser.Password);
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }

        public SlaveResponse AddHosts(string username, IList<WcfHost> wcfHosts)
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.AddHosts(username, TranslateHostsToSlave(wcfHosts));
                    slave.Persist();
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }

        public SlaveResponse RemoveHosts(string username, IList<WcfHost> wcfHosts)
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.RemoveHosts(username, TranslateHostsToSlave(wcfHosts));
                    slave.Persist();
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }


        public SlaveResponse DeleteUser(string name)
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.DeleteUser(name);
                    slave.Persist();
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }

        public SlaveResponse ClearDatabase()
        {
            var response = new SlaveResponse { ResponseOk = true };
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    slave.ClearDatabase();
                    slave.Persist();
                }
            }
            catch (Exception exp)
            {
                response.ResponseOk = false;
                response.Exception = exp.Message;
            }
            return response;
        }

        public WcfUserList GetUsers()
        {
            var list = new WcfUserList {ResponseOk = true};
            try
            {
                using (var lifetimeScope = _scope.BeginLifetimeScope())
                {
                    var slave = lifetimeScope.Resolve<ISlavePersistence>();
                    list.Users = TranslateUsers(slave.GetUsers()).ToList();
                }
            }
            catch (Exception exp)
            {
                list.ResponseOk = false;
                list.Exception = exp.Message;
            }
            return list;
        }

        private static IEnumerable<WcfUser> TranslateUsers(IEnumerable<SlaveUser> getUsers)
        {
            return getUsers.Select(user => new WcfUser
            {
                Name = user.Name,
                Password = user.Password,
                Hosts = TranslateHosts(user.Hosts).ToList()
            });
        }

        private static IEnumerable<WcfHost> TranslateHosts(IEnumerable<SlaveHostname> hosts)
        {
            return hosts.Select(hostname => new WcfHost {HostAdress = hostname.Host});
        }

        private static IEnumerable<string> TranslateHostsToSlave(IEnumerable<WcfHost> wcfHosts)
        {
            return wcfHosts.Select(wcfHost => wcfHost.HostAdress);
        }
    }
}