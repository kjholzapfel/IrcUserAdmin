using System;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public class AddUserCommand : ISlaveCommand
    {
        private readonly string _username;
        private readonly string _hashedPassword;

        public AddUserCommand(string username, string hashedPassword)
        {
            if (username == null) throw new ArgumentNullException("username");
            if (hashedPassword == null) throw new ArgumentNullException("hashedPassword");
            _username = username;
            _hashedPassword = hashedPassword;
        }

        public void Execute(ISlavePersistence slaveDao)
        {
           slaveDao.AddNewUser(_username, _hashedPassword);
        }
    }
}