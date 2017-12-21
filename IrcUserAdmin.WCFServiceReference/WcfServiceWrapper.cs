using System;
using System.Collections.Generic;
using System.ServiceModel;
using IrcUserAdmin.Entities.Slave;
using IrcUserAdmin.SlavePersistance;
using IrcUserAdmin.WCFServiceReference.ServiceReference;

namespace IrcUserAdmin.WCFServiceReference
{
    public class WcfServiceWrapper : ISlavePersistence
    {
        private readonly UserAdminClient _client;

        public WcfServiceWrapper()
        {
            _client = new UserAdminClient();

        }

        public void AddUser(string name, string password)
        {
            var user = new User {Name = name, Password = password};
            var result = _client.Using(wcf => wcf.AddUser(user));
            if (!result.ResponseOk)
            {
                throw new WcfServerException(result.Exception);
            }
        }


        public void Dispose()
        {
            if (_client != null)
            {
                var disposable = _client as IDisposable;
                disposable.Dispose();
            }
        }

        public void AddNewUser(string username, string hashedPassword)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void AddHosts(string username, IEnumerable<string> hosts)
        {
            throw new NotImplementedException();
        }

        public void RemoveHosts(string username, IEnumerable<string> hosts)
        {
            throw new NotImplementedException();
        }

        public void Persist()
        {
            throw new NotImplementedException();
        }

        public void ClearDatabase()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SlaveUser> GetUsers()
        {
            throw new NotImplementedException();
        }
    }

    public class WcfServerException : Exception
    {
        public WcfServerException(string exception): base (exception)
        {

        }
    }

    public static class WcfHelper
    {
        public static TResult Using<T, TResult>(this T client, Func<T, TResult> work) where T : ICommunicationObject
        {
            try
            {
                var result = work(client);

                client.Close();

                return result;
            }
            catch (Exception e)
            {
                client.Abort();

                throw;
            }
        }
    }
}