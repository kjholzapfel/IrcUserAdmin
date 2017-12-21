using System;
using System.Collections.Generic;
using IrcUserAdmin.Entities.Slave;

namespace IrcUserAdmin.SlavePersistance
{
    public interface ISlavePersistence : IDisposable
    {
        void AddNewUser(string username, string hashedPassword);
        void DeleteUser(string username);
        void ChangePassword(string username, string password);
        void AddHosts(string username, IEnumerable<string> hosts);
        void RemoveHosts(string username, IEnumerable<string> hosts);
        void Persist();
        void ClearDatabase();
        IEnumerable<SlaveUser> GetUsers();
    }
}