using System;
using System.Collections.Generic;
using System.Linq;
using IrcUserAdmin.Common.NHibernate;
using IrcUserAdmin.Entities;
using NHibernate.Linq;

namespace IrcUserAdmin.NHibernate
{
    public class UserDao : IUserDao
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserDao(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public IEnumerable<Group> GetGroupList()
        {
            return _unitOfWork.Session.Query<Group>().ToList();
        }

        public User GetUserFromIdent(string ident)
        {
            var user = GetUser(ident);
            return user;
        }

        public bool UserNameExists(string username)
        {
            return _unitOfWork.Session.Query<User>().Any(u => u.Name.ToLowerInvariant() ==  username.ToLowerInvariant());
        }

        public bool GroupExists(string groupname)
        {
            return _unitOfWork.Session.Query<Group>().Any(u => u.GroupName.ToLowerInvariant() ==  groupname.ToLowerInvariant());
        }

        public void AddNewUserToGroup(string username, string hashedPassword, string groupname)
        {
            var user = new User { Name = username, Password = hashedPassword };
            Group selectedgroup = GetGroup(groupname);
            user.Groups = new List<Group> {selectedgroup};
            _unitOfWork.AddToSave(user);
        }

        public void AddGroup(string groupname)
        {
            var newGroup = new Group {GroupName = groupname};
            _unitOfWork.AddToSave(newGroup);
        }

        public bool UserIsAdminOfGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var groupExists = GetGroup(groupname);
            if (groupExists != null && groupExists.GroupAdmins.Contains(user))
            { 
                return true;
            }
            return false;
        }

        public void AddUserAsAdminToGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var groupExists = GetGroup(groupname);
            if (groupExists != null)
            {
                groupExists.GroupAdmins.Add(user);
                _unitOfWork.AddToSave(groupExists);
            }
        }

        public void RemoveUserAsAdminFromGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var groupExists = GetGroup(groupname);
            if (groupExists != null)
            {
                groupExists.GroupAdmins.Remove(user);
                _unitOfWork.AddToSave(groupExists);
            }
        }

        public bool GroupChannelExists(string channel)
        {
            return _unitOfWork.Session.Query<Group>().Any(c => c.Channel.ToLower() == channel.ToLower());
        }

        public void AddChannelToGroup(string channel, string groupname)
        {
            var group = GetGroup(groupname);
            if (group != null)
            {
                group.Channel = channel;
                _unitOfWork.AddToSave(group);
            }
        }

        public void SetAdmin(string username, bool admin)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.IsAdmin = admin;
                _unitOfWork.AddToSave(user);
            }
        }

        public void SetOper(string username, bool oper)
        {
            var user = GetUser(username);
            if (user != null)
            {
                user.IsOper = oper;
                _unitOfWork.AddToSave(user);
            }
        }

        public List<string> GetUsers()
        {
            return _unitOfWork.Session.Query<User>().Select(s => s.Name).ToList();
        }

        public void DeleteGroup(string groupname)
        {
            var group = _unitOfWork.Session.Query<Group>().FirstOrDefault(s => s.GroupName.ToLowerInvariant() == groupname.ToLowerInvariant());
            _unitOfWork.AddToDelete(group);
        }

        public void DeleteUser(string username)
        {
            var user = _unitOfWork.Session.Query<User>().FirstOrDefault(s => s.Name.ToLowerInvariant() == username.ToLowerInvariant());
            _unitOfWork.AddToDelete(user);
        }

        public void ChangePassword(string username, string password)
        {
            var user = GetUser(username);
            user.Password = password;
            _unitOfWork.AddToSave(user);
        }

        public User GetUserInfo(string username)
        {
            var user = _unitOfWork.Session.Query<User>().FirstOrDefault(s => s.Name.ToLowerInvariant() == username.ToLowerInvariant());
            return user;
        }

        public bool UserExistsInGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var group = GetGroup(groupname);
            return group.Members.Contains(user);
        }

        public void AddUserToGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var group = GetGroup(groupname);
            group.Members.Add(user);
            _unitOfWork.AddToSave(group);
        }

        public void RemoveUserFromGroup(string username, string groupname)
        {
            var user = GetUser(username);
            var group = GetGroup(groupname);
            group.Members.Remove(user);
            _unitOfWork.AddToSave(group);
        }

        public Group GetGroupDetails(string groupname)
        {
            return GetGroup(groupname);
        }

        public void DisableAutoJoin(string username)
        {
            var user = GetUser(username);
            user.AutoJoin = false;
            _unitOfWork.AddToSave(user);
        }

        public void EnableAutoJoin(string username)
        {
            var user = GetUser(username);
            user.AutoJoin = false;
            _unitOfWork.AddToSave(user);
        }

        public void SetVHost(string username, string vhost)
        {
            var user = GetUser(username);
            user.Vhost = vhost;
            _unitOfWork.AddToSave(user);
        }

        public void AddHosts(string username, IList<string> hosts)
        {
            var user = GetUser(username);
            var existingsHost = user.Hosts.ToList(); //materialises host entities
            foreach (var host in hosts)
            {
                if (existingsHost.All(h => !string.Equals(h.Host, host, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var hostname = new Hostname { Host = host, User = user };
                    user.Hosts.Add(hostname);
                    _unitOfWork.AddToSave(hostname);
                }
            }
        }

        public bool HostExists(string username, string hostname)
        {
            var user = GetUser(username);
            var existingsHost = user.Hosts.ToList(); //materialises host entities
            return existingsHost.Any(h => string.Equals(h.Host, hostname, StringComparison.InvariantCultureIgnoreCase));
        }


        public void Persist()
        {
            _unitOfWork.PersistEntities();
        }

        public void RemoveHosts(string username, IList<string> hosts)
        {
            var user = GetUser(username);
            var existingsHost = user.Hosts.ToList(); //materialises host entities
            foreach (var host in hosts)
            {
                string host1 = host;
                var removeHost = existingsHost.FirstOrDefault(h => string.Equals(h.Host, host1, StringComparison.InvariantCultureIgnoreCase));
                if (removeHost != null)
                {
                    user.Hosts.Remove(removeHost);
                    _unitOfWork.AddToDelete(removeHost);
                }
            }
            _unitOfWork.AddToSave(user);
        } 

        private User GetUser(string username)
        {
            var savedUser = _unitOfWork.SavedEntities.OfType<User>().FirstOrDefault(u => u.Name.ToLowerInvariant() == username.ToLowerInvariant());
            if (savedUser != null)
            {
                return savedUser;
            }
            var user = _unitOfWork.Session.Query<User>().FirstOrDefault(s => s.Name.ToLowerInvariant() == username.ToLowerInvariant());
            return user;
        }

        private Group GetGroup(string groupname)
        {
            var savedGroup = _unitOfWork.SavedEntities.OfType<Group>().FirstOrDefault(u => u.GroupName.ToLowerInvariant() == groupname.ToLowerInvariant());
            if (savedGroup != null)
            {
                return savedGroup;
            }
            var group = _unitOfWork.Session.Query<Group>().FirstOrDefault(s => s.GroupName.ToLowerInvariant() == groupname.ToLowerInvariant());
            return group;
        }
    }
}