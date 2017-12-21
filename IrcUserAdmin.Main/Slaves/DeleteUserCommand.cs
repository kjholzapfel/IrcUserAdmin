using System;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public class DeleteUserCommand : ISlaveCommand
    {
        private readonly string _username;

        public DeleteUserCommand(string username)
        {
            if (username == null) throw new ArgumentNullException("username");
            _username = username;
        }

        public void Execute(ISlavePersistence slaveDao)
        {
            slaveDao.DeleteUser(_username);
        }
    }
}