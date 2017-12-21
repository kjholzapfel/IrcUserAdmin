using System;
using System.Collections.Generic;
using IrcUserAdmin.Entities;

namespace IrcUserAdmin.NHibernate
{
    public interface IUserDao : IDisposable
    {
        IEnumerable<Group> GetGroupList();
        User GetUserFromIdent(string ident);
        bool UserNameExists(string username);
        bool GroupExists(string groupname);
        void AddNewUserToGroup(string username, string hashedPassword, string groupname);
        void AddGroup(string groupname);
        bool UserIsAdminOfGroup(string username, string groupname);
        void AddUserAsAdminToGroup(string username, string groupname);
        void RemoveUserAsAdminFromGroup(string username, string groupname);
        bool GroupChannelExists(string channel);
        void AddChannelToGroup(string channel, string group);
        void SetAdmin(string username, bool admin);
        void SetOper(string username, bool oper);
        List<string> GetUsers();
        void DeleteGroup(string groupname);
        void DeleteUser(string username);
        void ChangePassword(string username, string password);
        User GetUserInfo(string username);
        bool UserExistsInGroup(string username, string groupname);
        void AddUserToGroup(string username, string groupname);
        void RemoveUserFromGroup(string username, string groupname);
        Group GetGroupDetails(string groupname);
        void DisableAutoJoin(string username);
        void EnableAutoJoin(string username);
        void SetVHost(string username, string vhost);
        void AddHosts(string username, IList<string> hosts);
        bool HostExists(string username, string hostname);
        void RemoveHosts(string username, IList<string> hosts);
        void Persist();
    }
} 