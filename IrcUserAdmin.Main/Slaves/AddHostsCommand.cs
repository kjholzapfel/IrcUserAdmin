using System;
using System.Collections.Generic;
using IrcUserAdmin.SlavePersistance;

namespace IrcUserAdmin.Slaves
{
    public class AddHostsCommand : ISlaveCommand
    {
        private readonly string _username;
        private readonly IList<string> _hosts;

        public AddHostsCommand(string username, IList<string> hosts)
        {
            if (username == null) throw new ArgumentNullException("username");
            if (hosts == null) throw new ArgumentNullException("hosts");
            _username = username;
            _hosts = hosts;
        }

        public void Execute(ISlavePersistence slaveDao)
        {
            slaveDao.AddHosts(_username, _hosts);
        }
    }
}