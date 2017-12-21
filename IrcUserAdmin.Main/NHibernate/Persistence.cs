using System;
using System.Collections.Generic;
using IrcUserAdmin.Entities;
using IrcUserAdmin.Slaves;

namespace IrcUserAdmin.NHibernate
{
    public class Persistence : IPersistance
    {
        private readonly IUserDao _dao;
        private readonly ISlaveExecutor _slaveExecutor;
        private readonly IList<ISlaveCommand> _slaveCommands;

        public Persistence(IUserDao dao, ISlaveExecutor slaveExecutor)
        {
            _dao = dao ?? throw new ArgumentNullException(nameof(dao));
            _slaveExecutor = slaveExecutor ?? throw new ArgumentNullException(nameof(slaveExecutor));
            _slaveCommands = new List<ISlaveCommand>();
        }

        public void Dispose()
        {
            _dao.Dispose();
        }

        public IEnumerable<Group> GetGroupList()
        {
            return _dao.GetGroupList();
        }

        public User GetUserFromIdent(string ident)
        {
            return _dao.GetUserFromIdent(ident);
        }

        public bool UserNameExists(string username)
        {
            return _dao.UserNameExists(username);
        }

        public bool GroupExists(string groupname)
        {
            return _dao.GroupExists(groupname);
        }

        public void AddNewUserToGroup(string username, string hashedPassword, string groupname)
        {
            _slaveCommands.Add(new AddUserCommand(username, hashedPassword));
            _dao.AddNewUserToGroup(username, hashedPassword, groupname);
        }

        public void AddGroup(string groupname)
        {
            _dao.AddGroup(groupname);
        }

        public bool UserIsAdminOfGroup(string username, string groupname)
        {
            return _dao.UserIsAdminOfGroup(username, groupname);
        }

        public void AddUserAsAdminToGroup(string username, string groupname)
        {
            _dao.AddUserAsAdminToGroup(username, groupname);
        }

        public void RemoveUserAsAdminFromGroup(string username, string groupname)
        {
            _dao.RemoveUserAsAdminFromGroup(username, groupname);
        }

        public bool GroupChannelExists(string channel)
        {
            return _dao.GroupChannelExists(channel);
        }

        public void AddChannelToGroup(string channel, string group)
        {
            _dao.AddChannelToGroup(channel, group);
        }

        public void SetAdmin(string username, bool admin)
        {
            _dao.SetAdmin(username, admin);
        }

        public void SetOper(string username, bool oper)
        {
            _dao.SetOper(username, oper);
        }

        public List<string> GetUsers()
        {
            return _dao.GetUsers();
        }

        public void DeleteGroup(string groupname)
        {
            _dao.DeleteGroup(groupname);
        }

        public void DeleteUser(string username)
        {
            _slaveCommands.Add(new DeleteUserCommand(username));
            _dao.DeleteUser(username);
        }

        public void ChangePassword(string username, string password)
        {
            _slaveCommands.Add(new ChangePasswordCommand(username, password));
            _dao.ChangePassword(username, password);
        }

        public User GetUserInfo(string username)
        {
            return _dao.GetUserInfo(username);
        }

        public bool UserExistsInGroup(string username, string groupname)
        {
            return _dao.UserExistsInGroup(username, groupname);
        }

        public void AddUserToGroup(string username, string groupname)
        {
            _dao.AddUserToGroup(username, groupname);
        }

        public void RemoveUserFromGroup(string username, string groupname)
        {
            _dao.RemoveUserFromGroup(username, groupname);
        }

        public Group GetGroupDetails(string groupname)
        {
            return _dao.GetGroupDetails(groupname);
        }

        public void DisableAutoJoin(string username)
        {
            _dao.DisableAutoJoin(username);
        }

        public void EnableAutoJoin(string username)
        {
            _dao.EnableAutoJoin(username);
        }

        public void SetVHost(string username, string vhost)
        {
            _dao.SetVHost(username, vhost);
        }

        public void AddHosts(string username, IList<string> hosts)
        {
            _slaveCommands.Add(new AddHostsCommand(username, hosts));
            _dao.AddHosts(username, hosts);
        }

        public bool HostExists(string username, string hostname)
        {
            return _dao.HostExists(username, hostname);
        }

        public void RemoveHosts(string username, IList<string> hosts)
        {
            _slaveCommands.Add(new DeleteHostsCommand(username, hosts));
            _dao.RemoveHosts(username, hosts);
        }

        public void Persist()
        {
            _dao.Persist();
            _slaveExecutor.ExecuteCommandsOnSlaves(_slaveCommands);
        }
    }

    public interface IPersistance : IUserDao
    {
    }
}