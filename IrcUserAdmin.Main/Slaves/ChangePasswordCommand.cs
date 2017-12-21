using System;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public class ChangePasswordCommand : ISlaveCommand
    {
        private readonly string _username;
        private readonly string _hashedPassword;

        public ChangePasswordCommand(string username, string hashedPassword)
        {
            if (username == null) throw new ArgumentNullException("username");
            if (hashedPassword == null) throw new ArgumentNullException("hashedPassword");
            _username = username;
            _hashedPassword = hashedPassword;
        }

        public void Execute(ISlavePersistence slaveDao)
        {
            slaveDao.ChangePassword(_username, _hashedPassword);
        }
    }
}