using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Common.NHibernate;
using IrcUserAdmin.Entities.Slave;
using NHibernate.Linq;

namespace IrcUserAdmin.SlavePersistance
{
    public class SlavePersistence : ISlavePersistence
    {
        private readonly IUnitOfWork _unitOfWork;

        public SlavePersistence(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public void AddNewUser(string username, string hashedPassword)
        {
            var user = new SlaveUser { Name = username, Password = hashedPassword };
            _unitOfWork.AddToSave(user);
        }

        public void DeleteUser(string username)
        {
            var user = GetUser(username);
            _unitOfWork.AddToDelete(user);
        }

        public void ChangePassword(string username, string password)
        {
            var user = GetUser(username);
            user.Password = password;
            _unitOfWork.AddToSave(user);
        }

        public void AddHosts(string username, IEnumerable<string> hosts)
        {
            var user = GetUser(username);
            var existingsHost = user.Hosts.ToList(); //materialises host entities
            foreach (var host in hosts)
            {
                if (existingsHost.All(h => !String.Equals(h.Host, host, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var hostname = new SlaveHostname { Host = host, User = user };
                    user.Hosts.Add(hostname);
                    _unitOfWork.AddToSave(hostname);
                }
            }
        }

        public void RemoveHosts(string username, IEnumerable<string> hosts)
        {
            var user = GetUser(username);
            var existingsHost = user.Hosts.ToList(); //materialises host entities
            foreach (var host in hosts)
            {
                string host1 = host;
                var removeHost = existingsHost.FirstOrDefault(h => String.Equals(h.Host, host1, StringComparison.InvariantCultureIgnoreCase));
                if (removeHost != null)
                {
                    user.Hosts.Remove(removeHost);
                    _unitOfWork.AddToDelete(removeHost);
                }
            }
            _unitOfWork.AddToSave(user);
        }

        private SlaveUser GetUser(string username)
        {
            return _unitOfWork.Session.Query<SlaveUser>().FirstOrDefault(s => s.Name.ToLowerInvariant() == username.ToLowerInvariant());
        }

        public void Persist()
        {
            _unitOfWork.PersistEntities();
        }

        public void ClearDatabase()
        {
            var users = _unitOfWork.Session.Query<SlaveUser>().ToList();
            foreach (var slaveUser in users)
            {
                _unitOfWork.AddToDelete(slaveUser);
            }
        }

        public IEnumerable<SlaveUser> GetUsers()
        {
            return _unitOfWork.Session.Query<SlaveUser>().ToList();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}